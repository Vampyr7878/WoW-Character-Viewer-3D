using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using WoW;

// Class to render item 3d models
public class ItemObject : ModelRenderer
{
    // Swappable item texture type 2
    public int Texture2 { get; private set; }
    // Swappable item texture type 3
    public int Texture3 { get; private set; }
    // Swappable item texture type 4 
    public int Texture4 { get; private set; }
    // File id of the model
    public int File { get; private set; }
    // Index of extra geoset to be enabled
    public int Geoset { get; set; }
    // Particle colors from database
    public ParticleColor[] ParticleColors { get; set; }

    // Reference to the main camera
    private new Transform camera;

    private void Start()
    {
        // Initiazlie
        Change = false;
        Loaded = false;
        camera = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (Loaded)
        {
            // Steup and render model
            if (Change)
            {
                try
                {
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    LoadTextures();
                    ParticleEffects();
                    for (int i = 0; i < Model.Skin.Textures.Length; i++)
                    {
                        SetMaterial(renderer, i);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                }
                Change = false;
            }
            // Rotate billboards
            for (int i = 0; i < Model.Skeleton.Bones.Length; i++)
            {
                if ((Model.Skeleton.Bones[i].Flags & 0x8) != 0)
                {
                    renderer.bones[i].transform.eulerAngles = new Vector3(camera.eulerAngles.x + 90, camera.eulerAngles.y + 90, camera.eulerAngles.z);
                }
            }
            // Animate textures
            for (int i = 0; i < Model.Skin.Textures.Length; i++)
            {
                AnimateTextures(renderer, i);
                AnimateColors(renderer, i);
            }
            for (int i = 0; i < textureTime.Length; i++)
            {
                textureTime[i] += Time.deltaTime;
            }
            for (int i = 0; i < colorTime.Length; i++)
            {
                colorTime[i] += Time.deltaTime;
            }
            for (int i = 0; i < transparencyTime.Length; i++)
            {
                transparencyTime[i] += Time.deltaTime;
            }
        }
        else
        {
            Change = false;
        }
    }

    // Set material with proper shader
    protected override void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        if (activeGeosets.Count == 0 || activeGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id))
        {
            Material material = Resources.Load<Material>($@"Materials\{Model.Skin.Textures[i].Shader}");
            if (material == null)
            {
                Debug.LogError(Model.Skin.Textures[i].Shader);
            }
            renderer.materials[i] = new(material.shader);
            renderer.materials[i].shader = material.shader;
            renderer.materials[i].CopyPropertiesFromMaterial(material);
            renderer.sharedMesh.OptimizeReorderVertexBuffer();
            Debug.Log(Model.Skin.Textures[i].Shader);
            SetTexture(renderer.materials[i], i);
        }
        else
        {
            renderer.materials[i] = new Material(hiddenMaterial.shader);
            renderer.materials[i].shader = hiddenMaterial.shader;
            renderer.materials[i].CopyPropertiesFromMaterial(hiddenMaterial);
        }
    }

    // Setup particle effects for rendering
    private void ParticleEffects()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        particles = particles.OrderBy(x => int.Parse(x.name.Replace("Particle", ""))).ToArray();
        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem.MainModule main = particles[i].main;
            main.startRotation3D = true;
            main.startRotationZ = -Mathf.PI / 2 * Model.Particles[i].TileRotation;
            ParticleSystem.EmissionModule emission = particles[i].emission;
            if (Model.Particles[i].ColorIndex != 0)
            {
                ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles[i].colorOverLifetime;
                if (ParticleColors.Length > 0)
                {
                    ParticleColor particleColors = ParticleColors[Model.Particles[i].ColorIndex - 11];
                    if (particleColors.Start != Color.black)
                    {
                        Gradient gradient = new();
                        GradientColorKey[] colorKeys = new GradientColorKey[3];
                        colorKeys[0] = new GradientColorKey(particleColors.Start, 0f);
                        colorKeys[1] = new GradientColorKey(particleColors.Mid, 0.5f);
                        colorKeys[2] = new GradientColorKey(particleColors.End, 1f);
                        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
                        alphaKeys[0] = new GradientAlphaKey(particleColors.Start.a / 255f, 0f);
                        alphaKeys[1] = new GradientAlphaKey(particleColors.Mid.a / 255f, 0.5f);
                        alphaKeys[2] = new GradientAlphaKey(particleColors.End.a / 255f, 1f);
                        gradient.SetKeys(colorKeys, alphaKeys);
                        colorOverLifetime.color = gradient;
                    }
                }
            }
            ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = particles[i].limitVelocityOverLifetime;
            limitVelocityOverLifetime.enabled = true;
            limitVelocityOverLifetime.drag = Model.Particles[i].Drag;
            limitVelocityOverLifetime.multiplyDragByParticleSize = false;
            limitVelocityOverLifetime.multiplyDragByParticleVelocity = true;
            ParticleSystemRenderer renderer = particles[i].GetComponent<ParticleSystemRenderer>();
            Material material = ParticleMaterial(Model.Particles[i].Blend);
            particles[i].transform.localScale = transform.localScale;
            bool right = name.Contains("right");
            if (Model.Particles[i].Flags == 8652344)
            {
                particles[i].transform.localEulerAngles = new Vector3(right ? -90f : 90f, 0f, 0f);
            }
            else if (Model.Particles[i].Flags == 8782897)
            {
                particles[i].transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            }
            else
            {
                particles[i].transform.localEulerAngles = new Vector3(180f, 0f, 0f);
            }
            renderer.material.shader = material.shader;
            renderer.material.CopyPropertiesFromMaterial(material);
            Texture2D temp = textures[Model.Particles[i].Textures[0]];
            Texture2D texture = new(temp.width, temp.height, TextureFormat.ARGB32, false);
            texture.SetPixels32(temp.GetPixels32());
            texture.alphaIsTransparency = true;
            texture.Apply();
            renderer.material.SetTexture("_MainTex", texture);
            renderer.material.renderQueue = 3100;
        }
    }

    // Setup all the material properties
    protected override void SetTexture(Material material, int i)
    {
        material.SetTexture("_Texture1", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture]]);
        if (Model.Skin.Textures[i].TextureCount > 1)
        {
            material.SetTexture("_Texture2", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 1]]);
        }
        if (Model.Skin.Textures[i].TextureCount > 2)
        {
            material.SetTexture("_Emission", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 2]]);
        }
        material.SetInt("_SrcColorBlend", (int)SrcColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstColorBlend", (int)DstColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_SrcAlphaBlend", (int)SrcAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstAlphaBlend", (int)DstAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetFloat("_AlphaCut", Model.Materials[Model.Skin.Textures[i].Material].Blend == 1 || Model.Materials[Model.Skin.Textures[i].Material].Blend == 7 ? 0.5f : 0f);
        Color color = Color.white;
        if (Model.Skin.Textures[i].Color != -1)
        {
             color = colors[Model.Skin.Textures[i].Color];
        }
        CullMode cull = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x04) != 0 ? CullMode.Off : CullMode.Back;
        if (File == 850143)
        {
            cull = CullMode.Off;
        }
        material.SetInt("_Cull", (int)cull);
        float depth = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x10) != 0 ? 0f : 1f;
        material.SetFloat("_DepthTest", depth);
        if (Model.Transparencies[Model.TransparencyLookup[Model.Skin.Textures[i].Transparency]].Transparency.Values[0].Length > 0)
        {
            color.a *= Model.Transparencies[Model.TransparencyLookup[Model.Skin.Textures[i].Transparency]].Transparency.Values[0][0];
        }
        material.SetColor("_Color", color);
        material.renderQueue += Model.Skin.Textures[i].Priority;
    }

    // Load specific texture
    private int LoadTexture(M2Texture texture, int i)
    {
        int file = 0;
        switch (texture.Type)
        {
            case 0:
                file = Model.TextureIDs[i];
                break;
            case 2:
                file = Texture2;
                break;
            case 3:
                file = Texture3;
                break;
            case 4:
                file = Texture4;
                break;
        }
        return file;
    }

    // Load and prepare all model textures
    public void LoadTextures()
    {
        for (int i = 0; i < textures.Length; i++)
        {
            int file = LoadTexture(Model.Textures[i], i);
            if (file == 0)
            {
                textures[i] = new Texture2D(200, 200);
            }
            else
            {
                Texture2D texture = TextureFromBLP(file);
                if (texture == null)
                {
                    Debug.LogError(file);
                }
                textures[i] = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
                textures[i].SetPixels32(texture.GetPixels32());
                textures[i].wrapModeU = (Model.Textures[i].Flags & 1) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                textures[i].wrapModeV = (Model.Textures[i].Flags & 2) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                textures[i].Apply();
            }
        }
    }

    // Unload the model
    public void UnloadModel()
    {
        DestroyImmediate(mesh);
        File = 0;
        Loaded = false;
    }

    // Load the model
#if UNITY_EDITOR
    public IEnumerator LoadModel(int file, ItemTexture[] textures, Dictionary<int, string> listFile, string dataPath)
#else
    public IEnumerator LoadModel(int file, ItemTexture[] textures, CASCHandler casc)
#endif
    {
        File = file;
        Texture2 = textures.Length > 0 ? textures[0].ID : 0;
        Texture3 = textures.Length > 1 ? textures[1].ID : 0;
        Texture4 = textures.Length > 2 ? textures[2].ID : 0;
#if UNITY_EDITOR
        this.listFile = listFile;
        this.dataPath = dataPath;
#else
        this.casc = casc;
#endif
        converter = new System.Drawing.ImageConverter();
        byte[] bytes;
        yield return null;
#if UNITY_EDITOR
        bytes = GetFileBytes(file, listFile, dataPath);
#else
        bytes = GetFileBytes(file, casc);
#endif
        yield return null;
        Model = new M2();
        Model.LoadFile(bytes);
        yield return null;
        if (Model.SkelFileID != 0)
        {
#if UNITY_EDITOR
            bytes = GetFileBytes(Model.SkelFileID, listFile, dataPath);
#else
            bytes = GetFileBytes(Model.SkelFileID, casc);
#endif
        }
        yield return null;
        Model.Skeleton.LoadFile(bytes, Model.SkelFileID);
        yield return null;
#if UNITY_EDITOR
        bytes = GetFileBytes(Model.SkinFileID, listFile, dataPath);
#else
        bytes = GetFileBytes(Model.SkinFileID, casc);
#endif
        yield return null;
        Model.Skin.LoadFile(bytes);
        yield return null;
        activeGeosets = Model.Skin.Submeshes.Select(s => (int)s.Id).Distinct().ToList();
        if (Geoset > 0)
        {
            activeGeosets.Remove(Model.Skin.Submeshes[Model.Skin.Textures[Geoset].Id].Id);
        }
        LoadColors();
        yield return null;
        this.textures = new Texture2D[Model.Textures.Length];
        mesh = WoWHelper.Generate3DMesh(Model);
        mesh.transform.parent = GetComponent<Transform>();
        mesh.transform.localPosition = Vector3.zero;
        mesh.transform.localEulerAngles = Vector3.zero;
        mesh.transform.localScale = Vector3.one;
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        yield return null;
        Transform[] bones = GetComponentInChildren<SkinnedMeshRenderer>().bones;
        if (Model.Particles.Length > 0)
        {
            GameObject[] particles = new GameObject[Model.Particles.Length];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = WoWHelper.ParticleEffect(Model.Particles[i]);
                particles[i].transform.parent = bones[Model.Particles[i].Bone];
                particles[i].transform.localPosition = Vector3.zero;
                particles[i].name = $"Particle{i}";
                Debug.Log($"Particle{i}: Flags-{Model.Particles[i].Flags}\tBlend-{Model.Particles[i].Blend}\tSource-{Model.Particles[i].Source}");
                yield return null;
            }
        }
        textureTime = new float[Model.TextureAnimations.Length];
        textureFrame = new int[Model.TextureAnimations.Length];
        colorTime = new float[Model.TextureAnimations.Length];
        colorFrame = new int[Model.TextureAnimations.Length];
        transparencyTime = new float[Model.TextureAnimations.Length];
        transparencyFrame = new int[Model.TextureAnimations.Length];
        for (int i = 0; i < textureTime.Length; i++)
        {
            textureTime[i] = 0f;
            textureFrame[i] = 0;
            yield return null;
        }
        for (int i = 0; i < colorTime.Length; i++)
        {
            colorTime[i] = 0f;
            colorFrame[i] = 0;
            yield return null;
        }
        for (int i = 0; i < transparencyTime.Length; i++)
        {
            transparencyTime[i] = 0f;
            transparencyFrame[i] = 0;
            yield return null;
        }
        for (int i = 0; i < bones.Length; i++)
        {
            if ((Model.Skeleton.Bones[i].Flags & 0x400) != 0 && Model.Skeleton.Bones[i].Parent >= 0)
            {
                bones[i].gameObject.AddComponent<Rigidbody>();
                bones[i].gameObject.AddComponent<HingeJoint>().axis = new Vector3(0.5f, 0.5f, 0f);
                HingeJoint joint = bones[i].GetComponent<HingeJoint>();
                joint.autoConfigureConnectedAnchor = false;
                Rigidbody parent = bones[Model.Skeleton.Bones[i].Parent].GetComponent<Rigidbody>();
                if (parent == null)
                {
                    bones[Model.Skeleton.Bones[i].Parent].gameObject.AddComponent<Rigidbody>().isKinematic = true;
                    parent = bones[Model.Skeleton.Bones[i].Parent].GetComponent<Rigidbody>();
                }
                joint.connectedBody = parent;
                joint.connectedAnchor = bones[i].localPosition;
                if (bones[i].childCount == 0)
                {
                    GameObject weight = new("Weight");
                    weight.transform.parent = bones[i];
                    weight.transform.localPosition = bones[i].transform.localPosition;
                    weight.transform.localScale = Vector3.one;
                }
                joint.anchor = bones[i].InverseTransformPoint(bones[i].GetComponentInChildren<Transform>().position);
            }
        }
        Loaded = true;
        Change = true;
    }

    // Load all bytes of the file into an array
#if UNITY_EDITOR
    private byte[] GetFileBytes(int file, Dictionary<int, string> listFile, string dataPath)
    {
        using BinaryReader reader = new(System.IO.File.Open($@"{dataPath}\{listFile[file]}", FileMode.Open));
        return reader.ReadBytes((int)reader.BaseStream.Length);
    }
#else
    private byte[] GetFileBytes(int file, CASCHandler casc)
    {
        using BinaryReader reader = new(casc.OpenFile(file));
        return reader.ReadBytes((int)reader.BaseStream.Length);
    }
#endif
}
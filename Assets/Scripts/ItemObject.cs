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

//Class to render item 3d models
public class ItemObject : ModelRenderer
{
    //Reference to the main camera
    private new Transform camera;

    //Swappable item texture
    public int MainTexture { get; private set; }
    //File id of the model
    public int File { get; private set; }
    //Particle colors from database
    public ParticleColor[] ParticleColors { get; set; }

    private void Start()
    {
        //Initiazlie
        Change = false;
        Loaded = false;
        camera = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (Loaded)
        {
            //Steup and render model
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
                        if (Model.Skin.Textures[i].Layer == 0)
                        {
                            SetMaterial(renderer, i);
                        }
                        else if (Model.Skin.Textures[i].Layer == 1)
                        {
                            List<Material> materials = renderer.materials.ToList();
                            materials.Add(new Material(materials[0]));
                            renderer.materials = materials.ToArray();
                            SetLayeredMaterial(renderer, i);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                }
                Change = false;
            }
            //Rotate billboards
            for (int i = 0; i < Model.Skeleton.Bones.Length; i++)
            {
                if ((Model.Skeleton.Bones[i].Flags & 0x8) != 0)
                {
                    renderer.bones[i].transform.eulerAngles = new Vector3(camera.eulerAngles.x - 90, camera.eulerAngles.y - 90, camera.eulerAngles.z);
                }
            }
            //Animate textures
            for (int i = 0; i < Model.Skin.Textures.Length; i++)
            {
                AnimateTextures(renderer, i);
            }
            for (int i = 0; i < time.Length; i++)
            {
                time[i] += Time.deltaTime;
            }
        }
        else
        {
            Change = false;
        }
    }

    //Animate current texture unit if it has any animations
    private new void AnimateTextures(SkinnedMeshRenderer renderer, int i)
    {
        int index = Model.TextureAnimationsLookup[Model.Skin.Textures[i].TextureAnimation];
        string texture = "_Texture1";
        if (index >= 0)
        {
            Vector2 offset = renderer.materials[Model.Skin.Textures[i].Id].GetTextureOffset(texture);
            offset = AnimateTexture(index, offset);
            renderer.materials[Model.Skin.Textures[i].Id].SetTextureOffset(texture, offset);
        }
        if (Model.Skin.Textures[i].TextureCount > 1)
        {
            index = Model.TextureAnimationsLookup[Model.Skin.Textures[i].TextureAnimation + 1];
            texture = "_Texture2";
            if (index >= 0)
            {
                Vector2 offset = renderer.materials[Model.Skin.Textures[i].Id].GetTextureOffset(texture);
                offset = AnimateTexture(index, offset);
                renderer.materials[Model.Skin.Textures[i].Id].SetTextureOffset(texture, offset);
            }
        }
    }

    //Set material with proper shader
    private void SetLayeredMaterial(SkinnedMeshRenderer renderer, int i)
    {
        Material material = Resources.Load<Material>($@"Materials\{Model.Skin.Textures[i].Shader}");
        if (material == null)
        {
            Debug.LogError(Model.Skin.Textures[i].Shader);
        }
        int index = renderer.materials.Length - 1;
        renderer.materials[index] = new Material(material.shader);
        renderer.materials[index].shader = material.shader;
        renderer.materials[index].CopyPropertiesFromMaterial(material);
        renderer.sharedMesh.OptimizeReorderVertexBuffer();
        SetTexture(renderer.materials[index], i);
    }

    //Set material with proper shader
    protected override void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        Material material = Resources.Load<Material>($@"Materials\{Model.Skin.Textures[i].Shader}");
        if (material == null)
        {
            Debug.LogError(Model.Skin.Textures[i].Shader);
        }
        renderer.materials[Model.Skin.Textures[i].Id] = new Material(material.shader);
        renderer.materials[Model.Skin.Textures[i].Id].shader = material.shader;
        renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(material);
        renderer.sharedMesh.OptimizeReorderVertexBuffer();
        SetTexture(renderer.materials[Model.Skin.Textures[i].Id], i);
    }

    //Setup particle effects for rendering
    private void ParticleEffects()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        particles = particles.OrderBy(x => int.Parse(x.name.Replace("Particle", ""))).ToArray();
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            ParticleSystem.MainModule main = particles[i].main;
            main.startRotation3D = true;
            main.startRotationZ = -Mathf.PI / 2 * Model.Particles[i].TileRotation;
            ParticleSystem.EmissionModule emission = particles[i].emission;
            if (Model.Name.Contains("RaidWarrior_N"))
            {
                main.startSize = main.startSize.constant / 2f;
            }
            emission.rateOverTimeMultiplier = 4f;
            if (Model.Particles[i].ColorIndex != 0)
            {
                ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles[i].colorOverLifetime;
                ParticleColor particleColors = ParticleColors[Model.Particles[i].ColorIndex - 11];
                if (particleColors != null)
                {
                    Gradient gradient = new Gradient();
                    GradientColorKey[] colorKeys = new GradientColorKey[3];
                    colorKeys[0] = new GradientColorKey(particleColors.Start, 0f);
                    colorKeys[1] = new GradientColorKey(particleColors.Mid, 0.5f);
                    colorKeys[2] = new GradientColorKey(particleColors.End, 1f);
                    GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
                    alphaKeys[0] = new GradientAlphaKey(particleColors.Start.a, 0f);
                    alphaKeys[1] = new GradientAlphaKey(particleColors.Mid.a, 0.5f);
                    alphaKeys[2] = new GradientAlphaKey(particleColors.End.a, 1f);
                    gradient.SetKeys(colorKeys, alphaKeys);
                    colorOverLifetime.color = gradient;
                }
            }
            ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = particles[i].limitVelocityOverLifetime;
            limitVelocityOverLifetime.enabled = true;
            limitVelocityOverLifetime.drag = Model.Particles[i].Drag;
            limitVelocityOverLifetime.multiplyDragByParticleSize = false;
            limitVelocityOverLifetime.multiplyDragByParticleVelocity = true;
            if (Model.Name.Contains("Shoulder_Leather_RaidDruid_I_01") && i > 0)
            {
                ParticleSystem.TextureSheetAnimationModule textureSheet = particles[i].textureSheetAnimation;
                textureSheet.frameOverTime = 0.109375f;
            }
            ParticleSystemRenderer renderer = particles[i].GetComponent<ParticleSystemRenderer>();
            Material material = ParticleMaterial(Model.Particles[i].Blend);
            particles[i].transform.localScale = transform.lossyScale;
            if (name.Contains("right") && (Model.Particles[i].Flags & 512) == 0)
            {
                renderer.flip = new Vector3(1f, 0f, 0f);
                main.startRotationZ = Mathf.PI / 2 * Model.Particles[i].TileRotation;
            }
            renderer.material.shader = material.shader;
            renderer.material.CopyPropertiesFromMaterial(material);
            Texture2D temp = textures[Model.Particles[i].Textures[0]];
            Texture2D texture = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
            texture.SetPixels32(temp.GetPixels32());
            texture.Apply();
            renderer.material.SetTexture("_MainTex", texture);
            renderer.material.renderQueue = 3100;
        }
    }

    //Setup all the material properties
    protected override void SetTexture(Material material, int i)
    {
        material.SetTexture("_Texture1", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture]]);
        if (Model.Skin.Textures[i].TextureCount > 1)
        {
            material.SetTexture("_Texture2", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 1]]);
        }
        if (Model.Skin.Textures[i].TextureCount > 2)
        {
            int index = Model.TextureLookup[Model.Skin.Textures[i].Texture + 2];
            Texture2D emission = new Texture2D(textures[index].width, textures[index].height);
            for (int x = 0; x < emission.width; x++)
            {
                for (int y = 0; y < emission.height; y++)
                {
                    Color pixel = textures[index].GetPixel(x, y);
                    pixel = new Color(pixel.a, pixel.a, pixel.a);
                    emission.SetPixel(x, y, pixel);
                }
            }
            emission.Apply();
            material.SetTexture("_Emission", emission);
        }
        material.SetInt("_SrcBlend", (int)SrcBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstBlend", (int)DstBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetFloat("_AlphaCut", Model.Materials[Model.Skin.Textures[i].Material].Blend == 1 || Model.Materials[Model.Skin.Textures[i].Material].Blend == 7 ? 0.5f : 0f);
        Color color = Color.white;
        if (Model.Skin.Textures[i].Color != -1)
        {
             color = colors[Model.Skin.Textures[i].Color];
        }
        CullMode cull = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x04) != 0 ? CullMode.Off : CullMode.Front;
        material.SetInt("_Cull", (int)cull);
        float depth = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x10) != 0 ? 0f : 1f;
        material.SetFloat("_DepthTest", depth);
        color.a = Model.Transparencies[Model.TransparencyLookup[Model.Skin.Textures[i].Transparency]];
        if(Model.Skin.Textures[i].Layer > 0)
        {
            color.a *= 0.25f;
        }
        material.SetColor("_Color", color);
        material.renderQueue += Model.Skin.Textures[i].Priority;
    }

    //Load specific texture
    private int LoadTexture(M2Texture texture, int i)
    {
        int file = 0;
        switch (texture.Type)
        {
            case 0:
                file = Model.TextureIDs[i];
                break;
            case 2:
                file = MainTexture;
                break;
        }
        return file;
    }

    //Load and prepare all model textures
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

    //Unload the model
    public void UnloadModel()
    {
        DestroyImmediate(mesh);
        File = 0;
        Loaded = false;
    }

    //Load the model
    public IEnumerator LoadModel(int file, int texture, CASCHandler casc)
    {
        File = file;
        MainTexture = texture;
        this.casc = casc;
        converter = new System.Drawing.ImageConverter();
        byte[] bytes;
        yield return null;
        using (BinaryReader reader = new BinaryReader(casc.OpenFile(file)))
        {
            bytes = reader.ReadBytes((int)reader.BaseStream.Length);
        }
        yield return null;
        Model = new M2();
        Model.LoadFile(bytes);
        yield return null;
        if (Model.SkelFileID != 0)
        {
            using (BinaryReader reader = new BinaryReader(casc.OpenFile(Model.SkelFileID)))
            {
                bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }
        yield return null;
        Model.Skeleton.LoadFile(bytes, Model.SkelFileID);
        yield return null;
        using (BinaryReader reader = new BinaryReader(casc.OpenFile(Model.SkinFileID)))
        {
            bytes = reader.ReadBytes((int)reader.BaseStream.Length);
        }
        yield return null;
        Model.Skin.LoadFile(bytes);
        yield return null;
        LoadColors();
        yield return null;
        textures = new Texture2D[Model.Textures.Length];
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
                yield return null;
            }
        }
        time = new float[Model.TextureAnimations.Length];
        frame = new int[Model.TextureAnimations.Length];
        for (int i = 0; i < time.Length; i++)
        {
            time[i] = 0f;
            frame[i] = 0;
            yield return null;
        }
        Loaded = true;
        Change = true;
    }
}
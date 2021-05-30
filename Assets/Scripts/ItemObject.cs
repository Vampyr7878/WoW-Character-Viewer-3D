﻿using CASCLib;
using M2Lib;
using System;
using System.Collections;
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
    private int mainTexture;

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
                    for (int i = 0; i < Model.Skin.Submeshes.Length; i++)
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
            //Rotate billboards
            for (int i = 0; i < Model.Skeleton.Bones.Length; i++)
            {
                if ((Model.Skeleton.Bones[i].Flags & 0x8) != 0)
                {
                    renderer.bones[i].transform.eulerAngles = new Vector3(camera.eulerAngles.x, camera.eulerAngles.y - 90, camera.eulerAngles.z);
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
    protected override void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        Material material;
        //if (Model.Name.ToLower().Contains("offhand_1h_artifactskulloferedar") && i % 2 == 0)
        //{
        //    renderer.materials[Model.Skin.Textures[i].Id] = new Material(hidden.shader);
        //    renderer.materials[Model.Skin.Textures[i].Id].shader = hidden.shader;
        //    renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(hidden);
        //    return;
        //}
        //if (Model.Skin.Textures[i].Shader == 32783)
        //{
        //    material = Resources.Load<Material>(@"Materials\32783s");
        //}
        //else
        //{
        material = Resources.Load<Material>($@"Materials\{Model.Skin.Textures[i].Shader}");
        //}
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
        particles = particles.OrderBy(x => x.name).ToArray();
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
            ParticleSystem.EmissionModule emission = particles[i].emission;
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
            ParticleSystemRenderer renderer = particles[i].GetComponent<ParticleSystemRenderer>();
            Material material;
            particles[i].transform.localScale = transform.lossyScale;
            if (Model.Particles[i].Blend == 2)
            {
                material = Resources.Load<Material>(@"Materials\particlefadematerial");
            }
            else
            {
                material = Resources.Load<Material>(@"Materials\particleadditivematerial");

            }
            if (name.Contains("right") && Model.Particles[i].TileRotation != 0)
            {
                renderer.flip = new Vector3(1f, 0f, 0f);
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
            material.SetTexture("_Emission", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 2]]);
        }
        material.SetInt("_SrcBlend", (int)SrcBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstBlend", (int)DstBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        if (Model.Skin.Textures[i].Color != -1)
        {
            material.SetColor("_Color", colors[Model.Skin.Textures[i].Color]);
        }
        CullMode cull = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x04) != 0 ? CullMode.Off : CullMode.Front;
        material.SetInt("_Cull", (int)cull);
        float depth = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x10) != 0 ? 0f : 1f;
        material.SetFloat("_DepthTest", depth);
        float alpha = Model.Transparencies[Model.TransparencyLookup[Model.Skin.Textures[i].Transparency]];
        Color color = new Color(1, 1, 1, alpha);
        material.SetColor("_Color", color);
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
                file = mainTexture;
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
                if (Model.Textures[i].Flags == 0)
                {
                    textures[i].wrapMode = TextureWrapMode.Clamp;
                }
                else
                {
                    textures[i].wrapMode = TextureWrapMode.Repeat;
                }
                textures[i].Apply();
            }
        }
    }

    //Unload the model
    public void UnloadModel()
    {
        DestroyImmediate(mesh);
        Loaded = false;
    }

    //Load the model
    public IEnumerator LoadModel(int file, int texture, CASCHandler casc)
    {
        mainTexture = texture;
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
        //Array.Sort(Model.Skin.Textures, (a, b) => Model.Materials[a.Material].Blend.CompareTo(Model.Materials[b.Material].Blend));
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
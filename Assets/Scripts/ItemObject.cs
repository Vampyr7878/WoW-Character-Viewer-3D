using M2Lib;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ItemObject : MonoBehaviour
{
    private GameObject mesh;
    private Color[] colors;
    private Texture2D[] textures;
    private bool loaded;

    public M2 Model { get; protected set; }
    public Material hidden;

    public string Path { get; set; }
    public string Texture { get; set; }
    public ParticleColor[] ParticleColors { get; set; }
    public bool Change { get; set; }

    void Start()
    {
        Change = false;
        loaded = false;
    }

    void FixedUpdate()
    {
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (loaded)
        {
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
            for (int i = 0; i < Model.Skeleton.Bones.Length; i++)
            {
                if ((Model.Skeleton.Bones[i].Flags & 0x8) != 0)
                {
                    renderer.bones[i].eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y - 90, Camera.main.transform.eulerAngles.z);
                }
            }
        }
        else
        {
            Change = false;
        }
    }

    private void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        Material material;
        //if (Model.Name.ToLower().Contains("offhand_1h_artifactskulloferedar") && i % 2 == 0)
        //{
        //    renderer.materials[Model.Skin.Textures[i].Id] = new Material(hidden.shader);
        //    renderer.materials[Model.Skin.Textures[i].Id].shader = hidden.shader;
        //    renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(hidden);
        //    return;
        //}
        if (Model.Skin.Textures[i].Shader == 32783)
        {
            material = Resources.Load<Material>(@"Materials\32783s");
        }
        else
        {
            material = Resources.Load<Material>($@"Materials\{Model.Skin.Textures[i].Shader}");
        }
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
            Debug.Log(Model.TextureIDs[Model.Particles[i].Textures[0]]);
            Texture2D temp = textures[Model.Particles[i].Textures[0]];
            Texture2D texture = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
            texture.SetPixels32(temp.GetPixels32());
            texture.Apply();
            renderer.material.SetTexture("_MainTex", texture);
            renderer.material.renderQueue = 3100;
        }
    }

    private BlendMode SrcBlend(short value)
    {
        BlendMode blend = BlendMode.One;
        switch (value)
        {
            case 0:
            case 1:
                blend = BlendMode.One;
                break;
            case 2:
            case 4:
                blend = BlendMode.SrcAlpha;
                break;
            case 3:
            case 7:
                blend = BlendMode.SrcColor;
                break;
            case 5:
            case 6:
                blend = BlendMode.DstColor;
                break;
        }
        return blend;
    }

    private BlendMode DstBlend(short value)
    {
        BlendMode blend = BlendMode.Zero;
        switch (value)
        {
            case 0:
            case 1:
            case 5:
                blend = BlendMode.Zero;
                break;
            case 2:
            case 7:
                blend = BlendMode.OneMinusSrcAlpha;
                break;
            case 3:
            case 4:
                blend = BlendMode.One;
                break;
            case 6:
                blend = BlendMode.SrcColor;
                break;
        }
        return blend;
    }

    private void SetTexture(Material material, int i)
    {
        material.SetTexture("_MainTex", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture]]);
        if (Model.TextureLookup.Length == Model.Skin.Textures[i].Texture + 1)
        {
            material.SetTexture("_Second", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture]]);
        }
        else
        {
            material.SetTexture("_Second", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 1]]);
        }
        //if (((Texture2D)(material.GetTexture("_MainTex"))).alphaIsTransparency)
        //{
        //    material.SetInt("_SrcBlend", (int)SrcBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        //    material.SetInt("_DstBlend", (int)DstBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        //    float depth = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x10) != 0 ? 0f : 1f;
        //    material.SetFloat("_DepthTest", depth);
        //}
        //else
        //{
        //    material.SetInt("_SrcBlend", (int)SrcBlend(0));
        //    material.SetInt("_DstBlend", (int)DstBlend(0));
        //    float depth = 1f;
        //    material.SetFloat("_DepthTest", depth);
        //}
        float alpha = Model.Transparencies[Model.TransparencyLookup[Model.Skin.Textures[i].Transparency]];
        material.SetFloat("_Alpha", alpha);
        if (Model.Materials[Model.Skin.Textures[i].Material].Blend > 1)
        {
            material.SetFloat("_AlphaCut", 0f);
        }
        if (Model.Skin.Textures[i].Color != -1)
        {
            material.SetColor("_Color", colors[Model.Skin.Textures[i].Color]);
        }
        CullMode cull = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x04) != 0 ? CullMode.Off : CullMode.Front;
        material.SetInt("_Cull", (int)cull);
    }

    private void LoadColors()
    {
        colors = new Color[Model.Colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(Model.Colors[i].R / 255f, Model.Colors[i].G / 255f, Model.Colors[i].B / 255f, 1.0f);
        }
    }

    private string LoadTexture(M2Texture texture, int i)
    {
        string file = "";
        switch (texture.Type)
        {
            case 0:
                file = Path + Model.TextureIDs[i].ToString();
                break;
            case 2:
                file = Path + Texture;
                break;
        }
        return file;
    }

    public void LoadTextures()
    {
        for (int i = 0; i < textures.Length; i++)
        {
            string file = LoadTexture(Model.Textures[i], i);
            if (file == "")
            {
                textures[i] = new Texture2D(200, 200);
            }
            else
            {
                Texture2D texture = Resources.Load<Texture2D>(file);
                if (texture == null)
                {
                    Debug.LogError(file);
                }
                textures[i] = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
                textures[i].SetPixels32(texture.GetPixels32());
                //textures[i].alphaIsTransparency = texture.alphaIsTransparency;
                textures[i].wrapMode = texture.wrapMode;
                textures[i].Apply();
            }
        }
    }

    public void UnloadModel()
    {
        DestroyImmediate(mesh);
        loaded = false;
    }

    public void LoadModel(string file)
    {
        GameObject prefab = Resources.Load<GameObject>(file.Replace(".m2", ""));
        mesh = Instantiate(prefab, gameObject.transform);
        if (mesh == null)
        {
            loaded = false;
            return;
        }
        else
        {
            loaded = true;
        }
        //GetComponentInChildren<M2Model>().LoadFile();
        //Model = GetComponentInChildren<M2Model>().m2.model;
        Array.Sort(Model.Skin.Textures, (a, b) => Model.Materials[a.Material].Blend.CompareTo(Model.Materials[b.Material].Blend));
        LoadColors();
        textures = new Texture2D[Model.Textures.Length];
    }
}

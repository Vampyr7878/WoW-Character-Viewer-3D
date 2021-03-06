using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using WoW;

//Class to render druid forms models
public class Druid : ModelRenderer
{
    //Reference to the character
    public Character character;

    //Reference to the main camera
    private new Transform camera;

    //List of geosets that are enabled for loading
    public List<int> ActiveGeosets { get; set; }
    //Particle colors from database
    public ParticleColor[] ParticleColors { get; set; }

    private void Start()
    {
        //Initiazlie
        modelsPath = @"creature\";
        camera = Camera.main.transform;
        ActiveGeosets = new List<int>() { 0 };
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
                    renderer.bones[i].transform.eulerAngles = new Vector3(camera.eulerAngles.x - 90, camera.eulerAngles.y - 90, camera.eulerAngles.z);
                }
            }
            //Animate textures
            if (character.Loaded)
            {
                for (int i = 0; i < Model.Skin.Textures.Length; i++)
                {
                    AnimateTextures(renderer, i);
                }
                for (int i = 0; i < time.Length; i++)
                {
                    time[i] += Time.deltaTime;
                }
            }
        }
    }

    //Set material with proper shader
    protected override void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        if (ActiveGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id))
        {
            Material material = Resources.Load<Material>($@"Materials\{Model.Skin.Textures[i].Shader}");
            if (material == null)
            {
                Debug.LogError(Model.Skin.Textures[i].Shader);
            }
            renderer.materials[Model.Skin.Textures[i].Id] = new Material(material.shader);
            renderer.materials[Model.Skin.Textures[i].Id].shader = material.shader;
            renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(material);
            SetTexture(renderer.materials[Model.Skin.Textures[i].Id], i);
        }
        else
        {
            renderer.materials[Model.Skin.Textures[i].Id] = new Material(hiddenMaterial.shader);
            renderer.materials[Model.Skin.Textures[i].Id].shader = hiddenMaterial.shader;
            renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(hiddenMaterial);
        }
    }

    //Setup particle effects for rendering
    private void ParticleEffects()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        particles = particles.OrderBy(x => x.name).ToArray();
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
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
            particles[i].transform.localScale = transform.lossyScale;
            ParticleSystemRenderer renderer = particles[i].GetComponent<ParticleSystemRenderer>();
            Material material = ParticleMaterial(Model.Particles[i].Blend);
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
        material.SetFloat("_AlphaCut", Model.Materials[Model.Skin.Textures[i].Material].Blend == 1 ? 0.5f : 0f);
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
        material.SetColor("_Color", color);
    }

    //Load specific texture
    private int LoadTexture(M2Texture texture, int i)
    {
        int file = -1;
        int index = Array.FindIndex(character.Options, o => o.Form == character.Form);
        switch (texture.Type)
        {
            case 0:
                file = Model.TextureIDs[i];
                break;
            case 11:
                file = character.Choices[index][character.Customization[index]].Textures[0].Texture1;
                break;
            case 12:
                file = character.Choices[index][character.Customization[index]].Textures[0].Texture2;
                break;
            case 13:
                file = character.Choices[index][character.Customization[index]].Textures[0].Texture3;
                break;
        }
        return file;
    }

    //Load and prepare all model textures
    private void LoadTextures()
    {
        Texture2D texture;
        for (int i = 0; i < textures.Length; i++)
        {
            int file = LoadTexture(Model.Textures[i], i);
            if (file == -1)
            {
                textures[i] = new Texture2D(200, 200);
            }
            else
            {
                texture = TextureFromBLP(file);
                textures[i] = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
                textures[i].SetPixels32(texture.GetPixels32());
                textures[i].wrapModeU = (Model.Textures[i].Flags & 1) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                textures[i].wrapModeV = (Model.Textures[i].Flags & 2) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                textures[i].Apply();
            }
        }
    }

    //Load and setup model prefab
    public IEnumerator LoadPrefab(string modelfile, CASCHandler casc)
    {
        DestroyImmediate(mesh);
        if (modelsPath == null)
        {
            modelsPath = @"creature\";
        }
        bool done = false;
        converter = new System.Drawing.ImageConverter();
        this.casc = casc;
        GameObject prefab = Resources.Load<GameObject>($"{modelsPath}{modelfile}_prefab");
        mesh = Instantiate(prefab, gameObject.transform);
        yield return null;
        M2Model m2 = GetComponentInChildren<M2Model>();
        byte[] data = m2.data.bytes;
        byte[] skin = m2.skin.bytes;
        byte[] skel = m2.skel.bytes;
        loadBinaries = new Thread(() => { Model = m2.LoadModel(data, skin, skel); done = true; });
        loadBinaries.Start();
        yield return null;
        while (loadBinaries.IsAlive)
        {
            yield return null;
        }
        if (done)
        {
            LoadColors();
            textures = new Texture2D[Model.Textures.Length];
            Transform[] bones = GetComponentInChildren<SkinnedMeshRenderer>().bones;
            yield return null;
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
            renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            Change = true;
            Loaded = !loadBinaries.IsAlive;
        }
    }

    //Load druid form model
    public void LoadModel(string modelfile, CASCHandler casc)
    {
        Loaded = false;
        if (loadBinaries != null)
        {
            loadBinaries.Abort();
        }
        StartCoroutine(LoadPrefab(modelfile, casc));
    }
}
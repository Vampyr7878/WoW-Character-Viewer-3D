using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

//Class to render druid forms models
public class Druid : ModelRenderer
{
    //Reference to the character
    public Character character;

    //Particle colors from database
    public ParticleColor[] ParticleColors { get; set; }

    private void Start()
    {
        //Initiazlie path
        modelsPath = @"creature\";
    }

    private void FixedUpdate()
    {
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        Transform camera = Camera.main.transform;
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
                    renderer.bones[i].transform.eulerAngles = new Vector3(camera.eulerAngles.x, camera.eulerAngles.y - 90, camera.eulerAngles.z);
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

    //Setup particle effects for rendering
    private void ParticleEffects()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        particles = particles.OrderBy(x => x.name).ToArray();
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
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
            Material material;
            if (Model.Particles[i].Blend == 2)
            {
                material = Resources.Load<Material>(@"Materials\particlefadematerial");
            }
            else
            {
                material = Resources.Load<Material>(@"Materials\particleadditivematerial");
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
                //textures[i].alphaIsTransparency = true;
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
                    particles[i] = ParticleEffect(Model.Particles[i]);
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
    
    //Particle shape
    private ParticleSystemShapeType ParticleShape(byte value)
    {
        ParticleSystemShapeType shape = ParticleSystemShapeType.Cone;
        switch (value)
        {
            case 1:
                shape = ParticleSystemShapeType.Rectangle;
                break;
            case 2:
                shape = ParticleSystemShapeType.Sphere;
                break;
            case 3:
                shape = ParticleSystemShapeType.Circle;
                break;
        }
        return shape;
    }

    //Generate particle effect system based on the data
    private GameObject ParticleEffect(M2Particle particle)
    {
        //Create gameobject
        GameObject element = new GameObject();
        element.AddComponent<ParticleSystem>();
        ParticleSystem system = element.GetComponent<ParticleSystem>();
        //Setup lifetime and speed in main module
        ParticleSystem.MainModule main = system.main;
        float variation = particle.LifespanVariation * particle.Lifespan;
        main.startLifetime = new ParticleSystem.MinMaxCurve((particle.Lifespan - variation) / 2, (particle.Lifespan + variation) / 2);
        variation = particle.SpeedVariation * particle.Speed;
        main.startSpeed = new ParticleSystem.MinMaxCurve((particle.Speed - variation) / 2f, (particle.Speed + variation) / 2f);
        //Setup emission rate in emission module
        ParticleSystem.EmissionModule emission = system.emission;
        variation = particle.EmissionVariation * particle.EmissionRate;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve((particle.EmissionRate - variation) / 2, (particle.EmissionRate + variation) / 2);
        emission.rateOverDistance = new ParticleSystem.MinMaxCurve(particle.EmissionRate - variation, particle.EmissionRate + variation);
        //Setup shape and scale in shape module
        ParticleSystem.ShapeModule shape = system.shape;
        shape.shapeType = ParticleShape(particle.Type);
        shape.scale = new Vector3(particle.EmissionWidth, particle.EmissionLength, particle.EmissionWidth);
        //Setup color and alpha gradients in color over lifetime module
        ParticleSystem.ColorOverLifetimeModule color = system.colorOverLifetime;
        color.enabled = true;
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[particle.Colors.Values.Length];
        for (int i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i] = new GradientColorKey(new Color(particle.Colors.Values[i].X / 255f, particle.Colors.Values[i].Y / 255f, particle.Colors.Values[i].Z / 255f), particle.Colors.Timestamps[i]);
        }
        GradientAlphaKey[] alphaKeys;
        if (particle.Alpha.Values.Length > 8)
        {
            alphaKeys = new GradientAlphaKey[particle.Alpha.Values.Length / 2];
        }
        else
        {
            alphaKeys = new GradientAlphaKey[particle.Alpha.Values.Length];
        }
        for (int i = 0, j = 0; i < alphaKeys.Length; i++, j++)
        {
            if (particle.Alpha.Values.Length > 8)
            {
                j++;
            }
            alphaKeys[i] = new GradientAlphaKey(particle.Alpha.Values[j], particle.Alpha.Timestamps[j]);
        }
        gradient.SetKeys(colorKeys, alphaKeys);
        color.color = gradient;
        //Setup size in size over lifetime module
        ParticleSystem.SizeOverLifetimeModule size = system.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        for (int i = 0; i < particle.Scale.Values.Length; i++)
        {
            curve.AddKey(particle.Scale.Timestamps[i], particle.Scale.Values[i].X);
        }
        size.size = new ParticleSystem.MinMaxCurve(1f, curve);
        //Setup texture sheet in texture sheet animation module
        ParticleSystem.TextureSheetAnimationModule textureSheet = system.textureSheetAnimation;
        textureSheet.enabled = true;
        textureSheet.numTilesX = particle.TileColumns;
        textureSheet.numTilesY = particle.TileRows;
        return element;
    }
}

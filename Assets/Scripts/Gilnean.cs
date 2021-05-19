using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using WoW;
using WoW.Characters;

public class Gilnean : MonoBehaviour
{
    public Material hiddenMaterial;
    public Character worgen;

    private CharacterHelper helper;
    private GameObject mesh;
    private Color[] colors;
    private List<int> activeGeosets;
    private string modelsPath;
    private Texture2D[] textures;
    private float[] time;
    private int[] frame;
    private Thread loadBinaries;

    public M2 Model { get; private set; }

    public bool Loaded { get; private set; }

    public string Suffix1 { get; set; }
    public string Suffix2 { get; set; }
    public string RacePath { get; set; }
    public bool Gender { get; set; }
    public bool Change { get; set; }

    private void Start()
    {
        modelsPath = @"character\";
        Gender = true;
        Change = false;
        Loaded = false;
    }

    private void FixedUpdate()
    {
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        Animator animator = GetComponentInChildren<Animator>();
        if (Loaded)
        {
            if (Change)
            {
                Resources.UnloadUnusedAssets();
                GC.Collect();
                helper.ChangeGeosets(activeGeosets);
                helper.LoadTextures(textures);
                for (int i = 0; i < Model.Skin.Textures.Length; i++)
                {
                    SetMaterial(renderer, i);
                }
                int index = Array.FindIndex(worgen.Options, o => o.Name == "Face" && o.Form == 7);
                animator.SetInteger("Face", worgen.Choices[index][worgen.Customization[index]].Bone);
                Change = false;
            }
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

    private void AnimateTextures(SkinnedMeshRenderer renderer, int i)
    {
        if (renderer.materials[Model.Skin.Textures[i].Id].shader != hiddenMaterial.shader)
        {
            int index = Model.TextureAnimationsLookup[Model.Skin.Textures[i].TextureAnimation];
            string texture = "_Texture1";
            if (index >= 0)
            {
                Vector2 offset = renderer.materials[Model.Skin.Textures[i].Id].GetTextureOffset(texture);
                offset = AnimateTexture(index, offset);
                renderer.materials[Model.Skin.Textures[i].Id].SetTextureOffset(texture, offset);
            }
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

    private Vector2 AnimateTexture(int index, Vector2 offset)
    {
        TextureAnimation animation = Model.TextureAnimations[index];
        if (time[index] >= animation.Translation.Timestamps[0][frame[index] + 1] / 1000f)
        {
            frame[index]++;
            if (frame[index] == animation.Translation.Timestamps[0].Length - 1)
            {
                frame[index] = 0;
                time[index] = 0f;
            }
        }
        float timestamp = (animation.Translation.Timestamps[0][frame[index] + 1] - animation.Translation.Timestamps[0][frame[index]]) / 1000f;
        offset.x += (animation.Translation.Values[0][frame[index]].X - animation.Translation.Values[0][frame[index] + 1].X) / timestamp * Time.deltaTime;
        offset.x = offset.x > 1 ? offset.x - 1 : offset.x;
        offset.x = offset.x < -1 ? offset.x + 1 : offset.x;
        offset.y += (animation.Translation.Values[0][frame[index]].Y - animation.Translation.Values[0][frame[index] + 1].Y) / timestamp * Time.deltaTime;
        offset.y = offset.y > 1 ? offset.y - 1 : offset.y;
        offset.y = offset.y < -1 ? offset.y + 1 : offset.y;
        return offset;
    }

    private void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        if (activeGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id))
        {
            Material material = Resources.Load<Material>(@"Materials\" + Model.Skin.Textures[i].Shader);
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

    private BlendMode SrcBlend(short value)
    {
        BlendMode blend = BlendMode.One;
        switch (value)
        {
            case 0:
            case 1:
            case 7:
                blend = BlendMode.One;
                break;
            case 2:
            case 4:
                blend = BlendMode.SrcAlpha;
                break;
            case 3:
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
        material.SetTexture("_Texture1", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture]]);
        if (Model.Skin.Textures[i].TextureCount > 1)
        {
            material.SetTexture("_Texture2", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 1]]);
        }
        if (helper.Emission == null)
        {
            material.SetTexture("_Emission", Texture2D.blackTexture);
        }
        else if (Model.Textures[Model.TextureLookup[Model.Skin.Textures[i].Texture]].Type == 1)
        {
            material.SetTexture("_Emission", helper.Emission);
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

    private void LoadColors()
    {
        colors = new Color[Model.Colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(Model.Colors[i].R / 255f, Model.Colors[i].G / 255f, Model.Colors[i].B / 255f, 1.0f);
        }
    }

    public void UnloadModel()
    {
        DestroyImmediate(mesh);
        Loaded = false;
        if (loadBinaries != null)
        {
            loadBinaries.Abort();
        }
    }

    private IEnumerator LoadPrefab(string modelfile, CASCHandler casc)
    {
        bool done = false;
        DestroyImmediate(mesh);
        GameObject prefab = Resources.Load<GameObject>(modelsPath + RacePath + modelfile + "_prefab");
        mesh = Instantiate(prefab, gameObject.transform);
        yield return null;
        M2Model m2 = GetComponentInChildren<M2Model>();
        byte[] data = m2.data.bytes;
        byte[] skin = m2.skin.bytes;
        byte[] skel = m2.skel.bytes;
        loadBinaries = new Thread(() => { Model = m2.LoadModel(data, skin, skel); done = true; });
        loadBinaries.Start();
        yield return null;
        activeGeosets = new List<int> { 0, 401, 501, 1301, 2001, 2201, 2301, 3401 };
        while (loadBinaries.IsAlive)
        {
            yield return null;
        }
        if (done)
        {
            LoadColors();
            yield return null;
            textures = new Texture2D[Model.Textures.Length];
            if (worgen.Gender)
            {
                helper = new GilneanMale(Model, worgen, casc);
            }
            else
            {
                helper = new GilneanFemale(Model, worgen, casc);
            }
            yield return null;
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
            yield return null;
        }
        gameObject.SetActive(false);
    }

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

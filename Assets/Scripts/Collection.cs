using BLPLib;
using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class Collection : MonoBehaviour
{
    public Material hiddenMaterial;
    public Character character;

    private GameObject mesh;
    private Color[] colors;
    private Texture2D[] textures;
    private float[] time;
    private int[] frame;
    //Reference to loaded casc data
    private CASCHandler casc;
    //Image converter for loading textures
    private System.Drawing.ImageConverter converter;
    private Thread loadBinaries;

    public M2 Model { get; protected set; }

    public List<int> ActiveGeosets { get; set; }
    public bool Change { get; set; }
    public bool Loaded { get; set; }
    public string Path { get; set; }
    public int Texture { get; set; }

    private void Start()
    {
        Change = false;
        Loaded = false;
    }

    private void FixedUpdate()
    {
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (Loaded)
        {
            if (Change)
            {
                Resources.UnloadUnusedAssets();
                GC.Collect();
                for (int i = 0; i < Model.Skin.Textures.Length; i++)
                {
                    SetMaterial(renderer, i);
                }
                Change = false;
            }
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
        else
        {
            Change = false;
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
        if (ActiveGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id))
        {
            Material material = Resources.Load<Material>(@"Materials\" + Model.Skin.Textures[i].Shader);
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

    //Create texture from BLP file
    protected Texture2D TextureFromBLP(int file)
    {
        BLP blp = new BLP(casc.OpenFile(file));
        System.Drawing.Bitmap image = blp.GetImage();
        Texture2D texture = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, true);
        texture.LoadImage((byte[])converter.ConvertTo(image, typeof(byte[])));
        texture.alphaIsTransparency = true;
        return texture;
    }

    private int LoadTexture(M2Texture texture, int i)
    {
        int file = -1;
        int index;
        switch (texture.Type)
        {
            case 0:
                file = Model.TextureIDs[i];
                break;
            case 2:
                file = Texture;
                break;
            case 8:
                index = Array.FindIndex(character.Options, o => o.Name == "Paint");
                file = character.Choices[index][character.Customization[index]].Textures[0].Texture1;
                break;
        }
        return file;
    }

    public void LoadTextures()
    {
        for (int i = 0; i < textures.Length; i++)
        {
            int file = LoadTexture(Model.Textures[i], i);
            if (file == -1)
            {
                textures[i] = new Texture2D(200, 200);
            }
            else
            {
                Texture2D texture = TextureFromBLP(file);
                textures[i] = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
                textures[i].SetPixels32(texture.GetPixels32());
                textures[i].alphaIsTransparency = true;
                textures[i].wrapMode = TextureWrapMode.Repeat;
                textures[i].Apply();
            }
        }
    }

    public void UnloadModel()
    {
        DestroyImmediate(mesh);
        Loaded = false;
    }

    public IEnumerator LoadModel(string collectionfile, CASCHandler casc)
    {
        bool done = false;
        converter = new System.Drawing.ImageConverter();
        this.casc = casc;
        GameObject prefab = Resources.Load<GameObject>(Path + collectionfile);
        mesh = Instantiate(prefab, gameObject.transform);
        yield return null;
        M2Model m2 = GetComponentInChildren<M2Model>();
        byte[] data = m2.data.bytes;
        byte[] skin = m2.skin.bytes;
        byte[] skel = m2.skel.bytes;
        loadBinaries = new Thread(() => { Model = m2.LoadModel(data, skin, skel); done = true; });
        loadBinaries.Start();
        yield return null;
        ActiveGeosets = new List<int>();
        while (loadBinaries.IsAlive)
        {
            yield return null;
        }
        if (done)
        {
            LoadColors();
            yield return null;
            textures = new Texture2D[Model.Textures.Length];
            SkinnedMeshRenderer renderer = mesh.GetComponentInChildren<SkinnedMeshRenderer>();
            SkinnedMeshRenderer renderer2 = character.GetComponentInChildren<SkinnedMeshRenderer>();
            Dictionary<int, int> boneMap = new Dictionary<int, int>();
            for (int i = 0, j = 0; i < Model.Skeleton.Bones.Length; i++, j++)
            {
                if (Model.Skeleton.Bones[i].Name == character.Model.Skeleton.Bones[j].Name)
                {
                    boneMap.Add(i, j);
                }
                else
                {
                    i--;
                }
                yield return null;
            }
            Transform[] bones = new Transform[boneMap.Count];
            Matrix4x4[] bind = new Matrix4x4[boneMap.Count];
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i] = renderer2.bones[boneMap[i]];
                bind[i] = renderer2.sharedMesh.bindposes[boneMap[i]];
                yield return null;
            }
            renderer.bones = bones;
            renderer.rootBone = renderer2.rootBone;
            renderer.sharedMesh.bindposes = bind;
            LoadTextures();
            yield return null;
            time = new float[Model.TextureAnimations.Length];
            frame = new int[Model.TextureAnimations.Length];
            for (int i = 0; i < time.Length; i++)
            {
                time[i] = 0f;
                frame[i] = 0;
                yield return null;
            }
            Loaded = !loadBinaries.IsAlive;
            yield return null;
        }
    }
}

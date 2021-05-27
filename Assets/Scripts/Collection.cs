using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

//Class to render any collection system models
public class Collection : ModelRenderer
{
    //Reference to the character wearing that collection
    public Character character;

    //List of geosets that are enabled for loading
    public List<int> ActiveGeosets { get; set; }
    //Path where the model is located
    public string Path { get; set; }
    //Changable texture from database
    public int Texture { get; set; }

    private void Start()
    {
        //Set the model to not loaded at start
        Change = false;
        Loaded = false;
    }

    private void FixedUpdate()
    {
        if (Loaded)
        {
            //Render the model
            if (Change)
            {
                Resources.UnloadUnusedAssets();
                GC.Collect();
                LoadTextures();
                for (int i = 0; i < Model.Skin.Textures.Length; i++)
                {
                    SetMaterial(renderer, i);
                }
                Change = false;
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
        else
        {
            Change = false;
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

    //Setup all the material properties
    protected override void SetTexture(Material material, int i)
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

    //Load specific texture
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

    //Load and prepare all model textures
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

    //Unload the model
    public void UnloadModel()
    {
        DestroyImmediate(mesh);
        Loaded = false;
        if (loadBinaries != null)
        {
            loadBinaries.Abort();
        }
    }

    //Load the model
    public IEnumerator LoadModel(string collectionfile, CASCHandler casc)
    {
        UnloadModel();
        bool done = false;
        converter = new System.Drawing.ImageConverter();
        this.casc = casc;
        GameObject prefab = Resources.Load<GameObject>($"{Path}{collectionfile}");
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
            renderer = mesh.GetComponentInChildren<SkinnedMeshRenderer>();
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
            Change = true;
        }
    }
}

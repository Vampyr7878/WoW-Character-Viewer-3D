using CASCLib;
using M2Lib;
using SkelLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using WoW;

// Class to render any collection system models
public class Collection : ModelRenderer
{
    // Reference to the character wearing that collection
    public Character character;

    // List of geosets that are enabled for loading
    public List<int> ActiveGeosets { get; set; }
    // Changable texture from database
    public int Texture { get; set; }
    // File id of the model
    public int File { get; private set; }

    private void Start()
    {
        // Set the model to not loaded at start
        Change = false;
        Loaded = false;
    }

    private void FixedUpdate()
    {
        if (Loaded)
        {
            // Render the model
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
            // Animate textures
            if (character.Loaded)
            {
                for (int i = 0; i < Model.Skin.Textures.Length; i++)
                {
                    AnimateTextures(renderer, i);
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
        }
        else
        {
            Change = false;
        }
    }

    // Set material with proper shader
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
            Debug.Log(Model.Skin.Textures[i].Shader);
            SetTexture(renderer.materials[Model.Skin.Textures[i].Id], i);
        }
        else
        {
            renderer.materials[Model.Skin.Textures[i].Id] = new Material(hiddenMaterial.shader);
            renderer.materials[Model.Skin.Textures[i].Id].shader = hiddenMaterial.shader;
            renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(hiddenMaterial);
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
        material.SetInt("_SrcColorBlend", (int)SrcColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstColorBlend", (int)DstColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_SrcAlphaBlend", (int)SrcAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstAlphaBlend", (int)DstAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetFloat("_AlphaCut", Model.Materials[Model.Skin.Textures[i].Material].Blend == 1 ? 0.5f : 0f);
        Color color = Color.white;
        if (Model.Skin.Textures[i].Color != -1)
        {
            color = colors[Model.Skin.Textures[i].Color];
        }
        CullMode cull = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x04) != 0 ? CullMode.Off : CullMode.Back;
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
    private int LoadTexture(M2Texture texture, int i, out WoWHelper.LayeredTexture layered)
    {
        int? file = null;
        int index, index2;
        layered = WoWHelper.LayeredTexture.None;
        Debug.Log($"Texture: {texture.Type}");
        switch (texture.Type)
        {
            case 0:
                file = Model.TextureIDs[i];
                break;
            case 1:
                index = character.helper.GetSkinColorIndex();
                file = character.Options[index].Choices[character.Customization[index]].Textures.First(t => t.Target == 1).ID;
                layered = WoWHelper.LayeredTexture.Skin;
                break;
            case 2:
                file = Texture;
                break;
            case 6:
                index = character.helper.GetHairColorIndex();
                file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                break;
            case 8:
                index = character.helper.GetArmorColorIndex();
                file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                break;
            case 9:
                index = character.helper.GetOrnamentColorIndex();
                file = character.Options[index].Choices[character.Customization[index]].Textures.
                    FirstOrDefault(t => t.Target == (character.ModelID == 89 ? 10 : (character.Race == WoWHelper.Race.Earthen ? 16: 11)))?.ID;
                break;
            case 20:
                index = character.helper.GetJewelryColorIndex();
                if (character.Options[index].Choices[character.Customization[index]].Textures.Length > 0)
                {
                    file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                }
                break;
            case 25:
                index = character.helper.GetArmorColorIndex();
                index2 = character.helper.GetArmorStyleIndex();
                file = character.Options[index].Choices[character.Customization[index]].Textures.
                    First(t => t.Related == character.Options[index2].Choices[character.Customization[index2]].ID).ID;
                break;
        }
        return file == null ? -1 : file.Value;
    }

    // Load and prepare all model textures
    public void LoadTextures()
    {
        for (int i = 0; i < textures.Length; i++)
        {
            int file = LoadTexture(Model.Textures[i], i, out WoWHelper.LayeredTexture layered);
            if (file <= 0)
            {
                textures[i] = new Texture2D(200, 200);
            }
            else
            {
                Texture2D texture = TextureFromBLP(file);
                textures[i] = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
                textures[i].SetPixels32(texture.GetPixels32());
                textures[i].wrapModeU = (Model.Textures[i].Flags & 1) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                textures[i].wrapModeV = (Model.Textures[i].Flags & 2) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                textures[i].Apply();
                switch (layered)
                {
                    case WoWHelper.LayeredTexture.Skin:
                        character.helper.LayeredTexture(textures[i]);
                        break;
                }
            }
        }
    }

    // Unload the model
    public void UnloadModel()
    {
        DestroyImmediate(mesh);
        Loaded = false;
        if (loadBinaries != null)
        {
            loadBinaries.Abort();
        }
    }

    // Find matching bones in character model
    private void MatchBones(GameObject modelRenderer, Bone[] parentBones)
    {
        renderer = mesh.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer renderer2 = modelRenderer.GetComponentInChildren<SkinnedMeshRenderer>();
        Dictionary<int, int> boneMap = new Dictionary<int, int>();
        for (int i = 0, j = 0; i < Model.Skeleton.Bones.Length; i++, j++)
        {
            if (Model.Skeleton.Bones[i].Name == parentBones[j].Name)
            {
                boneMap.Add(i, j);
            }
            else
            {
                i--;
            }
        }
        Transform[] bones = new Transform[boneMap.Count];
        Matrix4x4[] bind = new Matrix4x4[boneMap.Count];
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i] = renderer2.bones[boneMap[i]];
            bind[i] = renderer2.sharedMesh.bindposes[boneMap[i]];
        }
        renderer.bones = bones;
        renderer.rootBone = renderer2.rootBone;
        renderer.sharedMesh.bindposes = bind;
    }

    // Load the model from assets
#if UNITY_EDITOR
    public IEnumerator LoadModel(string collectionfile, GameObject parent, Bone[] parentBones, Dictionary<int, string> listFile, string dataPath)
#else
    public IEnumerator LoadModel(string collectionfile, GameObject parent, Bone[] parentBones, CASCHandler casc)
#endif
    {
        UnloadModel();
        bool done = false;
        converter = new System.Drawing.ImageConverter();
#if UNITY_EDITOR
        this.listFile = listFile;
        this.dataPath = dataPath;
#else
        this.casc = casc;
#endif
        GameObject prefab = Resources.Load<GameObject>(collectionfile);
        mesh = Instantiate(prefab, gameObject.transform);
        yield return null;
        M2Model m2 = GetComponentInChildren<M2Model>();
        byte[] data = m2.data.bytes;
        byte[] skin = m2.skin.bytes;
        byte[] skel = m2.skel == null ? null : m2.skel.bytes;
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
            MatchBones(parent, parentBones);
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
            Loaded = !loadBinaries.IsAlive;
            Change = false;
        }
    }

    // Load the model from CASC
#if UNITY_EDITOR
    public IEnumerator LoadModel(int file, int texture, GameObject parent, Bone[] parentBones, Dictionary<int, string> listFile, string dataPath)
    {
        File = file;
        if (file != 0)
        {
            Texture = texture;
            this.listFile = listFile;
            this.dataPath = dataPath;
            converter = new System.Drawing.ImageConverter();
            byte[] bytes;
            yield return null;
            using (BinaryReader reader = new BinaryReader(System.IO.File.Open($@"{dataPath}\{listFile[file]}", FileMode.Open)))
            {
                bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            }
            yield return null;
            Model = new M2();
            Model.LoadFile(bytes);
            yield return null;
            if (Model.SkelFileID != 0)
            {
                using (BinaryReader reader = new BinaryReader(System.IO.File.Open($@"{dataPath}\{listFile[Model.SkelFileID]}", FileMode.Open)))
                {
                    bytes = reader.ReadBytes((int)reader.BaseStream.Length);
                }
            }
            yield return null;
            Model.Skeleton.LoadFile(bytes, Model.SkelFileID);
            yield return null;
            using (BinaryReader reader = new BinaryReader(System.IO.File.Open($@"{dataPath}\{listFile[Model.SkinFileID]}", FileMode.Open)))
            {
                bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            }
#else
    public IEnumerator LoadModel(int file, int texture, GameObject parent, Bone[] parentBones, CASCHandler casc)
        {
        File = file;
        if (file != 0)
        {
            Texture = texture;
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
#endif
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
            MatchBones(parent, parentBones);
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
            Loaded = true;
            Change = false;
        }
    }
}
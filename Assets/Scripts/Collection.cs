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
using WoW;

// Class to render any collection system models
public class Collection : ModelRenderer
{
    // Reference to the character wearing that collection
    public Character character;

    // Swappable textures for blindfolds
    private Dictionary<int, int> dHBlindfolds;

    // List of geosets that are enabled for loading
    public HashSet<int> ActiveGeosets
    {
        get { return activeGeosets; }
        set { activeGeosets = value; }
    }
    // Changable texture from database
    public int Texture { get; set; }
    // File id of the model
    public int File { get; set; }

    private void Start()
    {
        // Set the model to not loaded at start
        Change = false;
        Loaded = false;
        dHBlindfolds = new()
        {
            { 7, 1306893 },
            { 8, 1277004 },
            { 19, 1306891 },
            { 20, 1314796 },
            { 57, 1306891 },
            { 58, 1314796 }
        };
    }

    private void FixedUpdate()
    {
        if (Loaded)
        {
            // Render the model
            if (Change)
            {
                try
                {
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    LoadTextures();
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
        if (activeGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id))
        {
            string materialName = $@"Materials\{Model.Skin.Textures[i].Shader}\{Model.Skin.Textures[i].Shader}" +
                $"{IsUnlit(Model.Materials[Model.Skin.Textures[i].Material].Flags)}{IsTwoSided(Model.Materials[Model.Skin.Textures[i].Material].Flags)}" +
                $"{IsDepth(Model.Materials[Model.Skin.Textures[i].Material].Flags)}{BlendMode(Model.Materials[Model.Skin.Textures[i].Material].Blend)}";
            var material = Resources.Load<Material>(materialName);
            if (material == null)
            {
                Debug.LogError($"Shader:{Model.Skin.Textures[i].Shader}\tFlags:{Model.Materials[Model.Skin.Textures[i].Material].Flags}" +
                    $"\tBlending:{Model.Materials[Model.Skin.Textures[i].Material].Blend}");
            }
            renderer.materials[i] = new Material(material.shader);
            renderer.materials[i].shader = material.shader;
            renderer.materials[i].CopyPropertiesFromMaterial(material);
            SetTexture(renderer.materials[i], i);
            Debug.Log($"Shader:{Model.Skin.Textures[i].Shader}\tFlags:{Model.Materials[Model.Skin.Textures[i].Material].Flags}" +
                    $"\tBlending:{Model.Materials[Model.Skin.Textures[i].Material].Blend}");
        }
        else
        {
            renderer.materials[i] = new Material(hiddenMaterial.shader);
            renderer.materials[i].shader = hiddenMaterial.shader;
            renderer.materials[i].CopyPropertiesFromMaterial(hiddenMaterial);
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
        var color = Color.white;
        if (Model.Skin.Textures[i].Color != -1)
        {
            color = colors[Model.Skin.Textures[i].Color];
        }
        if (Model.Transparencies[Model.TransparencyLookup[Model.Skin.Textures[i].Transparency]].Transparency.Values[0].Length > 0)
        {
            color.a *= Model.Transparencies[Model.TransparencyLookup[Model.Skin.Textures[i].Transparency]].Transparency.Values[0][0];
        }
        material.SetColor("_Color", color);
        material.renderQueue += Model.Skin.Textures[i].Priority;
    }

    // Get target id for Ornaments for specific race
    private int GetOrnamentTarget()
    {
        int target = character.Race switch
        {
            WoWHelper.Race.Dracthyr => character.ModelID == 89 ? 10 : 11,
            WoWHelper.Race.Earthen => 16,
            WoWHelper.Race.Haranir => 15,
            _ => 0
        };
        return target;
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
                if (mesh.name.Contains("demonhunter") && dHBlindfolds.ContainsKey(character.ModelID)
                    && dHBlindfolds[character.ModelID] == Model.TextureIDs[i])
                {
                    index = Array.FindIndex(character.Options, o => o.Name == "Blindfold" && o.Model == character.ModelID);
                    index2 = Array.FindIndex(character.Options, o => o.Name == "Eye Color" && o.Model == character.ModelID);
                    file = character.Options[index].Choices[character.Customization[index]].Textures.
                        FirstOrDefault(t => t.Related == character.Options[index2].Choices[character.Customization[index2]].ID)?.ID;
                }
                file = file == null ? Model.TextureIDs[i]: file;
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
                layered = WoWHelper.LayeredTexture.Hair;
                break;
            case 8:
                index = character.Race == WoWHelper.Race.Dracthyr ? character.helper.GetArmorColorIndex(): character.helper.GetSkinExtraIndex();
                file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                break;
            case 9:
                index = character.helper.GetOrnamentColorIndex();
                file = character.Options[index].Choices[character.Customization[index]].Textures.
                    FirstOrDefault(t => t.Target == GetOrnamentTarget())?.ID;
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
                if (character.Race == WoWHelper.Race.Dracthyr)
                {
                    index2 = character.helper.GetArmorStyleIndex();
                    file = character.Options[index].Choices[character.Customization[index]].Textures.
                        First(t => t.Related == character.Options[index2].Choices[character.Customization[index2]].ID).ID;
                }
                else
                {
                    file = character.Options[index].Choices[character.Customization[index]].Textures[0].ID;
                }
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
                var texture = TextureFromBLP(file);
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
                    case WoWHelper.LayeredTexture.Hair:
                        character.helper.LayeredHair(textures[i]);
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
        loadBinaries?.Abort();
    }

    // Find matching bones in character model
    private void MatchBones(GameObject modelRenderer, Bone[] parentBones)
    {
        renderer = mesh.GetComponentInChildren<SkinnedMeshRenderer>();
        var renderer2 = modelRenderer.GetComponentInChildren<SkinnedMeshRenderer>();
        Dictionary<int, int> boneMap = new();
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
        var bones = new Transform[boneMap.Count];
        var bind = new Matrix4x4[boneMap.Count];
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
#if UNITY_EDITOR
        this.listFile = listFile;
        this.dataPath = dataPath;
#else
        this.casc = casc;
#endif
        var prefab = Resources.Load<GameObject>(collectionfile);
        mesh = Instantiate(prefab, gameObject.transform);
        yield return null;
        var m2 = GetComponentInChildren<M2Model>();
        byte[] data = m2.data.bytes;
        byte[] skin = m2.skin.bytes;
        byte[] skel = m2.skel == null ? null : m2.skel.bytes;
        loadBinaries = new Thread(() => { Model = m2.LoadModel(data, skin, skel); done = true; });
        loadBinaries.Start();
        yield return null;
        activeGeosets = new();
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
            colorTime = new float[Model.Colors.Length];
            colorFrame = new int[Model.Colors.Length];
            transparencyTime = new float[Model.Colors.Length];
            transparencyFrame = new int[Model.Colors.Length];
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
#else
    public IEnumerator LoadModel(int file, int texture, GameObject parent, Bone[] parentBones, CASCHandler casc)
#endif
    {
        File = file;
        if (file != 0)
        {
            Texture = texture;
#if UNITY_EDITOR
            this.listFile = listFile;
            this.dataPath = dataPath;
#else
            this.casc = casc;
#endif
            byte[] bytes;
            yield return null;
#if UNITY_EDITOR
            using (BinaryReader reader = new(System.IO.File.Open($@"{dataPath}\{listFile[file]}", FileMode.Open)))
#else
            using (BinaryReader reader = new(casc.OpenFile(file)))
#endif
            {
                bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            }
            yield return null;
            Model = new M2();
            Model.LoadFile(bytes);
            yield return null;
            if (Model.SkelFileID != 0)
            {
#if UNITY_EDITOR
                using BinaryReader reader = new(System.IO.File.Open($@"{dataPath}\{listFile[Model.SkelFileID]}", FileMode.Open));
#else
                using BinaryReader reader = new(casc.OpenFile(Model.SkelFileID));
#endif
                bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            }
            yield return null;
            Model.Skeleton.LoadFile(bytes, Model.SkelFileID);
            yield return null;
#if UNITY_EDITOR
            using (BinaryReader reader = new(System.IO.File.Open($@"{dataPath}\{listFile[Model.SkinFileID]}", FileMode.Open)))
#else
            using (BinaryReader reader = new(casc.OpenFile(Model.SkinFileID)))
#endif
            {
                bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            }
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
            var bones = GetComponentInChildren<SkinnedMeshRenderer>().bones;
            if (Model.Particles.Length > 0)
            {
                var particles = new GameObject[Model.Particles.Length];
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i] = WoWHelper.ParticleEffect(this, Model.Particles[i]);
                    particles[i].transform.parent = bones[Model.Particles[i].Bone];
                    particles[i].transform.localPosition = Vector3.zero;
                    particles[i].name = $"Particle{i}";
                    yield return null;
                }
            }
            MatchBones(parent, parentBones);
            textureTime = new float[Model.TextureAnimations.Length];
            textureFrame = new int[Model.TextureAnimations.Length];
            colorTime = new float[Model.Colors.Length];
            colorFrame = new int[Model.Colors.Length];
            transparencyTime = new float[Model.Colors.Length];
            transparencyFrame = new int[Model.Colors.Length];
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
            Change = true;
        }
    }
}
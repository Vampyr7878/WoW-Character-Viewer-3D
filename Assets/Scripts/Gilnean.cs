using CASCLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using WoW;
using WoW.Characters;

//Class to handle Gilnean models
public class Gilnean : ModelRenderer
{
    //Reference to the main character model
    public Character worgen;

    //Reference to the laoded character helper
    private CharacterHelper helper;
    //List of geosets that are enabled for loading
    private List<int> activeGeosets;

    //Path from where the model is loaded
    public string RacePath { get; set; }
    //Character's gender
    public bool Gender { get; set; }

    private void Start()
    {
        //Initialize character
        modelsPath = @"character\";
        Gender = true;
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
                CheckHair();
                helper.ChangeGeosets(activeGeosets);
                EquipArmor();
                helper.LoadTextures(textures);
                for (int i = 0; i < Model.Skin.Textures.Length; i++)
                {
                    SetMaterial(renderer, i);
                }
                int index = Array.FindIndex(worgen.Options, o => o.Name == "Face" && o.Form == 7);
                animator.SetInteger("Face", worgen.Choices[index][worgen.Customization[index]].Bone);
                Change = false;
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
    }

    //Set material with proper shader
    protected override void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        if (activeGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id))
        {
            Material material = Resources.Load<Material>($@"Materials\{Model.Skin.Textures[i].Shader}");
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
        material.SetFloat("_AlphaCut", Model.Materials[Model.Skin.Textures[i].Material].Blend == 1 ? 0.1f : 0f);
        if (Model.Skin.Textures[i].Color != -1)
        {
            material.SetColor("_Color", colors[Model.Skin.Textures[i].Color]);
        }
        CullMode cull = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x04) != 0 ? CullMode.Off : CullMode.Front;
        material.SetInt("_Cull", (int)cull);
        float depth = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x10) != 0 ? 0f : 1f;
        material.SetFloat("_DepthTest", depth);
    }

    //Check if hair needs to be hidden
    private void CheckHair()
    {
        helper.HideHair = false;
        if (worgen.Items[0] == null)
        {
            return;
        }
        foreach (int helmet in worgen.Items[0].GilneanHelmet)
        {
            helper.HideHair = helmet == 0;
            if (helper.HideHair)
            {
                break;
            }
        }
    }

    private void EquipArmor()
    {
        EquipHead();
        EquipShoulder();
        EquipBack();
        EquipShirt();
        EquipWrist();
        EquipLegs();
        EquipChest();
        EquipTabard();
        EquipHands();
        EquipWaist();
        EquipFeet();
        EquipRightHand();
        EquipLeftHand();
    }

    //Load head slot item models and geosets
    private void EquipHead()
    {
        ItemObject helm = GameObject.Find("helm").GetComponent<ItemObject>();
        if (worgen.Items[0] == null)
        {
            helm.UnloadModel();
            worgen.collections[0].UnloadModel();
            return;
        }
        int model = 0;
        if (worgen.Items[0].LeftModel > 0)
        {
            model = worgen.Items[0].GetRaceSpecificModel(worgen.Items[0].LeftModel, 23, Gender, worgen.Class);
        }
        if (helm.File != model || helm.MainTexture != worgen.Items[0].LeftTexture)
        {
            helm.UnloadModel();
            if (worgen.Items[0].LeftModel > 0)
            {
                StartCoroutine(helm.LoadModel(model, worgen.Items[0].LeftTexture, casc));
                helm.ParticleColors = worgen.Items[0].ParticleColors;
                helm.Change = true;
            }
        }
        if (worgen.Items[0].RightModel > 0)
        {
            model = worgen.Items[0].GetRaceSpecificModel(worgen.Items[0].RightModel, 23, Gender, worgen.Class);
            worgen.collections[0].ActiveGeosets = new List<int>();
            worgen.collections[0].ActiveGeosets.Add(2701);
        }
        if (worgen.collections[0].File != model || worgen.collections[0].Texture != worgen.Items[0].RightTexture)
        {
            worgen.collections[0].UnloadModel();
            if (worgen.Items[0].RightModel > 0)
            {
                StartCoroutine(worgen.collections[0].LoadModel(model, worgen.Items[0].RightTexture, casc));
            }
        }
        foreach (int helmet in worgen.Items[0].GilneanHelmet)
        {
            if (helmet == 0)
            {
                continue;
            }
            else if (helmet == 7)
            {
                activeGeosets.RemoveAll(x => x > 699 && x < 800);
                activeGeosets.Add(701);
            }
            else if (helmet == 31)
            {
                continue;
            }
            else
            {
                activeGeosets.RemoveAll(x => x > helmet * 100 - 1 && x < (1 + helmet) * 100);
                activeGeosets.Add(helmet * 100);
            }
        }
    }
    
    //Load shoulder slot item model
    private void EquipShoulder()
    {
        ItemObject left = GameObject.Find("left shoulder").GetComponent<ItemObject>();
        ItemObject right = GameObject.Find("right shoulder").GetComponent<ItemObject>();
        if (worgen.Items[1] == null)
        {
            left.UnloadModel();
            right.UnloadModel();
            return;
        }
        int model = 0;
        if (worgen.Items[1].LeftModel > 0)
        {
            model = worgen.Items[1].GetSideSpecificModel(worgen.Items[1].LeftModel, false, worgen.Class);
        }
        if (left.File != model || left.MainTexture != worgen.Items[1].LeftTexture)
        {
            left.UnloadModel();
            if (worgen.Items[1].LeftModel > 0)
            {
                StartCoroutine(left.LoadModel(model, worgen.Items[1].LeftTexture, casc));
                left.ParticleColors = worgen.Items[1].ParticleColors;
                left.Change = true;
            }
        }
        if (worgen.Items[1].RightModel > 0)
        {
            model = worgen.Items[1].GetSideSpecificModel(worgen.Items[1].RightModel, true, worgen.Class);
        }
        if (right.File != model || right.MainTexture != worgen.Items[1].RightTexture)
        {
            right.UnloadModel();
            if (worgen.Items[1].RightModel > 0)
            {
                StartCoroutine(right.LoadModel(model, worgen.Items[1].RightTexture, casc));
                right.ParticleColors = worgen.Items[1].ParticleColors;
                right.Change = true;
            }
        }
    }

    //Load back slot item models and geosets
    private void EquipBack()
    {
        ItemObject backpack = GameObject.Find("backpack").GetComponent<ItemObject>();
        activeGeosets.RemoveAll(x => x > 1499 && x < 1600);
        if (worgen.Items[2] == null)
        {
            worgen.collections[1].UnloadModel();
            backpack.UnloadModel();
            activeGeosets.Add(1501);
            return;
        }
        int model = 0;
        if (worgen.Items[2].LeftModel > 0)
        {
            model = worgen.Items[2].GetRaceSpecificModel(worgen.Items[2].LeftModel, 23, Gender, worgen.Class);
            worgen.collections[1].ActiveGeosets = new List<int>();
            worgen.collections[1].ActiveGeosets.Add(1501);
        }
        if (worgen.collections[1].File != model || worgen.collections[1].Texture != worgen.Items[2].LeftTexture)
        {
            worgen.collections[1].UnloadModel();
            if (worgen.Items[2].LeftModel > 0)
            {
                StartCoroutine(worgen.collections[1].LoadModel(model, worgen.Items[2].LeftTexture, casc));
            }
        }
        if (worgen.Items[2].RightModel > 0)
        {
            model = worgen.Items[2].GetModel(worgen.Items[2].RightModel, worgen.Class);
        }
        if (backpack.File != model || backpack.MainTexture != worgen.Items[2].RightTexture)
        {
            backpack.UnloadModel();
            if (worgen.Items[2].RightModel > 0)
            {
                StartCoroutine(backpack.LoadModel(model, worgen.Items[2].RightTexture, casc));
                backpack.ParticleColors = worgen.Items[2].ParticleColors;
                backpack.Change = true;
            }
        }
        activeGeosets.Add(1501 + worgen.Items[2].Geoset1);
    }

    //Load chest slot item models and geosets
    private void EquipChest()
    {
        activeGeosets.RemoveAll(x => x > 799 && x < 900);
        activeGeosets.RemoveAll(x => x > 999 && x < 1100);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        activeGeosets.RemoveAll(x => x > 2199 && x < 2300);
        if (worgen.Items[3] == null)
        {
            worgen.collections[2].UnloadModel();
            activeGeosets.Add(801);
            activeGeosets.Add(1001);
            activeGeosets.Add(1301);
            activeGeosets.Add(2201);
        }
        else
        {
            int model = 0;
            if (worgen.Items[3].LeftModel > 0)
            {
                model = worgen.Items[3].GetRaceSpecificModel(worgen.Items[3].LeftModel, 23, Gender, worgen.Class);
                worgen.collections[2].ActiveGeosets = new List<int>();
                worgen.collections[2].ActiveGeosets.Add(801);
                worgen.collections[2].ActiveGeosets.Add(1001);
                if (worgen.Items[3].Geoset3 == 1)
                {
                    worgen.collections[2].ActiveGeosets.Add(1301);
                }
                worgen.collections[2].ActiveGeosets.Add(2201);
                worgen.collections[2].ActiveGeosets.Add(2801);
            }
            if (worgen.collections[2].File != model || worgen.collections[2].Texture != worgen.Items[3].LeftTexture)
            {
                worgen.collections[2].UnloadModel();
                if (worgen.Items[3].LeftModel > 0)
                {
                    StartCoroutine(worgen.collections[2].LoadModel(model, worgen.Items[3].LeftTexture, casc));
                }
            }
            activeGeosets.Add(801 + worgen.Items[3].Geoset1);
            activeGeosets.Add(1001 + worgen.Items[3].Geoset2);
            activeGeosets.Add(1301 + worgen.Items[3].Geoset3);
            if (worgen.Items[3].Geoset3 == 1)
            {
                activeGeosets.RemoveAll(x => x > 1099 && x < 1200);
                activeGeosets.RemoveAll(x => x > 899 && x < 1000);
                if (worgen.collections[5].Loaded)
                {
                    worgen.collections[5].ActiveGeosets.RemoveAll(x => x > 1099 && x < 1200);
                }
            }
            activeGeosets.Add(2201 + worgen.Items[3].Geoset4);
            if (worgen.Items[3].UpperLeg > 0)
            {
                activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
                if (worgen.collections[5].Loaded)
                {
                    worgen.collections[5].ActiveGeosets.Clear();
                }
            }
        }
        if (worgen.Items[10] != null && worgen.Items[10].Geoset3 == 1)
        {
            activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
            activeGeosets.Add(1302);
        }
        if (activeGeosets.Contains(1104))
        {
            activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        }
    }

    //Load shirt slot item geosets
    private void EquipShirt()
    {
        activeGeosets.RemoveAll(x => x > 799 && x < 900);
        if (worgen.Items[4] == null)
        {
            activeGeosets.Add(801);
            return;
        }
        activeGeosets.Add(801 + worgen.Items[4].Geoset1);
    }

    //Load tabard slot item geosets
    private void EquipTabard()
    {
        activeGeosets.RemoveAll(x => x > 1199 && x < 1300);
        if (worgen.Items[5] == null)
        {
            activeGeosets.Add(1201);
            return;
        }
        if ((worgen.Items[3] != null && worgen.Items[3].Geoset3 == 1) || (worgen.Items[10] != null && worgen.Items[10].Geoset3 == 1))
        {
            activeGeosets.Add(1201);
        }
        else
        {
            activeGeosets.Add(1201 + worgen.Items[5].Geoset1);
        }
    }

    //Load wrist slot item geosets
    private void EquipWrist()
    {
        if (worgen.Items[6] == null)
        {
            return;
        }
        activeGeosets.RemoveAll(x => x > 799 && x < 900);
    }

    //Load hands slot item models and geosets
    private void EquipHands()
    {
        activeGeosets.RemoveAll(x => x > 399 && x < 500);
        if (worgen.Items[8] == null)
        {
            worgen.collections[3].UnloadModel();
            activeGeosets.Add(401);
            return;
        }
        int model = 0;
        if (worgen.Items[8].LeftModel > 0)
        {
            model = worgen.Items[8].GetRaceSpecificModel(worgen.Items[8].LeftModel, 23, Gender, worgen.Class);
            worgen.collections[3].ActiveGeosets = new List<int>();
            if (activeGeosets.Contains(801))
            {
                worgen.collections[3].ActiveGeosets.Add(401);
            }
            worgen.collections[3].ActiveGeosets.Add(2301);
        }
        if (worgen.collections[3].File != model || worgen.collections[3].Texture != worgen.Items[8].LeftTexture)
        {
            worgen.collections[3].UnloadModel();
            if (worgen.Items[8].LeftModel > 0)
            {
                StartCoroutine(worgen.collections[3].LoadModel(model, worgen.Items[8].LeftTexture, casc));
            }
        }
        activeGeosets.Add(401 + worgen.Items[8].Geoset1);
        if (worgen.Items[8].Geoset1 != 0)
        {
            activeGeosets.RemoveAll(x => x > 799 && x < 900);
        }
    }

    //Load waist slot item models and geosets
    private void EquipWaist()
    {
        ItemObject buckle = GameObject.Find("buckle").GetComponent<ItemObject>();
        activeGeosets.RemoveAll(x => x > 1799 && x < 1900);
        if (worgen.Items[9] == null)
        {
            worgen.collections[4].UnloadModel();
            buckle.UnloadModel();
            activeGeosets.Add(1801);
            return;
        }
        int model = 0;
        if (worgen.Items[9].LeftModel > 0)
        {
            model = worgen.Items[9].GetModel(worgen.Items[9].LeftModel, worgen.Class);
        }
        if (buckle.File != model || buckle.MainTexture != worgen.Items[9].LeftTexture)
        {
            buckle.UnloadModel();
            if (worgen.Items[9].LeftModel > 0)
            {
                StartCoroutine(buckle.LoadModel(model, worgen.Items[9].LeftTexture, casc));
                buckle.ParticleColors = worgen.Items[9].ParticleColors;
                buckle.Change = true;
            }
        }
        if (worgen.Items[9].RightModel > 0)
        {
            model = worgen.Items[9].GetRaceSpecificModel(worgen.Items[9].RightModel, 23, Gender, worgen.Class);
            worgen.collections[4].ActiveGeosets = new List<int>();
            worgen.collections[4].ActiveGeosets.Add(1801);
        }
        if (worgen.collections[4].File != model || worgen.collections[4].Texture != worgen.Items[9].RightTexture)
        {
            worgen.collections[4].UnloadModel();
            if (worgen.Items[9].RightModel > 0)
            {
                StartCoroutine(worgen.collections[4].LoadModel(model, worgen.Items[9].RightTexture, casc));
            }
        }
        activeGeosets.Add(1801 + worgen.Items[9].Geoset1);
        if (worgen.Items[9].Geoset1 == 1)
        {
            activeGeosets.RemoveAll(x => x > 999 && x < 1100);
        }
    }

    //Load legs slot item models and geosets
    private void EquipLegs()
    {
        activeGeosets.RemoveAll(x => x > 1099 && x < 1200);
        activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        if (worgen.Items[10] == null)
        {
            worgen.collections[5].UnloadModel();
            activeGeosets.Add(1101);
            activeGeosets.Add(901);
            activeGeosets.Add(1301);
            return;
        }
        int model = 0;
        if (worgen.Items[10].LeftModel > 0)
        {
            model = worgen.Items[10].GetRaceSpecificModel(worgen.Items[10].LeftModel, 23, Gender, worgen.Class);
            worgen.collections[5].ActiveGeosets = new List<int>();
            worgen.collections[5].ActiveGeosets.Add(901);
            worgen.collections[5].ActiveGeosets.Add(1101);
        }
        if (worgen.collections[5].File != model || worgen.collections[5].Texture != worgen.Items[10].LeftTexture)
        {
            worgen.collections[5].UnloadModel();
            if (worgen.Items[10].LeftModel > 0)
            {
                StartCoroutine(worgen.collections[5].LoadModel(model, worgen.Items[10].LeftTexture, casc));
            }
        }
        activeGeosets.Add(1101 + worgen.Items[10].Geoset1);
        activeGeosets.Add(901 + worgen.Items[10].Geoset2);
        if (worgen.Items[10].Geoset1 != 3)
        {
            activeGeosets.Add(1301 + worgen.Items[10].Geoset3);
        }
        if (worgen.Items[10].UpperLeg > 0)
        {
            activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
        }
    }

    //Load feet slot item models and geosets
    private void EquipFeet()
    {
        activeGeosets.RemoveAll(x => x > 499 && x < 600);
        activeGeosets.RemoveAll(x => x > 1999 && x < 2100);
        if (worgen.Items[11] == null)
        {
            worgen.collections[6].UnloadModel();
            if (!activeGeosets.Contains(1302))
            {
                activeGeosets.Add(501);
            }
            activeGeosets.Add(2001);
            return;
        }
        int model = 0;
        if (worgen.Items[11].LeftModel > 0)
        {
            model = worgen.Items[11].GetRaceSpecificModel(worgen.Items[11].LeftModel, 23, Gender, worgen.Class);
            worgen.collections[6].ActiveGeosets = new List<int>();
            if (!activeGeosets.Contains(1302))
            {
                worgen.collections[6].ActiveGeosets.Add(501);
            }
            worgen.collections[6].ActiveGeosets.Add(2001);
        }
        if (worgen.collections[6].File != model || worgen.collections[6].Texture != worgen.Items[11].LeftTexture)
        {
            worgen.collections[6].UnloadModel();
            if (worgen.Items[11].LeftModel > 0)
            {
                StartCoroutine(worgen.collections[6].LoadModel(model, worgen.Items[11].LeftTexture, casc));
            }
        }
        if (!activeGeosets.Contains(1302))
        {
            activeGeosets.Add(501 + worgen.Items[11].Geoset1);
        }
        if (worgen.Items[11].Geoset1 != 0)
        {
            activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        }
        activeGeosets.Add(2002 - worgen.Items[11].Geoset2);
    }

    //Load main hand slot item models and geosets
    private void EquipRightHand()
    {
        ItemObject right = GameObject.Find("right hand").GetComponent<ItemObject>();
        ItemObject book = GameObject.Find("book").GetComponent<ItemObject>();
        right.UnloadModel();
        book.UnloadModel();
        if (worgen.Items[7] != null)
        {
            if (worgen.Items[7].Slot != 15)
            {
                if (worgen.Items[7].LeftModel > 0)
                {
                    int model = worgen.Items[7].GetModel(worgen.Items[7].LeftModel, worgen.Class);
                    StartCoroutine(right.LoadModel(model, worgen.Items[7].LeftTexture, casc));
                    right.ParticleColors = worgen.Items[7].ParticleColors;
                    right.Change = true;
                }
            }
            if (worgen.Items[7].Slot != 15 && worgen.Items[7].Slot != 26)
            {
                if (worgen.Items[7].RightModel > 0)
                {
                    int model = worgen.Items[7].GetModel(worgen.Items[7].RightModel, worgen.Class);
                    StartCoroutine(book.LoadModel(model, worgen.Items[7].RightTexture, casc));
                    book.ParticleColors = worgen.Items[7].ParticleColors;
                    book.Change = true;
                }
            }
        }
    }

    //Load offhand slot item models and geosets
    private void EquipLeftHand()
    {
        ItemObject left = GameObject.Find("left hand").GetComponent<ItemObject>();
        ItemObject shield = GameObject.Find("shield").GetComponent<ItemObject>();
        ItemObject quiver = GameObject.Find("quiver").GetComponent<ItemObject>();
        left.UnloadModel();
        shield.UnloadModel();
        quiver.UnloadModel();
        if (worgen.Items[7] != null)
        {
            if (worgen.Items[7].Slot == 15)
            {
                left.transform.localScale = new Vector3(1f, 1f, 1f);
                if (worgen.Items[7].LeftModel > 0)
                {
                    int model = worgen.Items[7].GetModel(worgen.Items[7].LeftModel, worgen.Class);
                    StartCoroutine(left.LoadModel(model, worgen.Items[7].LeftTexture, casc));
                    left.ParticleColors = worgen.Items[7].ParticleColors;
                    left.Change = true;
                }
            }
            if (worgen.Items[7].Slot == 15 || worgen.Items[7].Slot == 26)
            {
                if (worgen.Items[7].RightModel > 0)
                {
                    int model = worgen.Items[7].GetModel(worgen.Items[7].RightModel, worgen.Class);
                    StartCoroutine(quiver.LoadModel(model, worgen.Items[7].RightTexture, casc));
                    quiver.ParticleColors = worgen.Items[7].ParticleColors;
                    quiver.Change = true;
                }
            }
        }
        if (worgen.Items[12] != null)
        {
            if (worgen.Items[12].Slot == 14)
            {
                if (worgen.Items[12].LeftModel > 0)
                {
                    int model = worgen.Items[12].GetModel(worgen.Items[12].LeftModel, worgen.Class);
                    StartCoroutine(shield.LoadModel(model, worgen.Items[12].LeftTexture, casc));
                    shield.ParticleColors = worgen.Items[12].ParticleColors;
                    shield.Change = true;
                }
            }
            else
            {
                if (worgen.Items[12].LeftModel > 0)
                {
                    left.transform.localScale = new Vector3(1f, 1f, -1f);
                    int model = worgen.Items[12].GetModel(worgen.Items[12].LeftModel, worgen.Class);
                    StartCoroutine(left.LoadModel(model, worgen.Items[12].LeftTexture, casc));
                    left.ParticleColors = worgen.Items[12].ParticleColors;
                    left.Change = true;
                }
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

    //Load and setup model prefab
    private IEnumerator LoadPrefab(string modelfile)
    {
        bool done = false;
        DestroyImmediate(mesh);
        GameObject prefab = Resources.Load<GameObject>($"{modelsPath}{RacePath}{modelfile}_prefab");
        mesh = Instantiate(prefab, gameObject.transform);
        yield return null;
        M2Model m2 = GetComponentInChildren<M2Model>();
        byte[] data = m2.data.bytes;
        byte[] skin = m2.skin.bytes;
        byte[] skel = m2.skel.bytes;
        loadBinaries = new Thread(() => { Model = m2.LoadModel(data, skin, skel); done = true; });
        loadBinaries.Start();
        yield return null;
        activeGeosets = new List<int> { 0, 2301 };
        while (loadBinaries.IsAlive)
        {
            yield return null;
        }
        if (done)
        {
            LoadColors();
            yield return null;
            textures = new Texture2D[Model.Textures.Length];
            if (Gender)
            {
                helper = new GilneanMale(Model, worgen);
            }
            else
            {
                helper = new GilneanFemale(Model, worgen);
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
            renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            animator = GetComponentInChildren<Animator>();
            Change = true;
            Loaded = !loadBinaries.IsAlive;
        }
        gameObject.SetActive(false);
    }

    //Load the model
    public void LoadModel(string modelfile, CASCHandler casc)
    {
        Loaded = false;
        this.casc = casc;
        if (loadBinaries != null)
        {
            loadBinaries.Abort();
        }
        StartCoroutine(LoadPrefab(modelfile));
    }
}

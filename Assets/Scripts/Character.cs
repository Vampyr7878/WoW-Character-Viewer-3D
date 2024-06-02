using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using WoW;
using WoW.Characters;

//Main class to handle character rendering
public class Character : ModelRenderer
{
    //Referene to race specific collection model
    public Collection racial;

    //Reference to the helper class that handles specific race
    private CharacterHelper helper;
    //List of geosets that are enabled for loading
    private List<int> activeGeosets;
    //Reference to the mainm mesh prefab
    private GameObject mainMesh;
    //Reference to the extra mesh prefab
    private GameObject extraMesh;
    //M2 Data for the main mesh
    private M2 mainModel;
    //M2 Data for the extra mesh
    private M2 extraModel;

    //Dropdowns containing customization options
    public List<CustomDropdown> CustomizationDropdowns { get; set; }
    //Toggles containing customization options
    public List<Toggle> CustomizationToggles { get; set; }
    //Path from where the model is loaded
    public string RacePath { get; set; }
    //Character's race
    public int Race { get; set; }
    //Character's class
    public int Class { get; set; }
    //Character's gender
    public bool Gender { get; set; }
    //Curent character form
    public int Form { get; set; }
    //Character's model index
    public int ModelID { get; set; }
    //Currently selected customization category
    public int Category { get; set; }
    //Customization options for current character
    public CustomizationOption[] Options { get; set; }
    //Customization categories for current character
    public CustomizationCategory[] Categories { get; set; }
    //Customization values for each option
    public int[] Customization { get; set; }

    private void Start()
    {
        //Initialize character
        modelsPath = @"character\";
        converter = new System.Drawing.ImageConverter();
        CustomizationDropdowns = new List<CustomDropdown>();
        CustomizationToggles = new List<Toggle>();
        Gender = true;
        Change = false;
        Loaded = false;
    }

    private void FixedUpdate()
    {
        if (Loaded)
        {
            if (Change)
            {
                //Render the model
                try
                {
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    //CheckHair();
                    helper.ChangeGeosets(activeGeosets);
                    EquipArmor();
                    helper.LoadTextures(textures);
                    for (int i = 0; i < Model.Skin.Textures.Length; i++)
                    {
                        SetMaterial(renderer, i);
                    }
                    int index = Array.FindIndex(Options, o => o.Name == "Face");
                    animator.SetInteger("Face", Customization[index] + 1);
                    animator.SetBool("DeathKnight", Class == 6);
                    racial.Change = true;
                    //foreach (Collection collection in collections)
                    //{
                    //    collection.Change = true;
                    //}
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                }
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

    //Mark dropdown options active/inactive depending on if they are available for the first option
    public void ActivateRelatedChoices(int index, int index2)
    {
        int firstID = Options[index].Choices[Customization[index]].ID;
        List<Dropdown.OptionData> options = CustomizationDropdowns[index2].options;
        for (int i = 0; i < CustomizationDropdowns[index2].options.Count; i++)
        {
            if (Options[index2].Choices[((CustomOptionData)options[i]).Index].Textures.FirstOrDefault(t => t.Related == firstID) == null)
            {
                ((CustomOptionData)options[i]).Interactable = false;
            }
            else
            {
                ((CustomOptionData)options[i]).Interactable = true;
            }
        }
    }

    //Mark dropdown options active/inactive depending on if they are available for the first option
    public void ActivateUsingRequirmenets(int index, int[] requirements)
    {
        List<Dropdown.OptionData> options = CustomizationDropdowns[index].options;
        for (int i = 0; i < CustomizationDropdowns[index].options.Count; i++)
        {
            if (requirements.Contains(Options[index].Choices[((CustomOptionData)options[i]).Index].Requirement))
            {
                ((CustomOptionData)options[i]).Interactable = true;
            }
            else
            {
                ((CustomOptionData)options[i]).Interactable = false;
            }
        }
    }

    //Mark dropdown options active/inactive depending on if they are available for the first option
    public void ActivateUsingIds(int index, int[] list)
    {
        List<Dropdown.OptionData> options = CustomizationDropdowns[index].options;
        for (int i = 0; i < CustomizationDropdowns[index].options.Count; i++)
        {
            if (list.Contains(Options[index].Choices[((CustomOptionData)options[i]).Index].ID))
            {
                ((CustomOptionData)options[i]).Interactable = true;
            }
            else
            {
                ((CustomOptionData)options[i]).Interactable = false;
            }
        }
    }

    //Load only options in the dropdown that are meant to be shown
    public void ChangeDropdownOptions(int index)
    {
        if (Options[index].Type == 1)
        {
            bool on = CustomizationToggles[index].isOn;
            CustomizationToggles[index].transform.parent.gameObject.SetActive(Options[index].Choices.Length == 2 && Options[index].Category == Category);
            CustomizationToggles[index].isOn = on && Options[index].Choices.Length == 2;
            return;
        }
        Sprite sprite = Resources.LoadAll<Sprite>("Icons/charactercreate").Single(s => s.name == "color1");
        int value = CustomizationDropdowns[index].GetValue(), j = 0;
        CustomizationDropdowns[index].options.Clear();
        foreach (var choice in Options[index].Choices)
        {
            string name;
            if (choice.Color1 != Color.black)
            {
                name = $"{j + 1}:";
            }
            else if (string.IsNullOrEmpty(choice.Name))
            {
                name = $"{j + 1}";
            }
            else
            {
                name = $"{j + 1}: {choice.Name}";
            }
            CustomOptionData data = new(name, choice.Color1, choice.Color2, sprite, j);
            CustomizationDropdowns[index].options.Add(data);
            j++;
        }
        value = value >= CustomizationDropdowns[index].options.Count ? CustomizationDropdowns[index].options.Count - 1 : value;
        CustomizationDropdowns[index].SetValue(value);
        CustomizationDropdowns[index].RefreshShownValue();
        CustomizationDropdowns[index].transform.parent.gameObject.SetActive(CustomizationDropdowns[index].options.Count > 1 && Options[index].Category == Category);
    }

    public void ChangeForm(int form)
    {
        Form = form;
        helper.ChangeForm();
    }

    //Set material with proper shader
    protected override void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        if (Race == 9 && Model.Skin.Textures[i].Id == 1)
        {
            renderer.materials[Model.Skin.Textures[i].Id] = new Material(hiddenMaterial.shader);
            renderer.materials[Model.Skin.Textures[i].Id].shader = hiddenMaterial.shader;
            renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(hiddenMaterial);
        }
        else if (activeGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id))
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
            Debug.Log(Model.Skin.Textures[i].Shader);
        }
        else
        {
            renderer.materials[Model.Skin.Textures[i].Id] = new Material(hiddenMaterial.shader);
            renderer.materials[Model.Skin.Textures[i].Id].shader = hiddenMaterial.shader;
            renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(hiddenMaterial);
        }
    }

    //Load all the equipped items
    private void EquipArmor()
    {
        //EquipHead();
        //EquipShoulder();
        //EquipBack();
        EquipShirt();
        EquipWrist();
        EquipLegs();
        EquipChest();
        EquipTabard();
        EquipHands();
        EquipWaist();
        EquipFeet();
        //EquipRightHand();
        //EquipLeftHand();
    }

    //Load back slot item models and geosets
    private void EquipBack()
    {
        //ItemObject backpack = GameObject.Find("backpack").GetComponent<ItemObject>();
        activeGeosets.RemoveAll(x => x > 1499 && x < 1600);
        //if (Items[2] == null)
        //{
        //    collections[1].UnloadModel();
        //    backpack.UnloadModel();
        activeGeosets.Add(1501);
        //    return;
        //}
        //int model = 0;
        //if (Items[2].LeftModel > 0)
        //{
        //    model = Items[2].GetRaceSpecificModel(Items[2].LeftModel, Race, Gender, Class);
        //    collections[1].ActiveGeosets = new List<int>();
        //    collections[1].ActiveGeosets.Add(1501);
        //}
        //if (collections[1].File != model || collections[1].Texture != Items[2].LeftTexture2)
        //{
        //    collections[1].UnloadModel();
        //    if (Items[2].LeftModel > 0)
        //    {
        //        StartCoroutine(collections[1].LoadModel(model, Items[2].LeftTexture2, casc));
        //    }
        //}
        //if (Items[2].RightModel > 0)
        //{
        //    model = Items[2].GetModel(Items[2].RightModel, Class);
        //}
        //if (backpack.File != model || backpack.Texture2 != Items[2].RightTexture2)
        //{
        //    backpack.UnloadModel();
        //    if (Items[2].RightModel > 0)
        //    {
        //        StartCoroutine(backpack.LoadModel(model, Items[2].RightTexture2, Items[2].RightTexture3, Items[2].RightTexture4, casc));
        //        backpack.ParticleColors = Items[2].ParticleColors;
        //        backpack.Change = true;
        //    }
        //}
        //int geoset = Items[2].Geoset1;
        //if (Race == 5)
        //{
        //    int index = Array.FindIndex(Options, o => o.Name == "Skin Type");
        //    if (Customization[index] > 0)
        //    {
        //        geoset += 9;
        //    }
        //}
        //activeGeosets.Add(1501 + geoset);
    }

    //Load chest slot item models and geosets
    private void EquipChest()
    {
        activeGeosets.RemoveAll(x => x > 799 && x < 900);
        activeGeosets.RemoveAll(x => x > 999 && x < 1100);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        activeGeosets.RemoveAll(x => x > 2199 && x < 2300);
        //if (Items[3] == null)
        //{
        //    collections[2].UnloadModel();
        activeGeosets.Add(801);
        activeGeosets.Add(1001);
        activeGeosets.Add(1301);
        activeGeosets.Add(2201);
        //}
        //else
        //{
        //    int model = 0;
        //    if (Items[3].LeftModel > 0)
        //    {
        //        model = Items[3].GetRaceSpecificModel(Items[3].LeftModel, Race, Gender, Class);
        //        collections[2].ActiveGeosets = new List<int>();
        //        collections[2].ActiveGeosets.Add(801);
        //        collections[2].ActiveGeosets.Add(1001);
        //        if (Items[3].Geoset3 == 1)
        //        {
        //            collections[2].ActiveGeosets.Add(1301);
        //        }
        //        collections[2].ActiveGeosets.Add(2201);
        //        collections[2].ActiveGeosets.Add(2801);
        //    }
        //    if (collections[2].File != model || collections[2].Texture != Items[3].LeftTexture2)
        //    {
        //        collections[2].UnloadModel();
        //        if (Items[3].LeftModel > 0)
        //        {
        //            StartCoroutine(collections[2].LoadModel(model, Items[3].LeftTexture2, casc));
        //        }
        //    }
        //    activeGeosets.Add(801 + Items[3].Geoset1);
        //    activeGeosets.Add(1001 + Items[3].Geoset2);
        //    activeGeosets.Add(1301 + Items[3].Geoset3);
        //    if (Items[3].Geoset3 == 1)
        //    {
        //        activeGeosets.RemoveAll(x => x > 1099 && x < 1200);
        //        activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        //        if (racial.Loaded)
        //        {
        //            racial.ActiveGeosets.RemoveAll(x => x > 2999 && x < 3100);
        //        }
        //        if (collections[5].Loaded)
        //        {
        //            collections[5].ActiveGeosets.RemoveAll(x => x > 1099 && x < 1200);
        //        }
        //    }
        //    activeGeosets.Add(2201 + Items[3].Geoset4);
        //    if (Items[3].UpperLeg > 0)
        //    {
        //        activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //        if (demonHunter.Loaded)
        //        {
        //            demonHunter.ActiveGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //        }
        //        if (collections[5].Loaded)
        //        {
        //            collections[5].ActiveGeosets.Clear();
        //        }
        //    }
        //}
        //if (Items[10] != null && Items[10].Geoset3 == 1)
        //{
        //    activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        //    activeGeosets.Add(1302);
        //    if (racial.Loaded)
        //    {
        //        racial.ActiveGeosets.RemoveAll(x => x > 2999 && x < 3100);
        //    }
        //}
        //if (activeGeosets.Contains(1104))
        //{
        //    activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        //}
    }

    //Load shirt slot item geosets
    private void EquipShirt()
    {
        activeGeosets.RemoveAll(x => x > 799 && x < 900);
        //if (Items[4] == null)
        //{
        activeGeosets.Add(801);
        //    return;
        //}
        //activeGeosets.Add(801 + Items[4].Geoset1);
    }

    //Load tabard slot item geosets
    private void EquipTabard()
    {
        activeGeosets.RemoveAll(x => x > 1199 && x < 1300);
        //if (Items[5] == null)
        //{
        activeGeosets.Add(1201);
        //    return;
        //}
        //if ((Items[3] != null && Items[3].Geoset3 == 1) || (Items[10] != null && Items[10].Geoset3 == 1))
        //{
        //    activeGeosets.Add(1201);
        //}
        //else
        //{
        //    activeGeosets.Add(1201 + Items[5].Geoset1);
        //}
    }

    //Load wrist slot item geosets
    private void EquipWrist()
    {
        //if (Items[6] == null)
        //{
        //    return;
        //}
        //activeGeosets.RemoveAll(x => x > 799 && x < 900);
    }

    //Load hands slot item models and geosets
    private void EquipHands()
    {
        activeGeosets.RemoveAll(x => x > 399 && x < 500);
        //if (Items[8] == null)
        //{
        //    collections[3].UnloadModel();
        activeGeosets.Add(401);
        //    return;
        //}
        //int model = 0;
        //if (Items[8].LeftModel > 0)
        //{
        //    model = Items[8].GetRaceSpecificModel(Items[8].LeftModel, Race, Gender, Class);
        //    collections[3].ActiveGeosets = new List<int>();
        //    if (Race != 37)
        //    {
        //        if (activeGeosets.Contains(801))
        //        {
        //            collections[3].ActiveGeosets.Add(401);
        //        }
        //        collections[3].ActiveGeosets.Add(2301);
        //    }
        //}
        //if (collections[3].File != model || collections[3].Texture != Items[8].LeftTexture2)
        //{
        //    collections[3].UnloadModel();
        //    if (Items[8].LeftModel > 0)
        //    {
        //        StartCoroutine(collections[3].LoadModel(model, Items[8].LeftTexture2, casc));
        //    }
        //}
        //activeGeosets.Add(401 + Items[8].Geoset1);
        //if (Items[8].Geoset1 != 0)
        //{
        //    activeGeosets.RemoveAll(x => x > 799 && x < 900);
        //}
    }

    //Load waist slot item models and geosets
    private void EquipWaist()
    {
        //ItemObject buckle = GameObject.Find("buckle").GetComponent<ItemObject>();
        activeGeosets.RemoveAll(x => x > 1799 && x < 1900);
        //if (Items[9] == null)
        //{
        //    collections[4].UnloadModel();
        //    buckle.UnloadModel();
        activeGeosets.Add(1801);
        //    return;
        //}
        //int model = 0;
        //if (Items[9].LeftModel > 0)
        //{
        //    model = Items[9].GetModel(Items[9].LeftModel, Class);
        //}
        //if (buckle.File != model || buckle.Texture2 != Items[9].LeftTexture2)
        //{
        //    buckle.UnloadModel();
        //    if (Items[9].LeftModel > 0)
        //    {
        //        StartCoroutine(buckle.LoadModel(model, Items[9].LeftTexture2, Items[9].LeftTexture3, Items[9].LeftTexture4, casc));
        //        buckle.ParticleColors = Items[9].ParticleColors;
        //        buckle.Change = true;
        //    }
        //}
        //if (Items[9].RightModel > 0)
        //{
        //    model = Items[9].GetRaceSpecificModel(Items[9].RightModel, Race, Gender, Class);
        //    collections[4].ActiveGeosets = new List<int>();
        //    collections[4].ActiveGeosets.Add(1801);
        //}
        //if (collections[4].File != model || collections[4].Texture != Items[9].RightTexture2)
        //{
        //    collections[4].UnloadModel();
        //    if (Items[9].RightModel > 0)
        //    {
        //        StartCoroutine(collections[4].LoadModel(model, Items[9].RightTexture2, casc));
        //    }
        //}
        //activeGeosets.Add(1801 + Items[9].Geoset1);
        //if (Items[9].Geoset1 == 1)
        //{
        //    activeGeosets.RemoveAll(x => x > 999 && x < 1100);
        //}
    }

    //Load legs slot item models and geosets
    private void EquipLegs()
    {
        activeGeosets.RemoveAll(x => x > 1099 && x < 1200);
        activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        //if (Items[10] == null)
        //{
        //    collections[5].UnloadModel();
        activeGeosets.Add(1101);
        activeGeosets.Add(901);
        activeGeosets.Add(1301);
        //    return;
        //}
        //int model = 0;
        //if (Items[10].LeftModel > 0)
        //{
        //    model = Items[10].GetRaceSpecificModel(Items[10].LeftModel, Race, Gender, Class);
        //    collections[5].ActiveGeosets = new List<int>();
        //    collections[5].ActiveGeosets.Add(901);
        //    if (Race != 37)
        //    {
        //        collections[5].ActiveGeosets.Add(1101);
        //    }
        //}
        //if (collections[5].File != model || collections[5].Texture != Items[10].LeftTexture2)
        //{
        //    collections[5].UnloadModel();
        //    if (Items[10].LeftModel > 0)
        //    {
        //        StartCoroutine(collections[5].LoadModel(model, Items[10].LeftTexture2, casc));
        //    }
        //}
        //activeGeosets.Add(1101 + Items[10].Geoset1);
        //activeGeosets.Add(901 + Items[10].Geoset2);
        //if (Items[10].Geoset1 != 3)
        //{
        //    activeGeosets.Add(1301 + Items[10].Geoset3);
        //}
        //if (Items[10].UpperLeg > 0)
        //{
        //    activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //    if (demonHunter.Loaded)
        //    {
        //        demonHunter.ActiveGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //    }
        //}
    }

    //Load feet slot item models and geosets
    private void EquipFeet()
    {
        activeGeosets.RemoveAll(x => x > 499 && x < 600);
        activeGeosets.RemoveAll(x => x > 1999 && x < 2100);
        //if (Items[11] == null)
        //{
        //    collections[6].UnloadModel();
        if (!activeGeosets.Contains(1302))
        {
            activeGeosets.Add(501);
        }
        activeGeosets.Add(2001);
        //    return;
        //}
        //int model = 0;
        //if (Items[11].LeftModel > 0)
        //{
        //    model = Items[11].GetRaceSpecificModel(Items[11].LeftModel, Race, Gender, Class);
        //    collections[6].ActiveGeosets = new List<int>();
        //    if (Race != 37)
        //    {
        //        if (!activeGeosets.Contains(1302))
        //        {
        //            collections[6].ActiveGeosets.Add(501);
        //        }
        //        collections[6].ActiveGeosets.Add(2001);
        //    }
        //}
        //if (collections[6].File != model || collections[6].Texture != Items[11].LeftTexture2)
        //{
        //    collections[6].UnloadModel();
        //    if (Items[11].LeftModel > 0)
        //    {
        //        StartCoroutine(collections[6].LoadModel(model, Items[11].LeftTexture2, casc));
        //    }
        //}
        //if (!activeGeosets.Contains(1302))
        //{
        //    activeGeosets.Add(501 + Items[11].Geoset1);
        //}
        //if (Items[11].Geoset1 != 0)
        //{
        //    activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        //}
        //if (!(Items[11].Geoset2 == -1 && Items[11].Foot == 0))
        //{
        //    activeGeosets.Add(2002 - Items[11].Geoset2);
        //}
    }

    //Setup all the material properties
    protected override void SetTexture(Material material, int i)
    {
        material.SetTexture("_Texture1", textures[Model.TextureLookup[Model.Skin.Textures[i].Texture]]);
        if (Model.Skin.Textures[i].TextureCount > 1)
        {
            if (material.shader.name == "Custom/32783")
            {
                textures[Model.TextureLookup[Model.Skin.Textures[i].Texture + 1]].wrapMode = TextureWrapMode.Clamp;
            }
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
        //if (Race == 34 && material.shader.name == "Custom/16401")
        //{
        //    material.SetInt("_SrcColorBlend", (int)BlendMode.SrcColor);
        //    material.SetInt("_DstColorBlend", (int)BlendMode.One);
        //}
        //else
        //{
        material.SetInt("_SrcColorBlend", (int)SrcColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstColorBlend", (int)DstColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_SrcAlphaBlend", (int)SrcAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstAlphaBlend", (int)DstAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        //}
        material.SetFloat("_AlphaCut", Model.Materials[Model.Skin.Textures[i].Material].Blend == 1 ? 0.5f : 0f);
        if (Model.Skin.Textures[i].Color != -1)
        {
            material.SetColor("_Color", colors[Model.Skin.Textures[i].Color]);
        }
        CullMode cull = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x04) != 0 ? CullMode.Off : CullMode.Back;
        material.SetInt("_Cull", (int)cull);
        float depth = (Model.Materials[Model.Skin.Textures[i].Material].Flags & 0x10) != 0 ? 0f : 1f;
        material.SetFloat("_DepthTest", depth);
    }

    //Initialize helper class for chosen race/gender
    public void InitializeHelper()
    {
        if (Gender)
        {
            switch (Race)
            {
                case 1:
                    helper = new HumanMale(Model, this);
                    break;
                case 2:
                    helper = new OrcMale(Model, this);
                    break;
                case 3:
                    helper = new DwarfMale(Model, this);
                    break;
                case 4:
                    helper = new NightElfMale(Model, this);
                    break;
                case 5:
                    helper = new UndeadMale(Model, this);
                    break;
                case 6:
                    helper = new TaurenMale(Model, this);
                    break;
                case 7:
                    helper = new GnomeMale(Model, this);
                    break;
                case 8:
                    helper = new TrollMale(Model, this);
                    break;
                case 9:
                    helper = new GoblinMale(Model, this);
                    break;
                case 10:
                    helper = new BloodElfMale(Model, this);
                    break;
                case 11:
                    helper = new DraeneiMale(Model, this);
                    break;
                case 22:
                    helper = new WorgenMale(Model, this);
                    break;
                case 24:
                    helper = new PandarenMale(Model, this);
                    break;
                case 27:
                    helper = new NightborneMale(Model, this);
                    break;
                case 28:
                    helper = new HighmountainMale(Model, this);
                    break;
                case 29:
                    helper = new VoidElfMale(Model, this);
                    break;
                case 30:
                    helper = new LightforgedMale(Model, this);
                    break;
                case 31:
                    helper = new ZandalariMale(Model, this);
                    break;
                case 32:
                    helper = new KulTiranMale(Model, this);
                    break;
                case 34:
                    helper = new DarkIronMale(Model, this);
                    break;
                case 35:
                    helper = new VulperaMale(Model, this);
                    break;
                case 36:
                    helper = new MagharMale(Model, this);
                    break;
                case 37:
                    helper = new MechagnomeMale(Model, this);
                    break;
            }
        }
        else
        {
            switch (Race)
            {
                case 1:
                    helper = new HumanFemale(Model, this);
                    break;
                case 2:
                    helper = new OrcFemale(Model, this);
                    break;
                case 3:
                    helper = new DwarfFemale(Model, this);
                    break;
                case 4:
                    helper = new NightElfFemale(Model, this);
                    break;
                case 5:
                    helper = new UndeadFemale(Model, this);
                    break;
                case 6:
                    helper = new TaurenFemale(Model, this);
                    break;
                case 7:
                    helper = new GnomeFemale(Model, this);
                    break;
                case 8:
                    helper = new TrollFemale(Model, this);
                    break;
                case 9:
                    helper = new GoblinFemale(Model, this);
                    break;
                case 10:
                    helper = new BloodElfFemale(Model, this);
                    break;
                case 11:
                    helper = new DraeneiFemale(Model, this);
                    break;
                case 22:
                    helper = new WorgenFemale(Model, this);
                    break;
                case 24:
                    helper = new PandarenFemale(Model, this);
                    break;
                case 27:
                    helper = new NightborneFemale(Model, this);
                    break;
                case 28:
                    helper = new HighmountainFemale(Model, this);
                    break;
                case 29:
                    helper = new VoidElfFemale(Model, this);
                    break;
                case 30:
                    helper = new LightforgedFemale(Model, this);
                    break;
                case 31:
                    helper = new ZandalariFemale(Model, this);
                    break;
                case 32:
                    helper = new KulTiranFemale(Model, this);
                    break;
                case 34:
                    helper = new DarkIronFemale(Model, this);
                    break;
                case 35:
                    helper = new VulperaFemale(Model, this);
                    break;
                case 36:
                    helper = new MagharFemale(Model, this);
                    break;
                case 37:
                    helper = new MechagnomeFemale(Model, this);
                    break;
            }
        }
    }

    //Enable main mesh
    public void ActivateMainMesh()
    {
        mainMesh.SetActive(true);
        extraMesh.SetActive(false);
        mesh = mainMesh;
        Model = mainModel;
        helper.Model = mainModel;
        textures = new Texture2D[Model.Textures.Length];
        renderer = mesh.GetComponent<SkinnedMeshRenderer>();
    }

    //Enable extra mesh
    public void ActivateExtranMesh()
    {
        mainMesh.SetActive(false);
        extraMesh.SetActive(true);
        mesh = extraMesh;
        Model = extraModel;
        helper.Model = extraModel;
        textures = new Texture2D[Model.Textures.Length];
        renderer = mesh.GetComponent<SkinnedMeshRenderer>();
    }

    //Load and setup main and extra model prefabs
    private IEnumerator LoadPrefab(string modelfile, string extrafile, string collectionfile)
    {
        bool done = false;
        DestroyImmediate(mainMesh);
        DestroyImmediate(extraMesh);
        mainModel = null;
        extraModel = null;
        GameObject prefab = Resources.Load<GameObject>($"{modelsPath}{RacePath}{modelfile}_prefab");
        mainMesh = Instantiate(prefab, gameObject.transform);
        mesh = mainMesh;
        if (extrafile != null)
        {
            string path = $"{modelsPath}{RacePath}{extrafile}_prefab";
            path = Path.GetRelativePath(modelsPath, path);
            prefab = Resources.Load<GameObject>($"{modelsPath}{path}");
            extraMesh = Instantiate(prefab, gameObject.transform);
            extraMesh.SetActive(false);
        }
        yield return null;
        M2Model[] m2 = GetComponentsInChildren<M2Model>(true);
        byte[] data = m2[0].data.bytes;
        byte[] skin = m2[0].skin.bytes;
        byte[] skel = m2[0].skel == null ? null : m2[0].skel.bytes;
        loadBinaries = new Thread(() => { mainModel = m2[0].LoadModel(data, skin, skel); done = true; });
        loadBinaries.Start();
        yield return null;
        activeGeosets = new List<int> { 0 };
        while (loadBinaries.IsAlive)
        {
            yield return null;
        }
        Model = mainModel;
        if (extrafile != null)
        {
            data = m2[1].data.bytes;
            skin = m2[1].skin.bytes;
            skel = m2[1].skel == null ? null : m2[1].skel.bytes;
            loadBinaries = new Thread(() => { extraModel = m2[1].LoadModel(data, skin, skel); done = true; });
            loadBinaries.Start();
            yield return null;
            activeGeosets = new List<int> { 0 };
            while (loadBinaries.IsAlive)
            {
                yield return null;
            }
        }
        if (collectionfile != null)
        {
#if UNITY_EDITOR
            StartCoroutine(racial.LoadModel($@"{modelsPath}{RacePath}collection\{collectionfile}", listfile, dataPath));
#else
            StartCoroutine(racial.LoadModel($@"{modelsPath}{RacePath}collection\{collectionfile}", casc));
#endif
            yield return null;
            while (!racial.Loaded)
            {
                yield return null;
            }
        }
        else
        {
            racial.UnloadModel();
            yield return null;
        }
        if (done)
        {
            LoadColors();
            yield return null;
            textures = new Texture2D[Model.Textures.Length];
            InitializeHelper();
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
    }

    //Load the model
#if UNITY_EDITOR
    public void LoadModel(string modelfile, string extrafile, string collectionfile, Dictionary<int, string> listfile, string dataPath)
#else
    public void LoadModel(string modelfile, string extrafile, string collectionfile, CASCHandler casc)
#endif
    {
        Loaded = false;
#if UNITY_EDITOR
        this.listfile = listfile;
        this.dataPath = dataPath;
#else
        this.casc = casc;
#endif
        loadBinaries?.Abort();
        StartCoroutine(LoadPrefab(modelfile, extrafile, collectionfile));
    }
}

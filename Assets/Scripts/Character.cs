using Assets.WoW;
using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using WoW;
using WoW.Characters;

// Main class to handle character rendering
public class Character : ModelRenderer
{
    // Referene to race specific collection model
    public Collection racial;
    // Referene to race specific collection model
    public Collection armor;
    // Referene to race specific collection model
    public Collection extraRacial;
    // Referene to customizable creature model
    public Creature creature;
    // Compute shader to handle texture layers
    public ComputeShader layerShader;

    // Reference to the helper class that handles specific race
#if UNITY_EDITOR
    [SerializeReference]
#endif
    public CharacterHelper helper;
    // List of geosets that are enabled for loading
    private List<int> activeGeosets;
    // Reference to the mainm mesh prefab
    private GameObject mainMesh;
    // Reference to the extra mesh prefab
    private GameObject extraMesh;
    // M2 Data for the main mesh
    private M2 mainModel;
    // M2 Data for the extra mesh
    private M2 extraModel;

    // Dropdowns containing customization options
    public List<CustomDropdown> CustomizationDropdowns { get; set; }
    // Toggles containing customization options
    public List<Toggle> CustomizationToggles { get; set; }
    // Buttons containing customization categories
    public List<Button> CustomizationCategories { get; set; }
    //Equipped items
    public ItemInstance[] Items { get; set; }
    // Path from where the model is loaded
    public string RacePath { get; set; }
    // Extra path from where the model is loaded
    public string ExtraPath { get; set; }
    // Character's race
    public WoWHelper.Race Race { get; set; }
    // Character's class
    public WoWHelper.Class Class { get; set; }
    // Character's gender
    public bool Gender { get; set; }
    // Curent character form
    public int Form { get; set; }
    // Character's main form model ID
    public int MainFormID { get; set; }
    // Character's current model ID
    public int ModelID { get; set; }
    // Currently selected customization category
    public int Category { get; set; }
    // Customization options for current character
    public CustomizationOption[] Options { get; set; }
    // Customization categories for current character
    public CustomizationCategory[] Categories { get; set; }
    // Crature forms available to the character
    public CreatureForm[] CreatureForms { get; set; }
    // Customization values for each option
    public int[] Customization { get; set; }

    private void Start()
    {
        // Initialize
        Items = new ItemInstance[14];
        modelsPath = @"character\";
        converter = new System.Drawing.ImageConverter();
        CustomizationDropdowns = new List<CustomDropdown>();
        CustomizationToggles = new List<Toggle>();
        CustomizationCategories = new List<Button>();
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
                // Render the model
                try
                {
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    // CheckHair();
                    if (mesh.activeSelf)
                    {
                        helper.ChangeGeosets(activeGeosets);
                        if (ModelID != 89)
                        {
                            EquipArmor();
                        }
                        helper.LoadTextures(textures);
                        for (int i = 0; i < Model.Skin.Textures.Length; i++)
                        {
                            SetMaterial(renderer, i);
                        }
                        int index = Array.FindIndex(Options, o => o.Name == "Face" && o.Model == ModelID);
                        animator.SetInteger("Face", ChoiceIndex(index) + 1);
                        animator.SetBool("DeathKnight", Class == WoWHelper.Class.DeathKnight);
                    }
                    else
                    {
                        creature.ActivateRelatedOptions();
                        creature.ChangeRelatedOptions();
                        creature.ChangeRelatedOptions();
                        creature.ChangeRelatedOptions();
                        creature.ChangeFullTransformation();
#if UNITY_EDITOR
                        creature.ChangeModel(listFile, dataPath);
#else
                        creature.ChangeModel(casc);
#endif
                        creature.Change = true;
                    }
                    racial.Change = true;
                    armor.Change = true;
                    extraRacial.Change = true;
                    // foreach (Collection collection in collections)
                    // {
                    //     collection.Change = true;
                    // }
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                }
                Change = false;
            }
            // Animate textures
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

    // Get Choice's index
    private int ChoiceIndex(int index)
    {
        return Options[index].Choices.Select((c, i) => new { c, i }).First(x => x.c.Key == Customization[index]).i;
    }

    // Mark dropdown options active/inactive depending on if they are available for the first option
    public void ActivateRelatedChoices(int index, int index2)
    {
        int firstID = Options[index].Choices[Customization[index]].ID;
        List<Dropdown.OptionData> options = CustomizationDropdowns[index2].options;
        for (int i = 0; i < CustomizationDropdowns[index2].options.Count; i++)
        {
            if (Options[index2].Choices[((CustomOptionData)options[i]).ID].Textures.FirstOrDefault(t => t.Related == firstID) == null)
            {
                ((CustomOptionData)options[i]).Interactable = false;
            }
            else
            {
                ((CustomOptionData)options[i]).Interactable = true;
            }
        }
    }

    // Mark dropdown options active/inactive depending on if they are available based on requirements
    public void ActivateUsingRequirmenets(int index, int[] requirements)
    {
        List<Dropdown.OptionData> options = CustomizationDropdowns[index].options;
        for (int i = 0; i < CustomizationDropdowns[index].options.Count; i++)
        {
            if (requirements.Contains(Options[index].Choices[((CustomOptionData)options[i]).ID].Requirement))
            {
                ((CustomOptionData)options[i]).Interactable = true;
            }
            else
            {
                ((CustomOptionData)options[i]).Interactable = false;
            }
        }
    }

    // Mark dropdown options active/inactive depending on if they are on the list
    public void ActivateUsingIds(int index, int[] list)
    {
        List<Dropdown.OptionData> options = CustomizationDropdowns[index].options;
        for (int i = 0; i < CustomizationDropdowns[index].options.Count; i++)
        {
            if (list.Contains(((CustomOptionData)options[i]).ID))
            {
                ((CustomOptionData)options[i]).Interactable = true;
            }
            else
            {
                ((CustomOptionData)options[i]).Interactable = false;
            }
        }
    }

    // Load only options in the dropdown that are meant to be shown
    public void ChangeDropdownOptions(int index)
    {
        if (Options[index].Type == WoWHelper.CustomizationType.Toggle)
        {
            bool on = CustomizationToggles[index].isOn;
            CustomizationToggles[index].transform.parent.gameObject.SetActive(Options[index].Choices.Count == 2 && Options[index].Category == Category);
            CustomizationToggles[index].isOn = on && Options[index].Choices.Count == 2;
            return;
        }
        Sprite sprite = Resources.LoadAll<Sprite>("Icons/charactercreate").Single(s => s.name == "color1");
        int value = CustomizationDropdowns[index].GetValue(), j = 0;
        CustomizationDropdowns[index].options.Clear();
        foreach (var choice in Options[index].Choices)
        {
            string name;
            if (choice.Value.Color1 != Color.black)
            {
                name = $"{j + 1}:";
            }
            else if (string.IsNullOrEmpty(choice.Value.Name))
            {
                name = $"{j + 1}";
            }
            else
            {
                name = $"{j + 1}: {choice.Value.Name}";
            }
            CustomOptionData data = new(name, choice.Value.Color1, choice.Value.Color2, sprite, j, choice.Key);
            CustomizationDropdowns[index].options.Add(data);
            j++;
        }
        value = Options[index].Choices.ContainsKey(value) ? value : Options[index].Choices.First().Key;
        Customization[index] = value;
        CustomizationDropdowns[index].SetValue(value);
        CustomizationDropdowns[index].RefreshShownValue();
        CustomizationDropdowns[index].transform.parent.gameObject.SetActive(CustomizationDropdowns[index].options.Count > 1 && Options[index].Category == Category);
    }

    // Change character's form
    public void ChangeForm(int form)
    {
        Form = form;
        helper.ChangeForm();
    }

    // Set material with proper shader
    protected override void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        if (Race == WoWHelper.Race.Goblin && Model.Skin.Textures[i].Id == 1)
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

    // Load all the equipped items
    private void EquipArmor()
    {
        // EquipHead();
        // EquipShoulder();
        // EquipBack();
        EquipShirt();
        EquipWrist();
        EquipLegs();
        EquipChest();
        EquipTabard();
        EquipHands();
        EquipWaist();
        EquipFeet();
        // EquipRightHand();
        // EquipLeftHand();
    }

    // Load back slot item models and geosets
    private void EquipBack()
    {
        // ItemObject backpack = GameObject.Find("backpack").GetComponent<ItemObject>();
        activeGeosets.RemoveAll(x => x > 1499 && x < 1600);
        // if (Items[2] == null)
        // {
        //     collections[1].UnloadModel();
        //     backpack.UnloadModel();
        activeGeosets.Add(1501);
        //     return;
        // }
        // int model = 0;
        // if (Items[2].LeftModel > 0)
        // {
        //     model = Items[2].GetRaceSpecificModel(Items[2].LeftModel, Race, Gender, Class);
        //     collections[1].ActiveGeosets = new List<int>();
        //     collections[1].ActiveGeosets.Add(1501);
        // }
        // if (collections[1].File != model || collections[1].Texture != Items[2].LeftTexture2)
        // {
        //     collections[1].UnloadModel();
        //     if (Items[2].LeftModel > 0)
        //     {
        //         StartCoroutine(collections[1].LoadModel(model, Items[2].LeftTexture2, casc));
        //     }
        // }
        // if (Items[2].RightModel > 0)
        // {
        //     model = Items[2].GetModel(Items[2].RightModel, Class);
        // }
        // if (backpack.File != model || backpack.Texture2 != Items[2].RightTexture2)
        // {
        //     backpack.UnloadModel();
        //     if (Items[2].RightModel > 0)
        //     {
        //         StartCoroutine(backpack.LoadModel(model, Items[2].RightTexture2, Items[2].RightTexture3, Items[2].RightTexture4, casc));
        //         backpack.ParticleColors = Items[2].ParticleColors;
        //         backpack.Change = true;
        //     }
        // }
        // int geoset = Items[2].Geoset1;
        // if (Race == 5)
        // {
        //     int index = Array.FindIndex(Options, o => o.Name == "Skin Type");
        //     if (Customization[index] > 0)
        //     {
        //         geoset += 9;
        //     }
        // }
        // activeGeosets.Add(1501 + geoset);
    }

    // Load chest slot item models and geosets
    private void EquipChest()
    {
        activeGeosets.RemoveAll(x => x > 799 && x < 900);
        activeGeosets.RemoveAll(x => x > 999 && x < 1100);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        activeGeosets.RemoveAll(x => x > 2199 && x < 2300);
        // if (Items[3] == null)
        // {
        //     collections[2].UnloadModel();
        activeGeosets.Add(801);
        activeGeosets.Add(1001);
        activeGeosets.Add(1301);
        activeGeosets.Add(2201);
        // }
        // else
        // {
        //     int model = 0;
        //     if (Items[3].LeftModel > 0)
        //     {
        //         model = Items[3].GetRaceSpecificModel(Items[3].LeftModel, Race, Gender, Class);
        //         collections[2].ActiveGeosets = new List<int>();
        //         collections[2].ActiveGeosets.Add(801);
        //         collections[2].ActiveGeosets.Add(1001);
        //         if (Items[3].Geoset3 == 1)
        //         {
        //             collections[2].ActiveGeosets.Add(1301);
        //         }
        //         collections[2].ActiveGeosets.Add(2201);
        //         collections[2].ActiveGeosets.Add(2801);
        //     }
        //     if (collections[2].File != model || collections[2].Texture != Items[3].LeftTexture2)
        //     {
        //         collections[2].UnloadModel();
        //         if (Items[3].LeftModel > 0)
        //         {
        //             StartCoroutine(collections[2].LoadModel(model, Items[3].LeftTexture2, casc));
        //         }
        //     }
        //     activeGeosets.Add(801 + Items[3].Geoset1);
        //     activeGeosets.Add(1001 + Items[3].Geoset2);
        //     activeGeosets.Add(1301 + Items[3].Geoset3);
        //     if (Items[3].Geoset3 == 1)
        //     {
        //         activeGeosets.RemoveAll(x => x > 1099 && x < 1200);
        //         activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        //         if (racial.Loaded)
        //         {
        //             racial.ActiveGeosets.RemoveAll(x => x > 2999 && x < 3100);
        //         }
        //         if (collections[5].Loaded)
        //         {
        //             collections[5].ActiveGeosets.RemoveAll(x => x > 1099 && x < 1200);
        //         }
        //     }
        //     activeGeosets.Add(2201 + Items[3].Geoset4);
        //     if (Items[3].UpperLeg > 0)
        //     {
        //         activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //         if (demonHunter.Loaded)
        //         {
        //             demonHunter.ActiveGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //         }
        //         if (collections[5].Loaded)
        //         {
        //             collections[5].ActiveGeosets.Clear();
        //         }
        //     }
        // }
        // if (Items[10] != null && Items[10].Geoset3 == 1)
        // {
        //     activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        //     activeGeosets.Add(1302);
        //     if (racial.Loaded)
        //     {
        //         racial.ActiveGeosets.RemoveAll(x => x > 2999 && x < 3100);
        //     }
        // }
        // if (activeGeosets.Contains(1104))
        // {
        //     activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        // }
    }

    // Load shirt slot item geosets
    private void EquipShirt()
    {
        activeGeosets.RemoveAll(x => x > 799 && x < 900);
        // if (Items[4] == null)
        // {
        activeGeosets.Add(801);
        //     return;
        // }
        // activeGeosets.Add(801 + Items[4].Geoset1);
    }

    // Load tabard slot item geosets
    private void EquipTabard()
    {
        activeGeosets.RemoveAll(x => x > 1199 && x < 1300);
        // if (Items[5] == null)
        // {
        activeGeosets.Add(1201);
        //     return;
        // }
        // if ((Items[3] != null && Items[3].Geoset3 == 1) || (Items[10] != null && Items[10].Geoset3 == 1))
        // {
        //     activeGeosets.Add(1201);
        // }
        // else
        // {
        //     activeGeosets.Add(1201 + Items[5].Geoset1);
        // }
    }

    // Load wrist slot item geosets
    private void EquipWrist()
    {
        // if (Items[6] == null)
        // {
        //     return;
        // }
        // activeGeosets.RemoveAll(x => x > 799 && x < 900);
    }

    // Load hands slot item models and geosets
    private void EquipHands()
    {
        activeGeosets.RemoveAll(x => x > 399 && x < 500);
        // if (Items[8] == null)
        // {
        //     collections[3].UnloadModel();
        activeGeosets.Add(401);
        //     return;
        // }
        // int model = 0;
        // if (Items[8].LeftModel > 0)
        // {
        //     model = Items[8].GetRaceSpecificModel(Items[8].LeftModel, Race, Gender, Class);
        //     collections[3].ActiveGeosets = new List<int>();
        //     if (Race != 37)
        //     {
        //         if (activeGeosets.Contains(801))
        //         {
        //             collections[3].ActiveGeosets.Add(401);
        //         }
        //         collections[3].ActiveGeosets.Add(2301);
        //     }
        // }
        // if (collections[3].File != model || collections[3].Texture != Items[8].LeftTexture2)
        // {
        //     collections[3].UnloadModel();
        //     if (Items[8].LeftModel > 0)
        //     {
        //         StartCoroutine(collections[3].LoadModel(model, Items[8].LeftTexture2, casc));
        //     }
        // }
        // activeGeosets.Add(401 + Items[8].Geoset1);
        // if (Items[8].Geoset1 != 0)
        // {
        //     activeGeosets.RemoveAll(x => x > 799 && x < 900);
        // }
    }

    // Load waist slot item models and geosets
    private void EquipWaist()
    {
        // ItemObject buckle = GameObject.Find("buckle").GetComponent<ItemObject>();
        activeGeosets.RemoveAll(x => x > 1799 && x < 1900);
        // if (Items[9] == null)
        // {
        //     collections[4].UnloadModel();
        //     buckle.UnloadModel();
        activeGeosets.Add(1801);
        //     return;
        // }
        // int model = 0;
        // if (Items[9].LeftModel > 0)
        // {
        //     model = Items[9].GetModel(Items[9].LeftModel, Class);
        // }
        // if (buckle.File != model || buckle.Texture2 != Items[9].LeftTexture2)
        // {
        //     buckle.UnloadModel();
        //     if (Items[9].LeftModel > 0)
        //     {
        //         StartCoroutine(buckle.LoadModel(model, Items[9].LeftTexture2, Items[9].LeftTexture3, Items[9].LeftTexture4, casc));
        //         buckle.ParticleColors = Items[9].ParticleColors;
        //         buckle.Change = true;
        //     }
        // }
        // if (Items[9].RightModel > 0)
        // {
        //     model = Items[9].GetRaceSpecificModel(Items[9].RightModel, Race, Gender, Class);
        //     collections[4].ActiveGeosets = new List<int>();
        //     collections[4].ActiveGeosets.Add(1801);
        // }
        // if (collections[4].File != model || collections[4].Texture != Items[9].RightTexture2)
        // {
        //     collections[4].UnloadModel();
        //     if (Items[9].RightModel > 0)
        //     {
        //         StartCoroutine(collections[4].LoadModel(model, Items[9].RightTexture2, casc));
        //     }
        // }
        // activeGeosets.Add(1801 + Items[9].Geoset1);
        // if (Items[9].Geoset1 == 1)
        // {
        //     activeGeosets.RemoveAll(x => x > 999 && x < 1100);
        // }
    }

    // Load legs slot item models and geosets
    private void EquipLegs()
    {
        activeGeosets.RemoveAll(x => x > 1099 && x < 1200);
        activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        // if (Items[10] == null)
        // {
        //     collections[5].UnloadModel();
        activeGeosets.Add(1101);
        activeGeosets.Add(901);
        activeGeosets.Add(1301);
        //     return;
        // }
        // int model = 0;
        // if (Items[10].LeftModel > 0)
        // {
        //     model = Items[10].GetRaceSpecificModel(Items[10].LeftModel, Race, Gender, Class);
        //     collections[5].ActiveGeosets = new List<int>();
        //     collections[5].ActiveGeosets.Add(901);
        //     if (Race != 37)
        //     {
        //         collections[5].ActiveGeosets.Add(1101);
        //     }
        // }
        // if (collections[5].File != model || collections[5].Texture != Items[10].LeftTexture2)
        // {
        //     collections[5].UnloadModel();
        //     if (Items[10].LeftModel > 0)
        //     {
        //         StartCoroutine(collections[5].LoadModel(model, Items[10].LeftTexture2, casc));
        //     }
        // }
        // activeGeosets.Add(1101 + Items[10].Geoset1);
        // activeGeosets.Add(901 + Items[10].Geoset2);
        // if (Items[10].Geoset1 != 3)
        // {
        //     activeGeosets.Add(1301 + Items[10].Geoset3);
        // }
        // if (Items[10].UpperLeg > 0)
        // {
        //     activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //     if (demonHunter.Loaded)
        //     {
        //         demonHunter.ActiveGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //     }
        // }
    }

    // Load feet slot item models and geosets
    private void EquipFeet()
    {
        activeGeosets.RemoveAll(x => x > 499 && x < 600);
        activeGeosets.RemoveAll(x => x > 1999 && x < 2100);
        // if (Items[11] == null)
        // {
        //     collections[6].UnloadModel();
        if (!activeGeosets.Contains(1302))
        {
            activeGeosets.Add(501);
        }
        activeGeosets.Add(2001);
        //     return;
        // }
        // int model = 0;
        // if (Items[11].LeftModel > 0)
        // {
        //     model = Items[11].GetRaceSpecificModel(Items[11].LeftModel, Race, Gender, Class);
        //     collections[6].ActiveGeosets = new List<int>();
        //     if (Race != 37)
        //     {
        //         if (!activeGeosets.Contains(1302))
        //         {
        //             collections[6].ActiveGeosets.Add(501);
        //         }
        //         collections[6].ActiveGeosets.Add(2001);
        //     }
        // }
        // if (collections[6].File != model || collections[6].Texture != Items[11].LeftTexture2)
        // {
        //     collections[6].UnloadModel();
        //     if (Items[11].LeftModel > 0)
        //     {
        //         StartCoroutine(collections[6].LoadModel(model, Items[11].LeftTexture2, casc));
        //     }
        // }
        // if (!activeGeosets.Contains(1302))
        // {
        //     activeGeosets.Add(501 + Items[11].Geoset1);
        // }
        // if (Items[11].Geoset1 != 0)
        // {
        //     activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        // }
        // if (!(Items[11].Geoset2 == -1 && Items[11].Foot == 0))
        // {
        //     activeGeosets.Add(2002 - Items[11].Geoset2);
        // }
    }

    // Setup all the material properties
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
        // if (Race == 34 && material.shader.name == "Custom/16401")
        // {
        //     material.SetInt("_SrcColorBlend", (int)BlendMode.SrcColor);
        //     material.SetInt("_DstColorBlend", (int)BlendMode.One);
        // }
        // else
        // {
        material.SetInt("_SrcColorBlend", (int)SrcColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstColorBlend", (int)DstColorBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_SrcAlphaBlend", (int)SrcAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        material.SetInt("_DstAlphaBlend", (int)DstAlphaBlend(Model.Materials[Model.Skin.Textures[i].Material].Blend));
        // }
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

    // Initialize helper class for chosen race/gender
    public void InitializeHelper()
    {
        switch (Race)
        {
            case WoWHelper.Race.Human:
                helper = Gender ? new HumanMale(Model, this, layerShader) : new HumanFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Orc:
                helper = Gender ? new OrcMale(Model, this, layerShader) : new OrcFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Dwarf:
                helper = Gender ? new DwarfMale(Model, this, layerShader) : new DwarfFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.NightElf:
                helper = Gender ? new NightElfMale(Model, this, layerShader) : new NightElfFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Undead:
                helper = Gender ? new UndeadMale(Model, this, layerShader) : new UndeadFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Tauren:
                helper = Gender ? new TaurenMale(Model, this, layerShader) : new TaurenFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Gnome:
                helper = Gender ? new GnomeMale(Model, this, layerShader) : new GnomeFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Troll:
                helper = Gender ? new TrollMale(Model, this, layerShader) : new TrollFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Goblin:
                helper = Gender ? new GoblinMale(Model, this, layerShader) : new GoblinFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.BloodElf:
                helper = Gender ? new BloodElfMale(Model, this, layerShader) : new BloodElfFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Draenei:
                helper = Gender ? new DraeneiMale(Model, this, layerShader) : new DraeneiFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Worgen:
                helper = Gender ? new WorgenMale(Model, this, layerShader) : new WorgenFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Pandaren:
                helper = Gender ? new PandarenMale(Model, this, layerShader) : new PandarenFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Nightborne:
                helper = Gender ? new NightborneMale(Model, this, layerShader) : new NightborneFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Highmountain:
                helper = Gender ? new HighmountainMale(Model, this, layerShader) : new HighmountainFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.VoidElf:
                helper = Gender ? new VoidElfMale(Model, this, layerShader) : new VoidElfFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Lightforged:
                helper = Gender ? new LightforgedMale(Model, this, layerShader) : new LightforgedFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Zandalari:
                helper = Gender ? new ZandalariMale(Model, this, layerShader) : new ZandalariFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.KulTiran:
                helper = Gender ? new KulTiranMale(Model, this, layerShader) : new KulTiranFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.DarkIron:
                helper = Gender ? new DarkIronMale(Model, this, layerShader) : new DarkIronFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Vulpera:
                helper = Gender ? new VulperaMale(Model, this, layerShader) : new VulperaFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Maghar:
                helper = Gender ? new MagharMale(Model, this, layerShader) : new MagharFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Mechagnome:
                helper = Gender ? new MechagnomeMale(Model, this, layerShader) : new MechagnomeFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Dracthyr:
                helper = Gender ? new DracthyrMale(Model, this, layerShader) : new DracthyrFemale(Model, this, layerShader);
                break;
            case WoWHelper.Race.Earthen:
                helper = Gender ? new EarthenMale(Model, this, layerShader) : new EarthenFemale(Model, this, layerShader);
                break;
        }
    }

    // Enable creature mesh
    public void ActivateCreature()
    {
        mainMesh.SetActive(false);
        extraMesh?.SetActive(false);
        creature.gameObject.SetActive(true);
    }

    // Enable main mesh
    public void ActivateMainMesh()
    {
        mainMesh.SetActive(true);
        extraMesh?.SetActive(false);
        creature.gameObject.SetActive(false);
        mesh = mainMesh;
        Model = mainModel;
        helper.Model = mainModel;
        textures = new Texture2D[Model.Textures.Length];
        renderer = mesh.GetComponent<SkinnedMeshRenderer>();
        animator = mesh.GetComponent<Animator>();
        racial.gameObject.SetActive(true);
        armor.gameObject.SetActive(true);
        extraRacial.gameObject.SetActive(false);
    }

    // Enable extra mesh
    public void ActivateExtraMesh()
    {
        mainMesh.SetActive(false);
        extraMesh.SetActive(true);
        creature.gameObject.SetActive(false);
        mesh = extraMesh;
        Model = extraModel;
        helper.Model = extraModel;
        textures = new Texture2D[Model.Textures.Length];
        renderer = mesh.GetComponent<SkinnedMeshRenderer>();
        animator = mesh.GetComponent<Animator>();
        racial.gameObject.SetActive(false);
        armor.gameObject.SetActive(false);
        extraRacial.gameObject.SetActive(true);
    }

    // Load and setup main and extra model prefabs
    private IEnumerator LoadPrefab(string modelFile, string extraFile, string collectionFile, string armorFile, string extraCollectionFile)
    {
        bool done = false;
        DestroyImmediate(mainMesh);
        DestroyImmediate(extraMesh);
        extraMesh = null;
        mainModel = null;
        extraModel = null;
        string path = $"{modelsPath}{RacePath}{modelFile}_prefab";
        path = Path.GetRelativePath(modelsPath, path);
        GameObject prefab = Resources.Load<GameObject>($"{modelsPath}{path}");
        mainMesh = Instantiate(prefab, gameObject.transform);
        mesh = mainMesh;
        if (!string.IsNullOrEmpty(extraFile))
        {
            path = $"{modelsPath}{RacePath}{ExtraPath}{extraFile}_prefab";
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
        if (!string.IsNullOrEmpty(extraFile))
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
        if (!string.IsNullOrEmpty(collectionFile))
        {
            path = $@"{modelsPath}{RacePath}collection\{collectionFile}";
#if UNITY_EDITOR
            StartCoroutine(racial.LoadModel(path, mainMesh, mainModel.Skeleton.Bones, listFile, dataPath));
#else
            StartCoroutine(racial.LoadModel(path, mainMesh, mainModel.Skeleton.Bones, casc));
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
        if (!string.IsNullOrEmpty(armorFile))
        {
            path = $@"{modelsPath}{RacePath}armor\{armorFile}";
#if UNITY_EDITOR
            StartCoroutine(armor.LoadModel(path, mainMesh, mainModel.Skeleton.Bones, listFile, dataPath));
#else
            StartCoroutine(head.LoadModel(path, mainMesh, mainModel.Skeleton.Bones, casc));
#endif
            yield return null;
            while (!armor.Loaded)
            {
                yield return null;
            }
        }
        else
        {
            armor.UnloadModel();
            yield return null;
        }
        if (!string.IsNullOrEmpty(extraCollectionFile))
        {
            path = $@"{modelsPath}{RacePath}{ExtraPath}collection\{extraCollectionFile}";
            path = Path.GetRelativePath(modelsPath, path);
#if UNITY_EDITOR
            StartCoroutine(extraRacial.LoadModel($@"{modelsPath}{path}", extraMesh, extraModel.Skeleton.Bones, listFile, dataPath));
#else
            StartCoroutine(extraRacial.LoadModel($@"{modelsPath}{path}", extraMesh, extraModel.Skeleton.Bones, casc));
#endif
            yield return null;
            while (!extraRacial.Loaded)
            {
                yield return null;
            }
            extraRacial.gameObject.SetActive(false);
        }
        else
        {
            extraRacial.UnloadModel();
            yield return null;
        }
        if (done)
        {
            LoadColors();
            yield return null;
            textures = new Texture2D[Model.Textures.Length];
            InitializeHelper();
            yield return null;
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
            renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            animator = GetComponentInChildren<Animator>();
            Change = true;
            Loaded = !loadBinaries.IsAlive;
        }
    }

    // Load the model
#if UNITY_EDITOR
    public void LoadModel(string modelFile, string extraFile, string collectionFile, string armorFile,
        string extraCollectionFile, Dictionary<int, string> listFile, string dataPath)
#else
    public void LoadModel(string modelFile, string extraFile, string collectionFile, string armorFile, string extraCollectionFile, CASCHandler casc)
#endif
    {
        Loaded = false;
#if UNITY_EDITOR
        this.listFile = listFile;
        this.dataPath = dataPath;
#else
        this.casc = casc;
#endif
        loadBinaries?.Abort();
        StartCoroutine(LoadPrefab(modelFile, extraFile, collectionFile, armorFile, extraCollectionFile));
    }
}

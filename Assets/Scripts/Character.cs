using CASCLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using WoW;
using WoW.Characters;

//Main class to handle character rendering
public class Character : ModelRenderer
{
    //References to collection models
    public Collection[] collections;
    //Reference to Demon Hunter colleciton model
    public Collection demonHunter;
    //Referene to race specific collection model
    public Collection racial;
    //Reference to the main input class
    public ScreenInput input;

    //Reference to the helper class that handles specific race
    private CharacterHelper helper;
    //List of geosets that are enabled for loading
    private List<int> activeGeosets;
    //Textures that shift for worgen form
    private List<int> shiftTextures;

    //Dropdowns containing customization options
    public List<CustomDropdown> CustomizationDropdowns { get; set; }
    //Name of Demon Hunter collection file
    public string DemonHunterFile { get; set; }
    //Name of race specific collection file
    public string RacialCollection { get; set; }
    //Equipped items
    public ItemModel[] Items { get; set; }
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
    //Customization options for current character
    public CustomizationOption[] Options { get; set; }
    //Customization values for each option
    public int[] Customization { get; set; }
    //Customization choices for each option
    public CustomizationChoice[][] Choices { get; set; }

    private void Start()
    {
        //Initialize character
        Items = new ItemModel[13];
        shiftTextures = new List<int> { 3045083, 3045085, 3045087, 3045089 };
        modelsPath = @"character\";
        converter = new System.Drawing.ImageConverter();
        CustomizationDropdowns = new List<CustomDropdown>();
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
                    helper.ChangeGeosets(activeGeosets);
                    EquipArmor();
                    helper.LoadTextures(textures);
                    for (int i = 0; i < Model.Skin.Textures.Length; i++)
                    {
                        SetMaterial(renderer, i);
                    }
                    int index = Array.FindIndex(Options, o => o.Name == "Face");
                    animator.SetInteger("Face", Choices[index][Customization[index]].Bone);
                    demonHunter.Change = true;
                    racial.Change = true;
                    foreach (Collection collection in collections)
                    {
                        collection.Change = true;
                    }
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

    //Mark face options active/inactive depending on skin color
    public void ChangeFaceDropdown(int index, int index2)
    {
        for (int i = 0; i < CustomizationDropdowns[index2].options.Count; i++)
        {
            if (Choices[index2][((CustomOptionData)CustomizationDropdowns[index2].options[i]).Index].Textures[Customization[index]].Texture1 == -1)
            {
                ((CustomOptionData)CustomizationDropdowns[index2].options[i]).Interactable = false;
            }
            else
            {
                ((CustomOptionData)CustomizationDropdowns[index2].options[i]).Interactable = true;
            }
        }
    }

    //Mark tattoo color options active/inactive depending on chosen tattoo
    public void ChangeTattooDropdown(int index, int index2)
    {
        int bone = Choices[index][Customization[index]].Bone;
        for (int i = 0; i < CustomizationDropdowns[index2].options.Count; i++)
        {
            if (bone != 0)
            {
                if (i == bone)
                {
                    ((CustomOptionData)CustomizationDropdowns[index2].options[i]).Interactable = true;
                }
                else
                {
                    ((CustomOptionData)CustomizationDropdowns[index2].options[i]).Interactable = false;
                }
            }
            else
            {
                ((CustomOptionData)CustomizationDropdowns[index2].options[i]).Interactable = true;
            }
        }
    }

    //Mark customization options active/inactive depeing on other options chosen
    public void ChangeDropdown(int index, int index2)
    {
        int bone = Choices[index][Customization[index]].Bone;
        for (int i = 0; i < CustomizationDropdowns[index2].options.Count; i++)
        {
            if (i > bone)
            {
                ((CustomOptionData)CustomizationDropdowns[index2].options[i]).Interactable = false;
            }
            else
            {
                ((CustomOptionData)CustomizationDropdowns[index2].options[i]).Interactable = true;
            }
        }
    }

    //Mark customization options active/inactive depending on binary array
    public void ChangeDropdown(int index, int[] array)
    {
        for (int i = 0; i < CustomizationDropdowns[index].options.Count; i++)
        {
            if (array[i] == 0)
            {
                ((CustomOptionData)CustomizationDropdowns[index].options[i]).Interactable = false;
            }
            else
            {
                ((CustomOptionData)CustomizationDropdowns[index].options[i]).Interactable = true;
            }
        }
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
        }
        else
        {
            renderer.materials[Model.Skin.Textures[i].Id] = new Material(hiddenMaterial.shader);
            renderer.materials[Model.Skin.Textures[i].Id].shader = hiddenMaterial.shader;
            renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(hiddenMaterial);
        }
    }

    //Clear all the items
    public void ClearItems()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i] = null;
        }
    }

    //Load all the equipped items
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
        collections[0].UnloadModel();
        ItemObject helm = GameObject.Find("helm").GetComponent<ItemObject>();
        helm.UnloadModel();
        if (Items[0] != null)
        {
            if (Items[0].LeftModel > 0)
            {
                int model = Items[0].GetRaceSpecificModel(Items[0].LeftModel, Race, Gender, Class);
                StartCoroutine(helm.LoadModel(model, Items[0].LeftTexture, casc));
                helm.ParticleColors = Items[0].ParticleColors;
                helm.Change = true;
            }
            if (Items[0].RightModel > 0)
            {
                int model = Items[0].GetRaceSpecificModel(Items[0].RightModel, Race, Gender, Class);
                StartCoroutine(collections[0].LoadModel(model, Items[0].RightTexture, casc));
                collections[0].ActiveGeosets = new List<int>();
                collections[0].ActiveGeosets.Add(2701);
            }
            foreach (int helmet in Items[0].Helmet)
            {
                if (helmet == 0)
                {
                    activeGeosets.RemoveAll(x => x > 0 && x < 100);
                    activeGeosets.Add(1);
                }
                else if (helmet == 7)
                {
                    activeGeosets.RemoveAll(x => x > 699 && x < 800);
                    activeGeosets.Add(701);
                    if (racial.Loaded)
                    {
                        racial.ActiveGeosets.RemoveAll(x => x > 699 && x < 800);
                        racial.ActiveGeosets.Add(701);
                    }
                }
                else if (helmet == 24)
                {
                    if (racial.Loaded && !racial.ActiveGeosets.Contains(2400))
                    {
                        racial.ActiveGeosets.RemoveAll(x => x > 2399 && x < 2500);
                        racial.ActiveGeosets.Add(2401);
                    }
                }
                else if (helmet == 31)
                {
                    continue;
                }
                else
                {
                    activeGeosets.RemoveAll(x => x > helmet * 100 - 1 && x < (1 + helmet) * 100);
                    activeGeosets.Add(helmet * 100);
                    if (racial.Loaded)
                    {
                        racial.ActiveGeosets.RemoveAll(x => x > helmet * 100 - 1 && x < (1 + helmet) * 100);
                        racial.ActiveGeosets.Add(helmet * 100);
                    }
                }
            }
        }
    }

    //Load shoulder slot item model
    private void EquipShoulder()
    {
        ItemObject left = GameObject.Find("left shoulder").GetComponent<ItemObject>();
        ItemObject right = GameObject.Find("right shoulder").GetComponent<ItemObject>();
        left.UnloadModel();
        right.UnloadModel();
        if (Items[1] != null)
        {
            if (Items[1].LeftModel > 0)
            {
                int model = Items[1].GetSideSpecificModel(Items[1].LeftModel, false, Class);
                StartCoroutine(left.LoadModel(model, Items[1].LeftTexture, casc));
                left.ParticleColors = Items[1].ParticleColors;
                left.Change = true;
            }
            if (Items[1].RightModel > 0)
            {
                int model = Items[1].GetSideSpecificModel(Items[1].RightModel, true, Class);
                StartCoroutine(right.LoadModel(model, Items[1].RightTexture, casc));
                right.ParticleColors = Items[1].ParticleColors;
                right.Change = true;
            }
        }
    }

    //Load back slot item models and geosets
    private void EquipBack()
    {
        collections[1].UnloadModel();
        ItemObject backpack = GameObject.Find("backpack").GetComponent<ItemObject>();
        backpack.UnloadModel();
        activeGeosets.RemoveAll(x => x > 1499 && x < 1600);
        if (Items[2] != null)
        {
            if (Items[2].LeftModel > 0)
            {
                int model = Items[2].GetRaceSpecificModel(Items[2].LeftModel, Race, Gender, Class);
                StartCoroutine(collections[1].LoadModel(model, Items[2].LeftTexture, casc));
                collections[1].ActiveGeosets = new List<int>();
                collections[1].ActiveGeosets.Add(1501);
            }
            if (Items[2].RightModel > 0)
            {
                int model = Items[2].GetModel(Items[2].RightModel, Class);
                StartCoroutine(backpack.LoadModel(model, Items[2].RightTexture, casc));
                backpack.ParticleColors = Items[2].ParticleColors;
                backpack.Change = true;
            }
            int geoset = Items[2].Geoset1;
            if (Race == 5)
            {
                int index = Array.FindIndex(Options, o => o.Name == "Skin Type");
                if (Customization[index] > 0)
                {
                    geoset += 9;
                }
            }
            activeGeosets.Add(1501 + geoset);
        }
        else
        {
            activeGeosets.Add(1501);
        }
    }

    //Load chest slot item models and geosets
    private void EquipChest()
    {
        collections[2].UnloadModel();
        activeGeosets.RemoveAll(x => x > 799 && x < 900);
        activeGeosets.RemoveAll(x => x > 999 && x < 1100);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        activeGeosets.RemoveAll(x => x > 2199 && x < 2300);
        if (Items[3] != null)
        {
            if (Items[3].LeftModel > 0)
            {
                int model = Items[3].GetRaceSpecificModel(Items[3].LeftModel, Race, Gender, Class);
                StartCoroutine(collections[2].LoadModel(model, Items[3].LeftTexture, casc));
                collections[2].ActiveGeosets = new List<int>();
                collections[2].ActiveGeosets.Add(801);
                collections[2].ActiveGeosets.Add(1001);
                if (Items[3].Slot == 20)
                {
                    collections[2].ActiveGeosets.Add(1301);
                }
                collections[2].ActiveGeosets.Add(2201);
                collections[2].ActiveGeosets.Add(2801);
            }
            activeGeosets.Add(801 + Items[3].Geoset1);
            activeGeosets.Add(1001 + Items[3].Geoset2);
            activeGeosets.Add(1301 + Items[3].Geoset3);
            if (Items[3].Geoset3 == 1)
            {
                activeGeosets.RemoveAll(x => x > 1099 && x < 1200);
                activeGeosets.RemoveAll(x => x > 899 && x < 1000);
                if (racial.Loaded)
                {
                    racial.ActiveGeosets.RemoveAll(x => x > 2999 && x < 3100);
                }
            }
            activeGeosets.Add(2201 + Items[3].Geoset4);
            if (Items[3].UpperLeg > 0)
            {
                activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
                if (demonHunter.Loaded)
                {
                    demonHunter.ActiveGeosets.RemoveAll(x => x > 1399 && x < 1500);
                }
                if (collections[5].Loaded)
                {
                    collections[5].ActiveGeosets.Clear();
                }
            }
        }
        else
        {
            activeGeosets.Add(801);
            activeGeosets.Add(1001);
            activeGeosets.Add(1301);
            activeGeosets.Add(2201);
        }
        if (Items[10] != null && Items[10].Geoset3 == 1)
        {
            activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
            activeGeosets.Add(1302);
            if (racial.Loaded)
            {
                racial.ActiveGeosets.RemoveAll(x => x > 2999 && x < 3100);
            }
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
        if (Items[4] != null)
        {
            activeGeosets.Add(801 + Items[4].Geoset1);
        }
        else
        {
            activeGeosets.Add(801);
        }
    }

    //Load tabard slot item geosets
    private void EquipTabard()
    {
        activeGeosets.RemoveAll(x => x > 1199 && x < 1300);
        if (Items[5] != null)
        {
            if ((Items[3] != null && Items[3].Geoset3 == 1) || (Items[10] != null && Items[10].Geoset3 == 1))
            {
                activeGeosets.Add(1201);
            }
            else
            {
                activeGeosets.Add(1201 + Items[5].Geoset1);
            }
        }
        else
        {
            activeGeosets.Add(1201);
        }
    }

    //Load wrist slot item geosets
    private void EquipWrist()
    {
        if (Items[6] != null)
        {
            activeGeosets.RemoveAll(x => x > 799 && x < 900);
        }
    }

    //Load hands slot item models and geosets
    private void EquipHands()
    {
        collections[3].UnloadModel();
        activeGeosets.RemoveAll(x => x > 399 && x < 500);
        if (Items[8] != null)
        {
            if (Items[8].LeftModel > 0)
            {
                int model = Items[8].GetRaceSpecificModel(Items[8].LeftModel, Race, Gender, Class);
                StartCoroutine(collections[3].LoadModel(model, Items[8].LeftTexture, casc));
                collections[3].ActiveGeosets = new List<int>();
                if (Race != 37)
                {
                    if (activeGeosets.Contains(801))
                    {
                        collections[3].ActiveGeosets.Add(401);
                    }
                    collections[3].ActiveGeosets.Add(2301);
                }
            }
            activeGeosets.Add(401 + Items[8].Geoset1);
            if (Items[8].Geoset1 != 0)
            {
                activeGeosets.RemoveAll(x => x > 799 && x < 900);
            }
        }
        else
        {
            activeGeosets.Add(401);
        }
    }

    //Load waist slot item models and geosets
    private void EquipWaist()
    {
        collections[4].UnloadModel();
        ItemObject buckle = GameObject.Find("buckle").GetComponent<ItemObject>();
        buckle.UnloadModel();
        activeGeosets.RemoveAll(x => x > 1799 && x < 1900);
        if (Items[9] != null)
        {
            if (Items[9].RightModel > 0)
            {
                int model = Items[9].GetRaceSpecificModel(Items[9].RightModel, Race, Gender, Class);
                StartCoroutine(collections[4].LoadModel(model, Items[9].RightTexture, casc));
                collections[4].ActiveGeosets = new List<int>();
                collections[4].ActiveGeosets.Add(1801);
            }
            if (Items[9].LeftModel > 0)
            {
                int model = Items[9].GetModel(Items[9].LeftModel, Class);
                StartCoroutine(buckle.LoadModel(model, Items[9].LeftTexture, casc));
                buckle.ParticleColors = Items[9].ParticleColors;
                buckle.Change = true;
            }
            activeGeosets.Add(1801 + Items[9].Geoset1);
            if (Items[9].Geoset1 == 1)
            {
                activeGeosets.RemoveAll(x => x > 999 && x < 1100);
            }
        }
        else
        {
            activeGeosets.Add(1801);
        }
    }

    //Load legs slot item models and geosets
    private void EquipLegs()
    {
        collections[5].UnloadModel();
        activeGeosets.RemoveAll(x => x > 1099 && x < 1200);
        activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        if (Items[10] != null)
        {
            if (Items[10].LeftModel > 0)
            {
                int model = Items[10].GetRaceSpecificModel(Items[10].LeftModel, Race, Gender, Class);
                StartCoroutine(collections[5].LoadModel(model, Items[10].LeftTexture, casc));
                collections[5].ActiveGeosets = new List<int>();
                collections[5].ActiveGeosets.Add(901);
                if (Race != 37)
                {
                    collections[5].ActiveGeosets.Add(1101);
                }
            }
            activeGeosets.Add(1101 + Items[10].Geoset1);
            activeGeosets.Add(901 + Items[10].Geoset2);
            if (Items[10].Geoset1 != 3)
            {
                activeGeosets.Add(1301 + Items[10].Geoset3);
            }
            if (Items[10].UpperLeg > 0)
            {
                activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
                if (demonHunter.Loaded)
                {
                    demonHunter.ActiveGeosets.RemoveAll(x => x > 1399 && x < 1500);
                }
            }
        }
        else
        {
            activeGeosets.Add(1101);
            activeGeosets.Add(901);
            activeGeosets.Add(1301);
        }
    }

    //Load feet slot item models and geosets
    private void EquipFeet()
    {
        collections[6].UnloadModel();
        activeGeosets.RemoveAll(x => x > 499 && x < 600);
        activeGeosets.RemoveAll(x => x > 1999 && x < 2100);
        if (Items[11] != null)
        {
            if (Items[11].LeftModel > 0)
            {
                int model = Items[11].GetRaceSpecificModel(Items[11].LeftModel, Race, Gender, Class);
                StartCoroutine(collections[6].LoadModel(model, Items[11].LeftTexture, casc));
                collections[6].ActiveGeosets = new List<int>();
                if (Race != 37)
                {
                    if (!activeGeosets.Contains(1302))
                    {
                        collections[6].ActiveGeosets.Add(501);
                    }
                    collections[6].ActiveGeosets.Add(2001);
                }
            }
            if (!activeGeosets.Contains(1302))
            {
                activeGeosets.Add(501 + Items[11].Geoset1);
            }
            if (Items[11].Geoset1 != 0)
            {
                activeGeosets.RemoveAll(x => x > 899 && x < 1000);
            }
            activeGeosets.Add(2002 - Items[11].Geoset2);
        }
        else
        {
            if (!activeGeosets.Contains(1302))
            {
                activeGeosets.Add(501);
            }
            activeGeosets.Add(2001);
        }
    }

    //Load main hand slot item models and geosets
    private void EquipRightHand()
    {
        ItemObject right = GameObject.Find("right hand").GetComponent<ItemObject>();
        ItemObject book = GameObject.Find("book").GetComponent<ItemObject>();
        right.UnloadModel();
        book.UnloadModel();
        if (Items[7] != null)
        {
            if (Items[7].Slot != 15)
            {
                if (Items[7].LeftModel > 0)
                {
                    int model = Items[7].GetModel(Items[7].LeftModel, Class);
                    StartCoroutine(right.LoadModel(model, Items[7].LeftTexture, casc));
                    right.ParticleColors = Items[7].ParticleColors;
                    right.Change = true;
                }
            }
            if (Items[7].Slot != 15 && Items[7].Slot != 26)
            {
                if (Items[7].RightModel > 0)
                {
                    int model = Items[7].GetModel(Items[7].RightModel, Class);
                    StartCoroutine(book.LoadModel(model, Items[7].RightTexture, casc));
                    book.ParticleColors = Items[7].ParticleColors;
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
        if (Items[7] != null)
        {
            if (Items[7].Slot == 15)
            {
                left.transform.localScale = new Vector3(1f, 1f, 1f);
                if (Items[7].LeftModel > 0)
                {
                    int model = Items[7].GetModel(Items[7].LeftModel, Class);
                    StartCoroutine(left.LoadModel(model, Items[7].LeftTexture, casc));
                    left.ParticleColors = Items[7].ParticleColors;
                    left.Change = true;
                }
            }
            if (Items[7].Slot == 15 || Items[7].Slot == 26)
            {
                if (Items[7].RightModel > 0)
                {
                    int model = Items[7].GetModel(Items[7].RightModel, Class);
                    StartCoroutine(quiver.LoadModel(model, Items[7].RightTexture, casc));
                    quiver.ParticleColors = Items[7].ParticleColors;
                    quiver.Change = true;
                }
            }
        }
        if (Items[12] != null)
        {
            if (Items[12].Slot == 14)
            {
                if (Items[12].LeftModel > 0)
                {
                    int model = Items[12].GetModel(Items[12].LeftModel, Class);
                    StartCoroutine(shield.LoadModel(model, Items[12].LeftTexture, casc));
                    shield.ParticleColors = Items[12].ParticleColors;
                    shield.Change = true;
                }
            }
            else
            {
                if (Items[12].LeftModel > 0)
                {
                    left.transform.localScale = new Vector3(1f, 1f, -1f);
                    int model = Items[12].GetModel(Items[12].LeftModel, Class);
                    StartCoroutine(left.LoadModel(model, Items[12].LeftTexture, casc));
                    left.ParticleColors = Items[12].ParticleColors;
                    left.Change = true;
                }
            }
        }
    }

    //Load item texture
    Texture2D LoadTexture(int file, int width, int height)
    {
        if (file == 0)
        {
            return null;
        }
        if (Race == 22 && Form != 7 && shiftTextures.Contains(file))
        {
            file++;
        }
        Texture2D texture = TextureFromBLP(file);
        return TextureScaler.scaled(texture, width, height);
    }

    //Draw chest slot item textures
    public void TextureChest(Texture2D texture)
    {
        if (Items[3] != null)
        {
            if (Items[3].UpperArm > 0)
            {
                Texture2D armUpper = LoadTexture(Items[3].UpperArm, 256, 128);
                if (armUpper != null)
                {
                    helper.DrawTexture(texture, armUpper, 0, 384);
                }
            }
            if (Items[3].LowerArm > 0)
            {
                Texture2D armLower = LoadTexture(Items[3].LowerArm, 256, 128);
                if (armLower != null)
                {
                    helper.DrawTexture(texture, armLower, 0, 256);
                }
            }
            if (Items[3].UpperTorso > 0)
            {
                Texture2D torsoUpper = LoadTexture(Items[3].UpperTorso, 256, 128);
                if (torsoUpper != null)
                {
                    helper.DrawTexture(texture, torsoUpper, 256, 384);
                }
            }
            if (Items[3].LowerTorso > 0)
            {
                Texture2D torsoLower = LoadTexture(Items[3].LowerTorso, 256, 64);
                if (torsoLower != null)
                {
                    helper.DrawTexture(texture, torsoLower, 256, 320);
                }
            }
            if (Items[3].UpperLeg > 0)
            {
                Texture2D legUpper = LoadTexture(Items[3].UpperLeg, 256, 128);
                if (legUpper != null)
                {
                    helper.DrawTexture(texture, legUpper, 256, 192);
                }
            }
            if (Items[3].LowerLeg > 0)
            {
                Texture2D legLower = LoadTexture(Items[3].LowerLeg, 256, 128);
                if (legLower != null)
                {
                    helper.DrawTexture(texture, legLower, 256, 64);
                }
            }
        }
    }

    //Draw shirt slot item textures
    public void TextureShirt(Texture2D texture)
    {
        if (Items[4] != null)
        {
            if (Items[4].UpperArm > 0)
            {
                Texture2D armUpper = LoadTexture(Items[4].UpperArm, 256, 128);
                if (armUpper != null)
                {
                    helper.DrawTexture(texture, armUpper, 0, 384);
                }
            }
            if (Items[4].LowerArm > 0)
            {
                Texture2D armLower = LoadTexture(Items[4].LowerArm, 256, 128);
                if (armLower != null)
                {
                    helper.DrawTexture(texture, armLower, 0, 256);
                }
            }
            if (Items[4].UpperTorso > 0)
            {
                Texture2D torsoUpper = LoadTexture(Items[4].UpperTorso, 256, 128);
                if (torsoUpper != null)
                {
                    helper.DrawTexture(texture, torsoUpper, 256, 384);
                }
            }
            if (Items[4].LowerTorso > 0)
            {
                Texture2D torsoLower = LoadTexture(Items[4].LowerTorso, 256, 64);
                if (torsoLower != null)
                {
                    helper.DrawTexture(texture, torsoLower, 256, 320);
                }
            }
        }
    }

    //Draw tabard slot item textures
    public void TextureTabard(Texture2D texture)
    {
        if (Items[5] != null)
        {
            if (Items[5].UpperTorso > 0)
            {
                Texture2D torsoUpper = LoadTexture(Items[5].UpperTorso, 256, 128);
                if (torsoUpper != null)
                {
                    helper.DrawTexture(texture, torsoUpper, 256, 384);
                }
            }
            if (Items[5].LowerTorso > 0)
            {
                Texture2D torsoLower = LoadTexture(Items[5].LowerTorso, 256, 64);
                if (torsoLower != null)
                {
                    helper.DrawTexture(texture, torsoLower, 256, 320);
                }
            }
            if (Items[5].UpperLeg > 0)
            {
                Texture2D legUpper = LoadTexture(Items[5].UpperLeg, 256, 128);
                if (legUpper != null)
                {
                    helper.DrawTexture(texture, legUpper, 256, 192);
                }
            }
        }
    }

    //Draw wrist slot item textures
    public void TextureWrist(Texture2D texture)
    {
        if (Items[6] != null)
        {
            if (Items[6].LowerArm > 0)
            {
                Texture2D armLower = LoadTexture(Items[6].LowerArm, 256, 128);
                if (armLower != null)
                {
                    helper.DrawTexture(texture, armLower, 0, 256);
                }
            }
        }
    }

    //Draw hands slot item textures
    public void TextureHands(Texture2D texture)
    {
        if (Items[8] != null)
        {
            if (Items[8].LowerArm > 0)
            {
                Texture2D armLower = LoadTexture(Items[8].LowerArm, 256, 128);
                if (armLower != null)
                {
                    helper.DrawTexture(texture, armLower, 0, 256);
                }
            }
            if (Items[8].Hand > 0)
            {
                Texture2D hand = LoadTexture(Items[8].Hand, 256, 64);
                if (hand != null)
                {
                    helper.DrawTexture(texture, hand, 0, 192);
                }
            }
        }
    }

    //Draw waist slot item textures
    public void TextureWaist(Texture2D texture)
    {
        if (Items[9] != null)
        {
            if (Items[9].LowerTorso > 0)
            {
                Texture2D torsoLower = LoadTexture(Items[9].LowerTorso, 256, 64);
                if (torsoLower != null)
                {
                    helper.DrawTexture(texture, torsoLower, 256, 320);
                }
            }
            if (Items[9].UpperLeg > 0)
            {
                Texture2D legUpper = LoadTexture(Items[9].UpperLeg, 256, 128);
                if (legUpper != null)
                {
                    helper.DrawTexture(texture, legUpper, 256, 192);
                }
            }
        }
    }

    //Draw legs slot item textures
    public void TextureLegs(Texture2D texture)
    {
        if (Items[10] != null)
        {
            if (Items[10].UpperLeg > 0)
            {
                Texture2D legUpper = LoadTexture(Items[10].UpperLeg, 256, 128);
                if (legUpper != null)
                {
                    helper.DrawTexture(texture, legUpper, 256, 192);
                }
            }
            if (Items[10].LowerLeg > 0)
            {
                Texture2D legLower = LoadTexture(Items[10].LowerLeg, 256, 128);
                if (legLower != null)
                {
                    helper.DrawTexture(texture, legLower, 256, 64);
                }
            }
        }
    }

    //Draw feet slot item textures
    public void TextureFeet(Texture2D texture, bool showFeet = false)
    {
        if (Items[11] != null)
        {
            if (Items[11].LowerLeg > 0 && !activeGeosets.Contains(1302))
            {
                Texture2D legLower = LoadTexture(Items[11].LowerLeg, 256, 128);
                if (legLower != null)
                {
                    helper.DrawTexture(texture, legLower, 256, 64);
                }
            }
            if (Items[11].Foot > 0 && !showFeet)
            {
                Texture2D foot = LoadTexture(Items[11].Foot, 256, 64);
                if (foot != null)
                {
                    helper.DrawTexture(texture, foot, 256, 0);
                }
            }
        }
    }

    //Draw black texture based on chest slot textures to obstruct tattoo emission
    public void BlackChest(Texture2D texture)
    {
        if (Items[3] != null)
        {
            if (Items[3].UpperArm > 0)
            {
                Texture2D armUpper = LoadTexture(Items[3].UpperArm, 256, 128);
                helper.BlackTexture(armUpper, armUpper);
                armUpper.Apply();
                helper.DrawTexture(texture, armUpper, 0, 384);
            }
            if (Items[3].LowerArm > 0)
            {
                Texture2D armLower = LoadTexture(Items[3].LowerArm, 256, 128);
                helper.BlackTexture(armLower, armLower);
                armLower.Apply();
                helper.DrawTexture(texture, armLower, 0, 256);
            }
            if (Items[3].UpperTorso > 0)
            {
                Texture2D torsoUpper = LoadTexture(Items[3].UpperTorso, 256, 128);
                helper.BlackTexture(torsoUpper, torsoUpper);
                torsoUpper.Apply();
                helper.DrawTexture(texture, torsoUpper, 256, 384);
            }
            if (Items[3].LowerTorso > 0)
            {
                Texture2D torsoLower = LoadTexture(Items[3].LowerTorso, 256, 64);
                helper.BlackTexture(torsoLower, torsoLower);
                torsoLower.Apply();
                helper.DrawTexture(texture, torsoLower, 256, 320);
            }
            if (Items[3].UpperLeg > 0)
            {
                Texture2D legUpper = LoadTexture(Items[3].UpperLeg, 256, 128);
                helper.BlackTexture(legUpper, legUpper);
                legUpper.Apply();
                helper.DrawTexture(texture, legUpper, 256, 192);
            }
            if (Items[3].LowerLeg > 0)
            {
                Texture2D legLower = LoadTexture(Items[3].LowerLeg, 256, 128);
                helper.BlackTexture(legLower, legLower);
                legLower.Apply();
                helper.DrawTexture(texture, legLower, 256, 64);
            }
        }
    }

    //Draw black texture based on shirt slot textures to obstruct tattoo emission
    public void BlackShirt(Texture2D texture)
    {
        if (Items[4] != null)
        {
            if (Items[4].UpperArm > 0)
            {
                Texture2D armUpper = LoadTexture(Items[4].UpperArm, 256, 128);
                helper.BlackTexture(armUpper, armUpper);
                armUpper.Apply();
                helper.DrawTexture(texture, armUpper, 0, 384);
            }
            if (Items[4].LowerArm > 0)
            {
                Texture2D armLower = LoadTexture(Items[4].LowerArm, 256, 128);
                helper.BlackTexture(armLower, armLower);
                armLower.Apply();
                helper.DrawTexture(texture, armLower, 0, 256);
            }
            if (Items[4].UpperTorso > 0)
            {
                Texture2D torsoUpper = LoadTexture(Items[4].UpperTorso, 256, 128);
                helper.BlackTexture(torsoUpper, torsoUpper);
                torsoUpper.Apply();
                helper.DrawTexture(texture, torsoUpper, 256, 384);
            }
            if (Items[4].LowerTorso > 0)
            {
                Texture2D torsoLower = LoadTexture(Items[4].LowerTorso, 256, 64);
                helper.BlackTexture(torsoLower, torsoLower);
                torsoLower.Apply();
                helper.DrawTexture(texture, torsoLower, 256, 320);
            }
        }
    }

    //Draw black texture based on tabard slot textures to obstruct tattoo emission
    public void BlackTabard(Texture2D texture)
    {
        if (Items[5] != null)
        {
            if (Items[5].UpperTorso > 0)
            {
                Texture2D torsoUpper = LoadTexture(Items[5].UpperTorso, 256, 128);
                helper.BlackTexture(torsoUpper, torsoUpper);
                torsoUpper.Apply();
                helper.DrawTexture(texture, torsoUpper, 256, 384);
            }
            if (Items[5].LowerTorso > 0)
            {
                Texture2D torsoLower = LoadTexture(Items[5].LowerTorso, 256, 64);
                helper.BlackTexture(torsoLower, torsoLower);
                torsoLower.Apply();
                helper.DrawTexture(texture, torsoLower, 256, 320);
            }
            if (Items[5].UpperLeg > 0)
            {
                Texture2D legUpper = LoadTexture(Items[5].UpperLeg, 256, 128);
                helper.BlackTexture(legUpper, legUpper);
                legUpper.Apply();
                helper.DrawTexture(texture, legUpper, 256, 192);
            }
        }
    }

    //Draw black texture based on wrist slot textures to obstruct tattoo emission
    public void BlackWrist(Texture2D texture)
    {
        if (Items[6] != null)
        {
            if (Items[6].LowerArm > 0)
            {
                Texture2D armLower = LoadTexture(Items[6].LowerArm, 256, 128);
                helper.BlackTexture(armLower, armLower);
                armLower.Apply();
                helper.DrawTexture(texture, armLower, 0, 256);
            }
        }
    }

    //Draw black texture based on hands slot textures to obstruct tattoo emission
    public void BlackHands(Texture2D texture)
    {
        if (Items[8] != null)
        {
            if (Items[8].LowerArm > 0)
            {
                Texture2D armLower = LoadTexture(Items[8].LowerArm, 256, 128);
                helper.BlackTexture(armLower, armLower);
                armLower.Apply();
                helper.DrawTexture(texture, armLower, 0, 256);
            }
            if (Items[8].Hand > 0)
            {
                Texture2D hand = LoadTexture(Items[8].Hand, 256, 64);
                helper.BlackTexture(hand, hand);
                hand.Apply();
                helper.DrawTexture(texture, hand, 0, 192);
            }
        }
    }

    //Draw black texture based on waist slot textures to obstruct tattoo emission
    public void BlackWaist(Texture2D texture)
    {
        if (Items[9] != null)
        {
            if (Items[9].LowerTorso > 0)
            {
                Texture2D torsoLower = LoadTexture(Items[9].LowerTorso, 256, 64);
                helper.BlackTexture(torsoLower, torsoLower);
                torsoLower.Apply();
                helper.DrawTexture(texture, torsoLower, 256, 320);
            }
            if (Items[9].UpperLeg > 0)
            {
                Texture2D legUpper = LoadTexture(Items[9].UpperLeg, 256, 128);
                helper.BlackTexture(legUpper, legUpper);
                legUpper.Apply();
                helper.DrawTexture(texture, legUpper, 256, 192);
            }
        }
    }

    //Draw black texture based on legs slot textures to obstruct tattoo emission
    public void BlackLegs(Texture2D texture)
    {
        if (Items[10] != null)
        {
            if (Items[10].UpperLeg > 0)
            {
                Texture2D legUpper = LoadTexture(Items[10].UpperLeg, 256, 128);
                helper.BlackTexture(legUpper, legUpper);
                legUpper.Apply();
                helper.DrawTexture(texture, legUpper, 256, 192);
            }
            if (Items[10].LowerLeg > 0)
            {
                Texture2D legLower = LoadTexture(Items[10].LowerLeg, 256, 128);
                helper.BlackTexture(legLower, legLower);
                legLower.Apply();
                helper.DrawTexture(texture, legLower, 256, 64);
            }
        }
    }

    //Draw black texture based on feet slot textures to obstruct tattoo emission
    public void BlackFeet(Texture2D texture, bool showFeet = false)
    {
        if (Items[11] != null)
        {
            if (Items[11].LowerLeg > 0)
            {
                Texture2D legLower = LoadTexture(Items[11].LowerLeg, 256, 128);
                helper.BlackTexture(legLower, legLower);
                legLower.Apply();
                helper.DrawTexture(texture, legLower, 256, 64);
            }
            if (Items[11].Foot > 0 && !showFeet)
            {
                Texture2D foot = LoadTexture(Items[11].Foot, 256, 64);
                helper.BlackTexture(foot, foot);
                foot.Apply();
                helper.DrawTexture(texture, foot, 256, 0);
            }
        }
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
        if (Race == 4 || Race == 10)
        {
            demonHunter.Path = modelsPath + RacePath;
            StartCoroutine(demonHunter.LoadModel(DemonHunterFile, casc));
            yield return null;
            while (!demonHunter.Loaded)
            {
                yield return null;
            }
        }
        else
        {
            demonHunter.UnloadModel();
            yield return null;
        }
        if (Race == 37)
        {
            racial.Path = modelsPath + RacePath;
            StartCoroutine(racial.LoadModel(RacialCollection, casc));
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

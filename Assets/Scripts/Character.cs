using CASCLib;
using M2Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using WoW;
using WoW.Characters;

public class Character : MonoBehaviour
{
    public Material hiddenMaterial;
    //public Collection[] collections;
    public Collection demonHunter;
    //public Collection racial;
    public ScreenInput input;

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

    public List<CustomDropdown> CustomizationDropdowns { get; set; }
    public string DemonHunterFile { get; set; }
    public string RacialCollection { get; set; }
    //public ItemModel[] Items { get; set; }
    public string Suffix1 { get; set; }
    public string Suffix2 { get; set; }
    public string RacePath { get; set; }
    public int Race { get; set; }
    public int Class { get; set; }
    public bool Gender { get; set; }
    public int Form { get; set; }
    public bool Change { get; set; }
    public CustomizationOption[] Options { get; set; }
    public int[] Customization { get; set; }
    public CustomizationChoice[][] Choices { get; set; }

    private void Start()
    {
        //Items = new ItemModel[13];
        modelsPath = @"character\";
        CustomizationDropdowns = new List<CustomDropdown>();
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
                    //racial.Change = true;
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
            index= Model.TextureAnimationsLookup[Model.Skin.Textures[i].TextureAnimation + 1];
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

    private void SetMaterial(SkinnedMeshRenderer renderer, int i)
    {
        if (Race == 9 && Model.Skin.Textures[i].Id == 1)
        {
            renderer.materials[Model.Skin.Textures[i].Id] = new Material(hiddenMaterial.shader);
            renderer.materials[Model.Skin.Textures[i].Id].shader = hiddenMaterial.shader;
            renderer.materials[Model.Skin.Textures[i].Id].CopyPropertiesFromMaterial(hiddenMaterial);
        }
        else if (activeGeosets.Contains(Model.Skin.Submeshes[Model.Skin.Textures[i].Id].Id))
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

    public void ClearItems()
    {
        //for (int i = 0; i < Items.Length; i++)
        //{
        //    Items[i] = null;
        //}
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

    private void EquipHead()
    {
        //collections[0].UnloadModel();
        //ItemObject helm = GameObject.Find("helm").GetComponent<ItemObject>();
        //helm.UnloadModel();
        //if (Items[0] != null)
        //{
        //    if (Items[0].LeftModel != "")
        //    {
        //        string model = Items[0].LeftModel.Substring(0, Items[0].LeftModel.Length - 7) + Suffix1;
        //        helm.LoadModel(model);
        //        helm.Texture = Items[0].LeftTexture;
        //        helm.ParticleColors = Items[0].ParticleColors;
        //        helm.Path = @"item\objectcomponents\head\";
        //        helm.Change = true;
        //    }
        //    if (Items[0].RightModel != "")
        //    {
        //        collections[0].Path = "item/objectcomponents/collections/";
        //        string model = Items[0].RightModel.Substring(0, Items[0].RightModel.Length - 7) + Suffix2;
        //        collections[0].Texture = Items[0].RightTexture;
        //        collections[0].LoadModel(model.Replace(collections[0].Path, ""));
        //        collections[0].ActiveGeosets.Clear();
        //        collections[0].ActiveGeosets.Add(2701);
        //    }
        //    foreach (int helmet in Items[0].Helmet)
        //    {
        //        if (helmet == 0)
        //        {
        //            activeGeosets.RemoveAll(x => x > 0 && x < 100);
        //            activeGeosets.Add(1);
        //        }
        //        else if (helmet == 7)
        //        {
        //            activeGeosets.RemoveAll(x => x > 699 && x < 800);
        //            activeGeosets.Add(701);
        //            if (racial.Loaded)
        //            {
        //                racial.ActiveGeosets.RemoveAll(x => x > 699 && x < 800);
        //                racial.ActiveGeosets.Add(701);
        //            }
        //        }
        //        else if (helmet == 24)
        //        {
        //            if (racial.Loaded && !racial.ActiveGeosets.Contains(2400))
        //            {
        //                racial.ActiveGeosets.RemoveAll(x => x > 2399 && x < 2500);
        //                racial.ActiveGeosets.Add(2401);
        //            }
        //        }
        //        else if (helmet == 31)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            activeGeosets.RemoveAll(x => x > helmet * 100 - 1 && x < (1 + helmet) * 100);
        //            activeGeosets.Add(helmet * 100);
        //            if (racial.Loaded)
        //            {
        //                racial.ActiveGeosets.RemoveAll(x => x > helmet * 100 - 1 && x < (1 + helmet) * 100);
        //                racial.ActiveGeosets.Add(helmet * 100);
        //            }
        //        }
        //    }
        //}
    }

    private void EquipShoulder()
    {
        //ItemObject left = GameObject.Find("left shoulder").GetComponent<ItemObject>();
        //ItemObject right = GameObject.Find("right shoulder").GetComponent<ItemObject>();
        //left.UnloadModel();
        //right.UnloadModel();
        //if (Items[1] != null)
        //{
        //    if (Items[1].LeftModel != "")
        //    {
        //        left.LoadModel(Items[1].LeftModel);
        //        left.Texture = Items[1].LeftTexture;
        //        left.ParticleColors = Items[1].ParticleColors;
        //        left.Path = @"item\objectcomponents\shoulder\";
        //        left.Change = true;
        //    }
        //    if (Items[1].RightModel != "")
        //    {
        //        right.LoadModel(Items[1].RightModel);
        //        right.Texture = Items[1].RightTexture;
        //        right.ParticleColors = Items[1].ParticleColors;
        //        right.Path = @"item\objectcomponents\shoulder\";
        //        right.Change = true;
        //    }
        //}
    }

    private void EquipBack()
    {
        //collections[1].UnloadModel();
        //activeGeosets.RemoveAll(x => x > 1499 && x < 1600);
        //if (Items[2] != null)
        //{
        //    if (Items[2].LeftModel != "")
        //    {
        //        collections[1].Path = "item/objectcomponents/collections/";
        //        string model = Items[2].LeftModel.Substring(0, Items[2].LeftModel.Length - 7) + Suffix2;
        //        collections[1].Texture = Items[2].LeftTexture;
        //        collections[1].LoadModel(model.Replace(collections[1].Path, ""));
        //        collections[1].ActiveGeosets.Clear();
        //        collections[1].ActiveGeosets.Add(1501);
        //    }
        //    activeGeosets.Add(1501 + Items[2].Geoset1);
        //}
        //else
        //{
        //    activeGeosets.Add(1501);
        //}
    }

    private void EquipChest()
    {
        //collections[2].UnloadModel();
        activeGeosets.RemoveAll(x => x > 799 && x < 900);
        activeGeosets.RemoveAll(x => x > 999 && x < 1100);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        activeGeosets.RemoveAll(x => x > 2199 && x < 2300);
        //if (Items[3] != null)
        //{
        //    if (Items[3].LeftModel != "")
        //    {
        //        collections[2].Path = "item/objectcomponents/collections/";
        //        string model = Items[3].LeftModel.Substring(0, Items[3].LeftModel.Length - 7) + Suffix2;
        //        collections[2].Texture = Items[3].LeftTexture;
        //        collections[2].LoadModel(model.Replace(collections[2].Path, ""));
        //        collections[2].ActiveGeosets.Clear();
        //        collections[2].ActiveGeosets.Add(801);
        //        collections[2].ActiveGeosets.Add(1001);
        //        collections[2].ActiveGeosets.Add(1301);
        //        collections[2].ActiveGeosets.Add(2201);
        //        collections[2].ActiveGeosets.Add(2801);
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
        //    }
        //    activeGeosets.Add(2201 + Items[3].Geoset3);
        //    if (Items[3].UpperLeg != "")
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
        //else
        //{
            activeGeosets.Add(801);
            activeGeosets.Add(1001);
            activeGeosets.Add(1301);
            activeGeosets.Add(2201);
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

    private void EquipShirt()
    {
        //activeGeosets.RemoveAll(x => x > 799 && x < 900);
        //if (Items[4] != null)
        //{
        //    activeGeosets.Add(801 + Items[4].Geoset1);
        //}
        //else
        //{
        //    activeGeosets.Add(801);
        //}
    }

    private void EquipTabard()
    {
        //activeGeosets.RemoveAll(x => x > 1199 && x < 1300);
        //if (Items[5] != null)
        //{
        //    activeGeosets.Add(1201 + Items[5].Geoset1);
        //}
        //else
        //{
        //    activeGeosets.Add(1201);
        //}
    }

    private void EquipWrist()
    {
        //if (Items[6] != null)
        //{
        //    activeGeosets.RemoveAll(x => x > 799 && x < 900);
        //}
    }

    private void EquipHands()
    {
        //collections[3].UnloadModel();
        activeGeosets.RemoveAll(x => x > 399 && x < 500);
        //if (Items[8] != null)
        //{
        //    if (Items[8].LeftModel != "")
        //    {
        //        collections[3].Path = "item/objectcomponents/collections/";
        //        string model = Items[8].LeftModel.Substring(0, Items[8].LeftModel.Length - 7) + Suffix2;
        //        collections[3].Texture = Items[8].LeftTexture;
        //        collections[3].LoadModel(model.Replace(collections[3].Path, ""));
        //        collections[3].ActiveGeosets.Clear();
        //        if (Race != 37)
        //        {
        //            if (activeGeosets.Contains(801))
        //            {
        //                collections[3].ActiveGeosets.Add(401);
        //                collections[3].ActiveGeosets.Add(2301);
        //            }
        //        }
        //    }
        //    activeGeosets.Add(401 + Items[8].Geoset1);
        //    if (Items[8].Geoset1 != 0)
        //    {
        //        activeGeosets.RemoveAll(x => x > 799 && x < 900);
        //    }
        //}
        //else
        //{
            activeGeosets.Add(401);
        //}
    }

    private void EquipWaist()
    {
        //collections[4].UnloadModel();
        //ItemObject buckle = GameObject.Find("buckle").GetComponent<ItemObject>();
        //buckle.UnloadModel();
        activeGeosets.RemoveAll(x => x > 1799 && x < 1900);
        //if (Items[9] != null)
        //{
        //    if (Items[9].RightModel != "")
        //    {
        //        collections[4].Path = "item/objectcomponents/collections/";
        //        string model = Items[9].RightModel.Substring(0, Items[9].RightModel.Length - 7) + Suffix2;
        //        collections[4].Texture = Items[9].RightTexture;
        //        collections[4].LoadModel(model.Replace(collections[4].Path, ""));
        //        collections[4].ActiveGeosets.Clear();
        //        collections[4].ActiveGeosets.Add(1801);
        //    }
        //    if (Items[9].LeftModel != "")
        //    {
        //        buckle.LoadModel(Items[9].LeftModel);
        //        buckle.Texture = Items[9].LeftTexture;
        //        buckle.ParticleColors = Items[9].ParticleColors;
        //        buckle.Path = @"item\objectcomponents\waist\";
        //        buckle.Change = true;
        //    }
        //    activeGeosets.Add(1801 + Items[9].Geoset1);
        //    if (Items[9].Geoset1 == 1)
        //    {
        //        activeGeosets.RemoveAll(x => x > 999 && x < 1100);
        //    }
        //}
        //else
        //{
            activeGeosets.Add(1801);
        //}
    }

    private void EquipLegs()
    {
        //collections[5].UnloadModel();
        activeGeosets.RemoveAll(x => x > 1099 && x < 1200);
        activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        activeGeosets.RemoveAll(x => x > 1299 && x < 1400);
        //if (Items[10] != null)
        //{
        //    if (Items[10].LeftModel != "")
        //    {
        //        collections[5].Path = "item/objectcomponents/collections/";
        //        string model = Items[10].LeftModel.Substring(0, Items[10].LeftModel.Length - 7) + Suffix2;
        //        collections[5].Texture = Items[10].LeftTexture;
        //        collections[5].LoadModel(model.Replace(collections[5].Path, ""));
        //        collections[5].ActiveGeosets.Clear();
        //        collections[5].ActiveGeosets.Add(901);
        //        if (Race != 37)
        //        {
        //            collections[5].ActiveGeosets.Add(1101);
        //        }
        //    }
        //    activeGeosets.Add(1101 + Items[10].Geoset1);
        //    activeGeosets.Add(901 + Items[10].Geoset2);
        //    if (Items[10].Geoset1 != 3)
        //    {
        //        activeGeosets.Add(1301 + Items[10].Geoset3);
        //    }
        //    if (Items[10].UpperLeg != "")
        //    {
        //        activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //        if (demonHunter.Loaded)
        //        {
        //            demonHunter.ActiveGeosets.RemoveAll(x => x > 1399 && x < 1500);
        //        }
        //    }
        //}
        //else
        //{
            activeGeosets.Add(1101);
            activeGeosets.Add(901);
            activeGeosets.Add(1301);
        //}
    }

    private void EquipFeet()
    {
        //collections[6].UnloadModel();
        activeGeosets.RemoveAll(x => x > 499 && x < 600);
        activeGeosets.RemoveAll(x => x > 1999 && x < 2100);
        //if (Items[11] != null)
        //{
        //    if (Items[11].LeftModel != "")
        //    {
        //        collections[6].Path = "item/objectcomponents/collections/";
        //        string model = Items[11].LeftModel.Substring(0, Items[11].LeftModel.Length - 7) + Suffix2;
        //        collections[6].Texture = Items[11].LeftTexture;
        //        collections[6].LoadModel(model.Replace(collections[6].Path, ""));
        //        collections[6].ActiveGeosets.Clear();
        //        if (Race != 37)
        //        {
        //            if (!activeGeosets.Contains(1302))
        //            collections[6].ActiveGeosets.Add(501);
        //            collections[6].ActiveGeosets.Add(2001);
        //        }
        //    }
        //    if (!activeGeosets.Contains(1302))
        //    {
        //        activeGeosets.Add(501 + Items[11].Geoset1);
        //    }
        //    if (Items[11].Geoset1 != 0)
        //    {
        //        activeGeosets.RemoveAll(x => x > 899 && x < 1000);
        //    }
        //    activeGeosets.Add(2002 - Items[11].Geoset2);
        //}
        //else
        //{
            if (!activeGeosets.Contains(1302))
            {
                activeGeosets.Add(501);
            }
            activeGeosets.Add(2001);
        //}
    }

    private void EquipRightHand()
    {
        //ItemObject right = GameObject.Find("right hand").GetComponent<ItemObject>();
        //ItemObject book = GameObject.Find("book").GetComponent<ItemObject>();
        //right.UnloadModel();
        //book.UnloadModel();
        //if (Items[7] != null)
        //{
        //    if (Items[7].Slot != 15)
        //    {
        //        if (Items[7].LeftModel != "")
        //        {
        //            right.LoadModel(Items[7].LeftModel);
        //            right.Texture = Items[7].LeftTexture;
        //            right.ParticleColors = Items[7].ParticleColors;
        //            right.Path = @"item\objectcomponents\weapon\";
        //            right.Change = true;
        //        }
        //    }
        //    if (Items[7].Slot != 15 && Items[7].Slot != 26)
        //    {
        //        if (Items[7].RightModel != "")
        //        {
        //            book.LoadModel(Items[7].RightModel);
        //            book.Texture = Items[7].RightTexture;
        //            book.ParticleColors = Items[7].ParticleColors;
        //            book.Path = @"item\objectcomponents\weapon\";
        //            book.Change = true;
        //        }
        //    }
        //}
    }

    private void EquipLeftHand()
    {
        //ItemObject left = GameObject.Find("left hand").GetComponent<ItemObject>();
        //ItemObject shield = GameObject.Find("shield").GetComponent<ItemObject>();
        //ItemObject quiver = GameObject.Find("quiver").GetComponent<ItemObject>();
        //left.UnloadModel();
        //shield.UnloadModel();
        //quiver.UnloadModel();
        //if (Items[7] != null)
        //{
        //    if (Items[7].Slot == 15)
        //    {
        //        left.transform.localScale = new Vector3(1f, 1f, 1f);
        //        if (Items[7].LeftModel != "")
        //        {
        //            left.LoadModel(Items[7].LeftModel);
        //            left.Texture = Items[7].LeftTexture;
        //            left.ParticleColors = Items[7].ParticleColors;
        //            left.Path = @"item\objectcomponents\weapon\";
        //            left.Change = true;
        //        }
        //    }
        //    if (Items[7].Slot == 15 || Items[7].Slot == 26)
        //    {
        //        if (Items[7].RightModel != "")
        //        {
        //            quiver.LoadModel(Items[7].RightModel);
        //            quiver.Texture = Items[7].RightTexture;
        //            quiver.ParticleColors = Items[7].ParticleColors;
        //            quiver.Path = @"item\objectcomponents\quiver\";
        //            quiver.Change = true;
        //        }
        //    }
        //}
        //if (Items[12] != null)
        //{
        //    if (Items[12].Slot == 14)
        //    {
        //        if (Items[12].LeftModel != "")
        //        {
        //            shield.LoadModel(Items[12].LeftModel);
        //            shield.Texture = Items[12].LeftTexture;
        //            shield.ParticleColors = Items[12].ParticleColors;
        //            shield.Path = @"item\objectcomponents\shield\";
        //            shield.Change = true;
        //        }
        //    }
        //    else
        //    {
        //        if (Items[12].LeftModel != "")
        //        {
        //            if (Items[12].LeftModel.Contains("left"))
        //            {
        //                left.transform.localScale = new Vector3(1f, 1f, 1f);
        //            }
        //            else
        //            {
        //                left.transform.localScale = new Vector3(1f, 1f, -1f);
        //            }
        //            left.LoadModel(Items[12].LeftModel);
        //            left.Texture = Items[12].LeftTexture;
        //            left.ParticleColors = Items[12].ParticleColors;
        //            left.Path = @"item\objectcomponents\weapon\";
        //            left.Change = true;
        //        }
        //    }
        //}
    }

    Texture2D LoadTexture(string file, int width, int height)
    {
        Texture2D texture = Resources.Load<Texture2D>(file);
        if (texture == null)
        {
            texture = Resources.Load<Texture2D>(file.Replace("_u", Gender ? "_m" : "_f"));
        }
        if (texture == null)
        {
            return null;
        }
        return TextureScaler.scaled(texture, width, height);
    }

    public void TextureChest(Texture2D texture)
    {
        //if (Items[3] != null)
        //{
        //    if (Items[3].UpperArm != "")
        //    {
        //        Texture2D armUpper = LoadTexture(Items[3].UpperArm.Replace(".blp", ""), 256, 128);
        //        if (armUpper != null)
        //        {
        //            helper.DrawTexture(texture, armUpper, 0, 384);
        //        }
        //    }
        //    if (Items[3].LowerArm != "")
        //    {
        //        Texture2D armLower = LoadTexture(Items[3].LowerArm.Replace(".blp", ""), 256, 128);
        //        if (armLower != null)
        //        {
        //            helper.DrawTexture(texture, armLower, 0, 256);
        //        }
        //    }
        //    if (Items[3].UpperTorso != "")
        //    {
        //        Texture2D torsoUpper = LoadTexture(Items[3].UpperTorso.Replace(".blp", ""), 256, 128);
        //        if (torsoUpper != null)
        //        {
        //            helper.DrawTexture(texture, torsoUpper, 256, 384);
        //        }
        //    }
        //    if (Items[3].LowerTorso != "")
        //    {
        //        Texture2D torsoLower = LoadTexture(Items[3].LowerTorso.Replace(".blp", ""), 256, 64);
        //        if (torsoLower != null)
        //        {
        //            helper.DrawTexture(texture, torsoLower, 256, 320);
        //        }
        //    }
        //    if (Items[3].UpperLeg != "")
        //    {
        //        Texture2D legUpper = LoadTexture(Items[3].UpperLeg.Replace(".blp", ""), 256, 128);
        //        if (legUpper != null)
        //        {
        //            helper.DrawTexture(texture, legUpper, 256, 192);
        //        }
        //    }
        //    if (Items[3].LowerLeg != "")
        //    {
        //        Texture2D legLower = LoadTexture(Items[3].LowerLeg.Replace(".blp", ""), 256, 128);
        //        if (legLower != null)
        //        {
        //            helper.DrawTexture(texture, legLower, 256, 64);
        //        }
        //    }
        //}
    }

    public void TextureShirt(Texture2D texture)
    {
        //if (Items[4] != null)
        //{
        //    if (Items[4].UpperArm != "")
        //    {
        //        Texture2D armUpper = LoadTexture(Items[4].UpperArm.Replace(".blp", ""), 256, 128);
        //        if (armUpper != null)
        //        {
        //            helper.DrawTexture(texture, armUpper, 0, 384);
        //        }
        //    }
        //    if (Items[4].LowerArm != "")
        //    {
        //        Texture2D armLower = LoadTexture(Items[4].LowerArm.Replace(".blp", ""), 256, 128);
        //        if (armLower != null)
        //        {
        //            helper.DrawTexture(texture, armLower, 0, 256);
        //        }
        //    }
        //    if (Items[4].UpperTorso != "")
        //    {
        //        Texture2D torsoUpper = LoadTexture(Items[4].UpperTorso.Replace(".blp", ""), 256, 128);
        //        if (torsoUpper != null)
        //        {
        //            helper.DrawTexture(texture, torsoUpper, 256, 384);
        //        }
        //    }
        //    if (Items[4].LowerTorso != "")
        //    {
        //        Texture2D torsoLower = LoadTexture(Items[4].LowerTorso.Replace(".blp", ""), 256, 64);
        //        if (torsoLower != null)
        //        {
        //            helper.DrawTexture(texture, torsoLower, 256, 320);
        //        }
        //    }
        //}
    }

    public void TextureTabard(Texture2D texture)
    {
        //if (Items[5] != null)
        //{
        //    if (Items[5].UpperTorso != "")
        //    {
        //        Texture2D torsoUpper = LoadTexture(Items[5].UpperTorso.Replace(".blp", ""), 256, 128);
        //        if (torsoUpper != null)
        //        {
        //            helper.DrawTexture(texture, torsoUpper, 256, 384);
        //        }
        //    }
        //    if (Items[5].LowerTorso != "")
        //    {
        //        Texture2D torsoLower = LoadTexture(Items[5].LowerTorso.Replace(".blp", ""), 256, 64);
        //        if (torsoLower != null)
        //        {
        //            helper.DrawTexture(texture, torsoLower, 256, 320);
        //        }
        //    }
        //    if (Items[5].UpperLeg != "")
        //    {
        //        Texture2D legUpper = LoadTexture(Items[5].UpperLeg.Replace(".blp", ""), 256, 128);
        //        if (legUpper != null)
        //        {
        //            helper.DrawTexture(texture, legUpper, 256, 192);
        //        }
        //    }
        //}
    }

    public void TextureWrist(Texture2D texture)
    {
        //if (Items[6] != null)
        //{
        //    if (Items[6].LowerArm != "")
        //    {
        //        Texture2D armLower = LoadTexture(Items[6].LowerArm.Replace(".blp", ""), 256, 128);
        //        if (armLower != null)
        //        {
        //            helper.DrawTexture(texture, armLower, 0, 256);
        //        }
        //    }
        //}
    }

    public void TextureHands(Texture2D texture)
    {
        //if (Items[8] != null)
        //{
        //    if (Items[8].LowerArm != "")
        //    {
        //        Texture2D armLower = LoadTexture(Items[8].LowerArm.Replace(".blp", ""), 256, 128);
        //        if (armLower != null)
        //        {
        //            helper.DrawTexture(texture, armLower, 0, 256);
        //        }
        //    }
        //    if (Items[8].Hand != "")
        //    {
        //        Texture2D hand = LoadTexture(Items[8].Hand.Replace(".blp", ""), 256, 64);
        //        if (hand != null)
        //        {
        //            helper.DrawTexture(texture, hand, 0, 192);
        //        }
        //    }
        //}
    }

    public void TextureWaist(Texture2D texture)
    {
        //if (Items[9] != null)
        //{
        //    if (Items[9].LowerTorso != "")
        //    {
        //        Texture2D torsoLower = LoadTexture(Items[9].LowerTorso.Replace(".blp", ""), 256, 64);
        //        if (torsoLower != null)
        //        {
        //            helper.DrawTexture(texture, torsoLower, 256, 320);
        //        }
        //    }
        //    if (Items[9].UpperLeg != "")
        //    {
        //        Texture2D legUpper = LoadTexture(Items[9].UpperLeg.Replace(".blp", ""), 256, 128);
        //        if (legUpper != null)
        //        {
        //            helper.DrawTexture(texture, legUpper, 256, 192);
        //        }
        //    }
        //}
    }

    public void TextureLegs(Texture2D texture)
    {
        //if (Items[10] != null)
        //{
        //    if (Items[10].UpperLeg != "")
        //    {
        //        Texture2D legUpper = LoadTexture(Items[10].UpperLeg.Replace(".blp", ""), 256, 128);
        //        if (legUpper != null)
        //        {
        //            helper.DrawTexture(texture, legUpper, 256, 192);
        //        }
        //    }
        //    if (Items[10].LowerLeg != "")
        //    {
        //        Texture2D legLower = LoadTexture(Items[10].LowerLeg.Replace(".blp", ""), 256, 128);
        //        if (legLower != null)
        //        {
        //            helper.DrawTexture(texture, legLower, 256, 64);
        //        }
        //    }
        //}
    }

    public void TextureFeet(Texture2D texture, bool showFeet = false)
    {
        //if (Items[11] != null)
        //{
        //    if (Items[11].LowerLeg != "" && !activeGeosets.Contains(1302))
        //    {
        //        Texture2D legLower = LoadTexture(Items[11].LowerLeg.Replace(".blp", ""), 256, 128);
        //        if (legLower != null)
        //        {
        //            helper.DrawTexture(texture, legLower, 256, 64);
        //        }
        //    }
        //    if (Items[11].Foot != "" && !showFeet)
        //    {
        //        Texture2D foot = LoadTexture(Items[11].Foot.Replace(".blp", ""), 256, 64);
        //        if (foot != null)
        //        {
        //            helper.DrawTexture(texture, foot, 256, 0);
        //        }
        //    }
        //}
    }

    public void BlackChest(Texture2D texture)
    {
        //if (Items[3] != null)
        //{
        //    if (Items[3].UpperArm != "")
        //    {
        //        Texture2D armUpper = LoadTexture(Items[3].UpperArm.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(armUpper, armUpper);
        //        armUpper.Apply();
        //        helper.DrawTexture(texture, armUpper, 0, 384);
        //    }
        //    if (Items[3].LowerArm != "")
        //    {
        //        Texture2D armLower = LoadTexture(Items[3].LowerArm.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(armLower, armLower);
        //        armLower.Apply();
        //        helper.DrawTexture(texture, armLower, 0, 256);
        //    }
        //    if (Items[3].UpperTorso != "")
        //    {
        //        Texture2D torsoUpper = LoadTexture(Items[3].UpperTorso.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(torsoUpper, torsoUpper);
        //        torsoUpper.Apply();
        //        helper.DrawTexture(texture, torsoUpper, 256, 384);
        //    }
        //    if (Items[3].LowerTorso != "")
        //    {
        //        Texture2D torsoLower = LoadTexture(Items[3].LowerTorso.Replace(".blp", ""), 256, 64);
        //        helper.BlackTexture(torsoLower, torsoLower);
        //        torsoLower.Apply();
        //        helper.DrawTexture(texture, torsoLower, 256, 320);
        //    }
        //    if (Items[3].UpperLeg != "")
        //    {
        //        Texture2D legUpper = LoadTexture(Items[3].UpperLeg.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(legUpper, legUpper);
        //        legUpper.Apply();
        //        helper.DrawTexture(texture, legUpper, 256, 192);
        //    }
        //    if (Items[3].LowerLeg != "")
        //    {
        //        Texture2D legLower = LoadTexture(Items[3].LowerLeg.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(legLower, legLower);
        //        legLower.Apply();
        //        helper.DrawTexture(texture, legLower, 256, 64);
        //    }
        //}
    }

    public void BlackShirt(Texture2D texture)
    {
        //if (Items[4] != null)
        //{
        //    if (Items[4].UpperArm != "")
        //    {
        //        Texture2D armUpper = LoadTexture(Items[4].UpperArm.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(armUpper, armUpper);
        //        armUpper.Apply();
        //        helper.DrawTexture(texture, armUpper, 0, 384);
        //    }
        //    if (Items[4].LowerArm != "")
        //    {
        //        Texture2D armLower = LoadTexture(Items[4].LowerArm.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(armLower, armLower);
        //        armLower.Apply();
        //        helper.DrawTexture(texture, armLower, 0, 256);
        //    }
        //    if (Items[4].UpperTorso != "")
        //    {
        //        Texture2D torsoUpper = LoadTexture(Items[4].UpperTorso.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(torsoUpper, torsoUpper);
        //        torsoUpper.Apply();
        //        helper.DrawTexture(texture, torsoUpper, 256, 384);
        //    }
        //    if (Items[4].LowerTorso != "")
        //    {
        //        Texture2D torsoLower = LoadTexture(Items[4].LowerTorso.Replace(".blp", ""), 256, 64);
        //        helper.BlackTexture(torsoLower, torsoLower);
        //        torsoLower.Apply();
        //        helper.DrawTexture(texture, torsoLower, 256, 320);
        //    }
        //}
    }

    public void BlackTabard(Texture2D texture)
    {
        //if (Items[5] != null)
        //{
        //    if (Items[5].UpperTorso != "")
        //    {
        //        Texture2D torsoUpper = LoadTexture(Items[5].UpperTorso.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(torsoUpper, torsoUpper);
        //        torsoUpper.Apply();
        //        helper.DrawTexture(texture, torsoUpper, 256, 384);
        //    }
        //    if (Items[5].LowerTorso != "")
        //    {
        //        Texture2D torsoLower = LoadTexture(Items[5].LowerTorso.Replace(".blp", ""), 256, 64);
        //        helper.BlackTexture(torsoLower, torsoLower);
        //        torsoLower.Apply();
        //        helper.DrawTexture(texture, torsoLower, 256, 320);
        //    }
        //    if (Items[5].UpperLeg != "")
        //    {
        //        Texture2D legUpper = LoadTexture(Items[5].UpperLeg.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(legUpper, legUpper);
        //        legUpper.Apply();
        //        helper.DrawTexture(texture, legUpper, 256, 192);
        //    }
        //}
    }

    public void BlackWrist(Texture2D texture)
    {
        //if (Items[6] != null)
        //{
        //    if (Items[6].LowerArm != "")
        //    {
        //        Texture2D armLower = LoadTexture(Items[6].LowerArm.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(armLower, armLower);
        //        armLower.Apply();
        //        helper.DrawTexture(texture, armLower, 0, 256);
        //    }
        //}
    }

    public void BlackHands(Texture2D texture)
    {
        //if (Items[8] != null)
        //{
        //    if (Items[8].LowerArm != "")
        //    {
        //        Texture2D armLower = LoadTexture(Items[8].LowerArm.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(armLower, armLower);
        //        armLower.Apply();
        //        helper.DrawTexture(texture, armLower, 0, 256);
        //    }
        //    if (Items[8].Hand != "")
        //    {
        //        Texture2D hand = LoadTexture(Items[8].Hand.Replace(".blp", ""), 256, 64);
        //        helper.BlackTexture(hand, hand);
        //        hand.Apply();
        //        helper.DrawTexture(texture, hand, 0, 192);
        //    }
        //}
    }

    public void BlackWaist(Texture2D texture)
    {
        //if (Items[9] != null)
        //{
        //    if (Items[9].LowerTorso != "")
        //    {
        //        Texture2D torsoLower = LoadTexture(Items[9].LowerTorso.Replace(".blp", ""), 256, 64);
        //        helper.BlackTexture(torsoLower, torsoLower);
        //        torsoLower.Apply();
        //        helper.DrawTexture(texture, torsoLower, 256, 320);
        //    }
        //    if (Items[9].UpperLeg != "")
        //    {
        //        Texture2D legUpper = LoadTexture(Items[9].UpperLeg.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(legUpper, legUpper);
        //        legUpper.Apply();
        //        helper.DrawTexture(texture, legUpper, 256, 192);
        //    }
        //}
    }

    public void BlackLegs(Texture2D texture)
    {
        //if (Items[10] != null)
        //{
        //    if (Items[10].UpperLeg != "")
        //    {
        //        Texture2D legUpper = LoadTexture(Items[10].UpperLeg.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(legUpper, legUpper);
        //        legUpper.Apply();
        //        helper.DrawTexture(texture, legUpper, 256, 192);
        //    }
        //    if (Items[10].LowerLeg != "")
        //    {
        //        Texture2D legLower = LoadTexture(Items[10].LowerLeg.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(legLower, legLower);
        //        legLower.Apply();
        //        helper.DrawTexture(texture, legLower, 256, 64);
        //    }
        //}
    }

    public void BlackFeet(Texture2D texture, bool showFeet = false)
    {
        //if (Items[11] != null)
        //{
        //    if (Items[11].LowerLeg != "")
        //    {
        //        Texture2D legLower = LoadTexture(Items[11].LowerLeg.Replace(".blp", ""), 256, 128);
        //        helper.BlackTexture(legLower, legLower);
        //        legLower.Apply();
        //        helper.DrawTexture(texture, legLower, 256, 64);
        //    }
        //    if (Items[11].Foot != "" && !showFeet)
        //    {
        //        Texture2D foot = LoadTexture(Items[11].Foot.Replace(".blp", ""), 256, 64);
        //        helper.BlackTexture(foot, foot);
        //        foot.Apply();
        //        helper.DrawTexture(texture, foot, 256, 0);
        //    }
        //}
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

    public void InitializeHelper(CASCHandler casc)
    {
        if (Gender)
        {
            switch (Race)
            {
                case 1:
                    helper = new HumanMale(Model, this, casc);
                    break;
                case 2:
                    helper = new OrcMale(Model, this, casc);
                    break;
                case 3:
                    helper = new DwarfMale(Model, this, casc);
                    break;
                case 4:
                    helper = new NightElfMale(Model, this, casc);
                    break;
                case 5:
                    helper = new UndeadMale(Model, this, casc);
                    break;
                case 6:
                    helper = new TaurenMale(Model, this, casc);
                    break;
                case 7:
                    helper = new GnomeMale(Model, this, casc);
                    break;
                case 8:
                    helper = new TrollMale(Model, this, casc);
                    break;
                case 9:
                    helper = new GoblinMale(Model, this, casc);
                    break;
                case 10:
                    helper = new BloodElfMale(Model, this, casc);
                    break;
                case 11:
                    helper = new DraeneiMale(Model, this, casc);
                    break;
                case 22:
                    helper = new WorgenMale(Model, this, casc);
                    break;
                case 24:
                    helper = new PandarenMale(Model, this, casc);
                    break;
                case 27:
                    helper = new NightborneMale(Model, this, casc);
                    break;
                //case 28:
                //    helper = new HighmountainMale(Model, this, casc);
                //    break;
                //case 29:
                //    helper = new VoidElfMale(Model, this, casc);
                //    break;
                //case 30:
                //    helper = new LightforgedMale(Model, this, casc);
                //    break;
                //case 31:
                //    helper = new ZandalariMale(Model, this, casc);
                //    break;
                //case 32:
                //    helper = new KulTiranMale(Model, this, casc);
                //    break;
                //case 34:
                //    helper = new DarkIronMale(Model, this, casc);
                //    break;
                //case 35:
                //    helper = new VulperaMale(Model, this, casc);
                //    break;
                //case 36:
                //    helper = new MagharMale(Model, this, casc);
                //    break;
                //case 37:
                //    helper = new MechagnomeMale(Model, this, casc);
                //    break;
            }
        }
        else
        {
            switch (Race)
            {
                case 1:
                    helper = new HumanFemale(Model, this, casc);
                    break;
                case 2:
                    helper = new OrcFemale(Model, this, casc);
                    break;
                case 3:
                    helper = new DwarfFemale(Model, this, casc);
                    break;
                case 4:
                    helper = new NightElfFemale(Model, this, casc);
                    break;
                case 5:
                    helper = new UndeadFemale(Model, this, casc);
                    break;
                case 6:
                    helper = new TaurenFemale(Model, this, casc);
                    break;
                case 7:
                    helper = new GnomeFemale(Model, this, casc);
                    break;
                case 8:
                    helper = new TrollFemale(Model, this, casc);
                    break;
                case 9:
                    helper = new GoblinFemale(Model, this, casc);
                    break;
                case 10:
                    helper = new BloodElfFemale(Model, this, casc);
                    break;
                case 11:
                    helper = new DraeneiFemale(Model, this, casc);
                    break;
                case 22:
                    helper = new WorgenFemale(Model, this, casc);
                    break;
                case 24:
                    helper = new PandarenFemale(Model, this, casc);
                    break;
                //case 27:
                //    helper = new NightborneFemale(Model, this, casc);
                //    break;
                //case 28:
                //    helper = new HighmountainFemale(Model, this, casc);
                //    break;
                //case 29:
                //    helper = new VoidElfFemale(Model, this, casc);
                //    break;
                //case 30:
                //    helper = new LightforgedFemale(Model, this, casc);
                //    break;
                //case 31:
                //    helper = new ZandalariFemale(Model, this, casc);
                //    break;
                //case 32:
                //    helper = new KulTiranFemale(Model, this, casc);
                //    break;
                //case 34:
                //    helper = new DarkIronFemale(Model, this, casc);
                //    break;
                //case 35:
                //    helper = new VulperaFemale(Model, this, casc);
                //    break;
                //case 36:
                //    helper = new MagharFemale(Model, this, casc);
                //    break;
                //case 37:
                //    helper = new MechagnomeFemale(Model, this, casc);
                //    break;
            }
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
        //if (Race == 37)
        //{
        //    racial.Path = modelsPath + RacePath;
        //    racial.LoadModel(RacialCollection);
        //}
        if (done)
        {
            LoadColors();
            yield return null;
            textures = new Texture2D[Model.Textures.Length];
            InitializeHelper(casc);
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

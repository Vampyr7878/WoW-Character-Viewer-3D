using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle highmountain male customization
    public class HighmountainMale : CharacterHelper
    {
        private int[] wraps;
        private int[] decoration;

        public HighmountainMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horn Wraps");
            wraps = new int[character.Choices[index].Length];
            index = Array.FindIndex(Character.Options, o => o.Name == "Horn Decoration");
            decoration = new int[character.Choices[index].Length];
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeEars(activeGeosets);
            ChangeForemane(activeGeosets);
            ChangeHair(activeGeosets);
            ChangeBeard(activeGeosets);
            ChangeEyeColor(activeGeosets);
            ChangeHeaddress(activeGeosets);
            ChangeHornStyle(activeGeosets);
            ChangeHornWraps(activeGeosets);
            ChangeHornDecoration(activeGeosets);
            ChangeFeather(activeGeosets);
            ChangeNosePiercing(activeGeosets);
            ChangeTail(activeGeosets);
            ChangeTailDecoration(activeGeosets);
        }

        private void ChangeEars(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 699 && x < 800);
            activeGeosets.Add(702);
        }

        private void ChangeForemane(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Foremane");
            activeGeosets.RemoveAll(x => x > 0 && x < 100);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeHair(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair");
            activeGeosets.RemoveAll(x => x > 199 && x < 300);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeBeard(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Beard");
            activeGeosets.RemoveAll(x => x > 99 && x < 200);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeEyeColor(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 3299 && x < 3400);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset2);
        }

        private void ChangeHeaddress(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Headdress");
            activeGeosets.RemoveAll(x => x > 3699 && x < 3800);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeHornStyle(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horn Style");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Horn Wraps");
            int index3 = Array.FindIndex(Character.Options, o => o.Name == "Horn Decoration");
            activeGeosets.RemoveAll(x => x > 2399 && x < 2500);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            GetHornWraps(index);
            if (wraps[Character.Customization[index2]] == 0)
            {
                Character.CustomizationDropdowns[index2].value = 0;
            }
            Character.ChangeDropdown(index2, wraps);
            GetHornDecoration(index3, index, index2);
            if (decoration[Character.Customization[index3]] == 0)
            {
                Character.CustomizationDropdowns[index3].value = 0;
            }
            Character.ChangeDropdown(index3, decoration);
        }

        private void ChangeHornWraps(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horn Wraps");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Horn Style");
            activeGeosets.RemoveAll(x => x > 4199 && x < 4300);
            if (Character.Choices[index2][Character.Customization[index2]].Model == 5)
            {
                activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset2);
            }
            else
            {
                activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            }
        }

        private void ChangeHornDecoration(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horn Decoration");
            activeGeosets.RemoveAll(x => x > 4299 && x < 4400);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeFeather(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Feather");
            activeGeosets.RemoveAll(x => x > 3899 && x < 4000);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeNosePiercing(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Nose Piercing");
            activeGeosets.RemoveAll(x => x > 1599 && x < 1700);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeTail(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Tail");
            activeGeosets.RemoveAll(x => x > 3799 && x < 3900);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeTailDecoration(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Tail Decoration");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Tail");
            activeGeosets.RemoveAll(x => x > 3999 && x < 4100);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[Character.Customization[index2]].Geoset1);
        }

        private void GetHornWraps(int index)
        {
            int flag = Character.Choices[index][Character.Customization[index]].Model;
            for (int i = 0; i < wraps.Length; i++)
            {
                wraps[i] = (flag & 1);
                flag >>= 1;
            }
        }

        private void GetHornDecoration(int decor, int style, int wrap)
        {
            for (int i = 0; i < decoration.Length; i++)
            {
                if (Character.Customization[wrap] > 0 && Character.Choices[style][Character.Customization[style]].Model != 5 && Character.Choices[decor][i].Bone > 0)
                {
                    decoration[i] = 1;
                }
                else
                {
                    decoration[i] = 0;
                }
            }
            decoration[0] = 1;
            decoration[Character.Choices[style][Character.Customization[style]].Bone] = 1;
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            int index, index2;
            index = Array.FindIndex(Character.Options, o => o.Name == "Face");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            Texture2D face = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
            DrawTexture(texture, face, 512, 0);
            index = Array.FindIndex(Character.Options, o => o.Name == "Body Paint");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Body Paint Color");
            if (Character.Choices[index2][Character.Customization[index2]].Textures[Character.Customization[index]].Texture1 >= 0)
            {
                Texture2D tattoo = Character.TextureFromBLP(Character.Choices[index2][Character.Customization[index2]].Textures[Character.Customization[index]].Texture1);
                DrawTexture(texture, tattoo, 0, 0);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            if (!(Character.Items[3] != null && Character.Items[3].UpperLeg !> 0) && Character.Items[10] == null)
            {
                Texture2D underwear = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture3);
                DrawTexture(texture, underwear, 256, 192);
            }
            Character.TextureShirt(texture);
            if (!(Character.Items[4] != null && Character.Items[4].Geoset1 != 0))
            {
                Character.TextureWrist(texture);
            }
            Character.TextureLegs(texture);
            Character.TextureFeet(texture, true);
            Character.TextureChest(texture);
            if (!(Character.Items[3] != null && Character.Items[3].Geoset1 != 0))
            {
                Character.TextureWrist(texture);
            }
            Character.TextureHands(texture);
            if (!(Character.Items[8] != null && Character.Items[8].Geoset1 != 0))
            {
                Character.TextureChest(texture);
            }
            Character.TextureTabard(texture);
            Character.TextureWaist(texture);
        }

        protected override int LoadTexture(M2Texture texture, int i, out bool skin)
        {
            int file = -1;
            int index, index2;
            skin = false;
            switch (texture.Type)
            {
                case 0:
                    file = Model.TextureIDs[i];
                    break;
                case 1:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
                    file = Character.Choices[index][Character.Customization[index]].Textures[0].Texture1;
                    skin = true;
                    break;
                case 2:
                    file = Character.Items[2] != null ? Character.Items[2].LeftTexture : -1;
                    break;
                case 6:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Horn Color");
                    index2 = Array.FindIndex(Character.Options, o => o.Name == "Horn Markings");
                    file = Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1;
                    break;
                case 8:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
                    file = Character.Choices[index][Character.Customization[index]].Textures[0].Texture2;
                    break;
                case 19:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
                    file = Character.Choices[index][Character.Customization[index]].Textures[0].Texture1;
                    break;
                case 20:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Jewelry Color");
                    file = Character.Choices[index][Character.Customization[index]].Textures[0].Texture1;
                    break;
            }
            return file;
        }
    }
}

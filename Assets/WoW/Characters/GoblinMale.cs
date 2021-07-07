using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle goblin male customization
    public class GoblinMale : CharacterHelper
    {
        private int[] nose;
        private int[] ears;

        public GoblinMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            int index = Array.FindIndex(Character.Options, o => o.Name == "Nose Ring");
            nose = new int[character.Choices[index].Length];
            index = Array.FindIndex(Character.Options, o => o.Name == "Earrings");
            ears = new int[character.Choices[index].Length];
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeSkinColor();
            ChangeHairStyle(activeGeosets);
            ChangeEyeColor(activeGeosets);
            ChangeChin(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeEarrings(activeGeosets);
            ChangeNose(activeGeosets);
            ChangeNoseRing(activeGeosets);
        }

        private void ChangeSkinColor()
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Face");
            if (Character.Choices[index2][Character.Customization[index2]].Textures[Character.Customization[index]].Texture1 == -1)
            {
                for (int i = 0; i < Character.Choices[index2].Length; i++)
                {
                    if (Character.Choices[index2][i].Textures[Character.Customization[index]].Texture1 >= 0)
                    {
                        Character.CustomizationDropdowns[index2].SetValue(i);
                        break;
                    }
                }
            }
            Character.ChangeFaceDropdown(index, index2);
        }

        private void ChangeHairStyle(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            activeGeosets.RemoveAll(x => x > 0 && x < 100);
            activeGeosets.Add(HideHair ? Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset2 : Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeEyeColor(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 3299 && x < 3400);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset2);
        }

        private void ChangeChin(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Chin");
            activeGeosets.RemoveAll(x => x > 3999 && x < 4100);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeEars(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Ears");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Earrings");
            activeGeosets.RemoveAll(x => x > 699 && x < 800);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            GetEarrings(index);
            if (ears[Character.Customization[index2]] == 0)
            {
                Character.CustomizationDropdowns[index2].value = 0;
            }
            Character.ChangeDropdown(index2, ears);
        }

        private void ChangeEarrings(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Earrings");
            activeGeosets.RemoveAll(x => x > 3499 && x < 3600);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeNose(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Nose");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Nose Ring");
            activeGeosets.RemoveAll(x => x > 4099 && x < 4200);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            GetNoseRings(index);
            if (nose[Character.Customization[index2]] == 0)
            {
                Character.CustomizationDropdowns[index2].value = 0;
            }
            Character.ChangeDropdown(index2, nose);
        }

        private void ChangeNoseRing(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Nose Ring");
            activeGeosets.RemoveAll(x => x > 1599 && x < 1700);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void GetEarrings(int index)
        {
            int flag = Character.Choices[index][Character.Customization[index]].Bone;
            for (int i = 0; i < ears.Length; i++)
            {
                ears[i] = (flag & 1);
                flag >>= 1;
            }
        }

        private void GetNoseRings(int index)
        {
            int flag = Character.Choices[index][Character.Customization[index]].Bone;
            for (int i = 0; i < nose.Length; i++)
            {
                nose[i] = (flag & 1);
                flag >>= 1;
            }
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            int index, index2;
            index = Array.FindIndex(Character.Options, o => o.Name == "Face");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            if (Character.Class == 6)
            {
                Texture2D face = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture2);
                DrawTexture(texture, face, 512, 0);
            }
            else
            {
                Texture2D face = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
                DrawTexture(texture, face, 512, 0);
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
            Character.TextureFeet(texture);
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
            int index;
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
                    index = Array.FindIndex(Character.Options, o => o.Name == "Hair Color");
                    file = Character.Choices[index][Character.Customization[index]].Textures[0].Texture1;
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

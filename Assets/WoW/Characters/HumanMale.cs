using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle human male customization
    public class HumanMale : CharacterHelper
    {
        public HumanMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeSkinColor();
            ChangeEars(activeGeosets);
            ChangeFaceShape(activeGeosets);
            ChangeHairStyle(activeGeosets);
            ChangeMustache(activeGeosets);
            ChangeBeard(activeGeosets);
            ChangeSideburns(activeGeosets);
            ChangeEyebrows(activeGeosets);
            ChangeEyeColor(activeGeosets);
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

        private void ChangeEars(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 699 && x < 800);
            activeGeosets.Add(702);
        }

        private void ChangeFaceShape(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Face Shape");
            activeGeosets.RemoveAll(x => x > 3199 && x < 3300);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeHairStyle(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Hair Color");
            activeGeosets.RemoveAll(x => x > 0 && x < 100);
            activeGeosets.Add(HideHair ? Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset2 : Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            int[] colors = new int[Character.Choices[index2].Length];
            switch (Character.Choices[index][Character.Customization[index]].Bone)
            {
                case 0:
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = Character.Choices[index2][i].Geosets[0].Geoset1;
                    }
                    break;
                case 1:
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = Character.Choices[index2][i].Geosets[0].Geoset2;
                    }
                    break;
                case 2:
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = Character.Choices[index2][i].Geosets[0].Geoset3;
                    }
                    break;
            }
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == -1)
                {
                    Character.Choices[index2][i].Color1 = Color.clear;
                }
                else
                {
                    Color c;
                    ColorUtility.TryParseHtmlString($"#{colors[i].ToString("X8").Substring(2)}", out c);
                    Character.Choices[index2][i].Color1 = c;
                }
            }
            Character.input.SetCustomizationNamesAndColors(index2);
        }

        private void ChangeMustache(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Mustache");
            activeGeosets.RemoveAll(x => x > 299 && x < 400);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeBeard(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Beard");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Sideburns");
            activeGeosets.RemoveAll(x => x > 99 && x < 200);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            if (Character.Customization[index2] > Character.Choices[index][Character.Customization[index]].Bone)
            {
                Character.CustomizationDropdowns[index2].SetValue(Character.Choices[index][Character.Customization[index]].Bone);
            }
            Character.ChangeDropdown(index, index2);
        }

        private void ChangeSideburns(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Sideburns");
            activeGeosets.RemoveAll(x => x > 199 && x < 300);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeEyebrows(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Eyebrows");
            activeGeosets.RemoveAll(x => x > 3399 && x < 3500);
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

        protected override void LayeredTexture(Texture2D texture)
        {
            int index, index2;
            index = Array.FindIndex(Character.Options, o => o.Name == "Face");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            Texture2D face = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
            DrawTexture(texture, face, 512, 0);
            index = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            if (Character.Choices[index][Character.Customization[index]].Textures[0].Texture2 >= 0)
            {
                Texture2D texture2 = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture2);
                OverlayTexture(texture, texture2, 0, 0);
            }
            if (!(Character.Items[3] != null && Character.Items[3].UpperLeg !> 0) && Character.Items[10] == null)
            {
                Texture2D underwear = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture3);
                DrawTexture(texture, underwear, 256, 192);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
            if (Character.Choices[index][Character.Customization[index]].Textures[0].Texture2 >= 0)
            {
                Texture2D eyeglow = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture2);
                DrawTexture(texture, eyeglow, 512, 0, 0.5f);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Hair Color");
            if (Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1 >= 0)
            {
                Texture2D scalp = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
                DrawTexture(texture, scalp, 512, 0);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Eyebrows");
            if (Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1 >= 0)
            {
                Texture2D facial = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
                DrawTexture(texture, facial, 512, 0);
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
                    index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
                    index2 = Array.FindIndex(Character.Options, o => o.Name == "Hair Color");
                    switch (Character.Choices[index][Character.Customization[index]].Bone)
                    {
                        case 0:
                            file = Character.Choices[index2][Character.Customization[index2]].Textures[0].Texture1;
                            break;
                        case 1:
                            file = Character.Choices[index2][Character.Customization[index2]].Textures[0].Texture2;
                            break;
                        case 2:
                            file = Character.Choices[index2][Character.Customization[index2]].Textures[0].Texture3;
                            break;
                    }
                    break;
                case 19:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
                    file = Character.Choices[index][Character.Customization[index]].Textures[0].Texture1;
                    break;
            }
            return file;
        }
    }
}

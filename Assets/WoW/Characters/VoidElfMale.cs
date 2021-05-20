using CASCLib;
using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle void elf male customization
    public class VoidElfMale : CharacterHelper
    {
        public VoidElfMale(M2 model, Character character, CASCHandler casc)
        {
            Model = model;
            Character = character;
            converter = new System.Drawing.ImageConverter();
            this.casc = casc;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeSkinColor();
            ChangeEars(activeGeosets);
            ChangeHairStyle(activeGeosets);
            ChangeFacialHair(activeGeosets);
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

        private void ChangeHairStyle(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            activeGeosets.RemoveAll(x => x > 0 && x < 100);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeFacialHair(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Facial Hair");
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

        protected override void LayeredTexture(Texture2D texture)
        {
            int index, index2;
            index = Array.FindIndex(Character.Options, o => o.Name == "Face");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            Texture2D face = TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
            DrawTexture(texture, face, 512, 0);
            index = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            //if (!(Character.Items[3] != null && Character.Items[3].UpperLeg != "") && Character.Items[10] == null)
            //{
                Texture2D underwear = TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture3);
                DrawTexture(texture, underwear, 256, 192);
            //}
            //Character.TextureShirt(texture);
            //if (!(Character.Items[4] != null && Character.Items[4].Geoset1 != 0))
            //{
            //    Character.TextureWrist(texture);
            //}
            //Character.TextureLegs(texture);
            //Character.TextureFeet(texture);
            //Character.TextureChest(texture);
            //if (!(Character.Items[3] != null && Character.Items[3].Geoset1 != 0))
            //{
            //    Character.TextureWrist(texture);
            //}
            //Character.TextureHands(texture);
            //if (!(Character.Items[8] != null && Character.Items[8].Geoset1 != 0))
            //{
            //    Character.TextureChest(texture);
            //}
            //Character.TextureTabard(texture);
            //Character.TextureWaist(texture);
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
                //case 2:
                //    if (Character.Items[2] != null)
                //    {
                //        file = Character.Items[2].LeftTexture;
                //    }
                //    else
                //    {
                //        file = "";
                //    }
                //    break;
                case 6:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Hair Color");
                    file = Character.Choices[index][Character.Customization[index]].Textures[0].Texture1;
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

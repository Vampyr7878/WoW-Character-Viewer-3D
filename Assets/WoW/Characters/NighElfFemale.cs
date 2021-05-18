using CASCLib;
using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle night elf female customization
    public class NightElfFemale : CharacterHelper
    {
        public int[] eyes;

        public NightElfFemale(M2 model, Character character, CASCHandler casc)
        {
            Model = model;
            Character = character;
            converter = new System.Drawing.ImageConverter();
            this.casc = casc;
            int index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
            eyes = new int[character.Choices[index].Length];
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeSkinColor();
            ChangeUnderwear(Character.demonHunter.ActiveGeosets);
            ChangeHairStyle(activeGeosets);
            ChangeVines(activeGeosets);
            ChangeEyebrows(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeBlindfold(Character.demonHunter.ActiveGeosets);
            ChangeScars();
            ChangeEyeColor(activeGeosets);
            ChangeHeaddress(activeGeosets);
            ChangeEarrings(activeGeosets);
            ChangeNoseRing(activeGeosets);
            ChangeNecklace(activeGeosets);
            ChangeHorns(Character.demonHunter.ActiveGeosets);
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

        private void ChangeUnderwear(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
            if (Character.Class == 12)
            {
                activeGeosets.Add(1401);
            }
            else
            {
                activeGeosets.Add(1400);
            }
        }

        private void ChangeHairStyle(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            activeGeosets.RemoveAll(x => x > 0 && x < 100);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeVines(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Vines");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            activeGeosets.RemoveAll(x => x > 3899 && x < 4000);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[Character.Customization[index2]].Geoset1);
        }

        private void ChangeEyebrows(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Eyebrows");
            activeGeosets.RemoveAll(x => x > 3399 && x < 3500);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeEars(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Ears");
            activeGeosets.RemoveAll(x => x > 699 && x < 800);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeBlindfold(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Blindfold");
            activeGeosets.RemoveAll(x => x > 2499 && x < 2600);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeScars()
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Scars");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
            GetEyeColors(index);
            if (eyes[Character.Customization[index2]] == 0)
            {
                Character.CustomizationDropdowns[index2].SetValue(0);
            }
            Character.ChangeDropdown(index2, eyes);
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

        private void ChangeEarrings(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Earrings");
            activeGeosets.RemoveAll(x => x > 3499 && x < 3600);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeNoseRing(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Nose Ring");
            activeGeosets.RemoveAll(x => x > 1599 && x < 1700);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeNecklace(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Necklace");
            activeGeosets.RemoveAll(x => x > 3599 && x < 3700);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeHorns(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horns");
            activeGeosets.RemoveAll(x => x > 2399 && x < 2500);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void GetEyeColors(int index)
        {
            int flag = Character.Choices[index][Character.Customization[index]].Bone;
            for (int i = 0; i < eyes.Length; i++)
            {
                eyes[i] = (flag & 1);
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
                Texture2D face = TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture2);
                DrawTexture(texture, face, 512, 0);
            }
            else
            {
                Texture2D face = TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
                DrawTexture(texture, face, 512, 0);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Markings");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Markings Color");
            if (Character.Choices[index2][Character.Customization[index2]].Textures[Character.Customization[index]].Texture1 >= 0)
            {
                Texture2D facial = TextureFromBLP(Character.Choices[index2][Character.Customization[index2]].Textures[Character.Customization[index]].Texture1);
                DrawTexture(texture, facial, 512, 0);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Tattoo");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Tattoo Color");
            if (Character.Choices[index2][Character.Customization[index2]].Textures[Character.Customization[index]].Texture1 >= 0)
            {
                Texture2D tattoo = TextureFromBLP(Character.Choices[index2][Character.Customization[index2]].Textures[Character.Customization[index]].Texture1);
                DrawTexture(texture, tattoo, 0, 0);
                Texture2D temp = Resources.Load<Texture2D>("Materials/Emission");
                Emission = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
                Emission.SetPixels32(temp.GetPixels32());
                tattoo = TextureFromBLP(Character.Choices[index2][Character.Customization[index2]].Textures[Character.Customization[index]].Texture2);
                DrawTexture(Emission, tattoo, 0, 0);
            }
            else
            {
                Emission = null;
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Scars");
            if (Character.Choices[index][Character.Customization[index]].Textures[0].Texture1 >= 0)
            {
                Texture2D scars = TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture1);
                OverlayTexture(texture, scars, 512, 0);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            Texture2D bra = null;
            //if (Character.Items[3] == null && Character.Items[4] == null && Character.Items[5] == null)
            //{
                bra = TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture4);
                DrawTexture(texture, bra, 256, 384);
            //}
            Texture2D underwear = null;
            //if (!(Character.Items[3] != null && Character.Items[3].UpperLeg != "") && Character.Items[10] == null)
            //{
                underwear = TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture3);
                DrawTexture(texture, underwear, 256, 192);
            //}
            if (Emission != null)
            {
                if (bra != null)
                {
                    Texture2D temp = new Texture2D(bra.width, bra.height, TextureFormat.ARGB32, false);
                    BlackTexture(bra, temp);
                    temp.Apply();
                    DrawTexture(Emission, temp, 256, 384);
                }
                if (underwear != null)
                {
                    Texture2D temp = new Texture2D(underwear.width, underwear.height, TextureFormat.ARGB32, false);
                    BlackTexture(underwear, temp);
                    temp.Apply();
                    DrawTexture(Emission, temp, 256, 192);
                }
                //Character.BlackChest(Emission);
                //Character.BlackShirt(Emission);
                //Character.BlackTabard(Emission);
                //Character.BlackWrist(Emission);
                //Character.BlackHands(Emission);
                //Character.BlackWaist(Emission);
                //Character.BlackLegs(Emission);
                //Character.BlackFeet(Emission);
                Emission.Apply();
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
            if (Character.Choices[index][Character.Customization[index]].Textures[0].Texture2 >= 0)
            {
                Texture2D eyeglow = TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture2);
                DrawTexture(texture, eyeglow, 512, 0, 0.5f);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Hair Color");
            if (Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1 >= 0)
            {
                Texture2D scalp = TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
                DrawTexture(texture, scalp, 512, 0);
            }
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
                    index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
                    index2 = Array.FindIndex(Character.Options, o => o.Name == "Hair Color");
                    file = Character.Choices[index2][Character.Customization[index2]].Textures[0].Texture1;
                    break;
                case 10:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Vine Color");
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

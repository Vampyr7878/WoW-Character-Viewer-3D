using CASCLib;
using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle mag'har male customization
    public class MagharMale : CharacterHelper
    {
        public MagharMale(M2 model, Character character, CASCHandler casc)
        {
            Model = model;
            Character = character;
            converter = new System.Drawing.ImageConverter();
            this.casc = casc;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeHairStyle(activeGeosets);
            ChangeBeard(activeGeosets);
            ChangeSideburns(activeGeosets);
            ChangeEyeColor(activeGeosets);
            ChangeEarrings(activeGeosets);
            ChangePiercings(activeGeosets);
            ChangeTusks(activeGeosets);
            ChangeHunched();
        }

        private void ChangeHairStyle(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            activeGeosets.RemoveAll(x => x > 0 && x < 100);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeBeard(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Beard");
            activeGeosets.RemoveAll(x => x > 99 && x < 200);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeSideburns(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Sideburns");
            activeGeosets.RemoveAll(x => x > 199 && x < 300);
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

        private void ChangeEarrings(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Earrings");
            activeGeosets.RemoveAll(x => x > 699 && x < 800);
            activeGeosets.RemoveAll(x => x > 3499 && x < 3600);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset2);
        }

        private void ChangePiercings(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Piercings");
            activeGeosets.RemoveAll(x => x > 1599 && x < 1700);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeTusks(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Tusks");
            activeGeosets.RemoveAll(x => x > 3999 && x < 4100);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeHunched()
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hunched");
            bool upright = Character.Choices[index][Character.Customization[index]].Bone == 1;
            Character.GetComponentInChildren<Animator>().SetBool("Upright", upright);
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

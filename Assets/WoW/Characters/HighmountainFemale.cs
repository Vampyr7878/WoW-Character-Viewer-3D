using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle highmountain female customization
    public class HighmountainFemale : CharacterHelper
    {
        public HighmountainFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeEars(activeGeosets);
            ChangeForemane(activeGeosets);
            ChangeHair(activeGeosets);
            ChangeHairDecoration(activeGeosets);
            ChangeHeaddress(activeGeosets);
            ChangeEyeColor(activeGeosets);
            ChangeHornStyle(activeGeosets);
            ChangeHornWraps(activeGeosets);
            ChangeHornDecoration(activeGeosets);
            ChangeFeather(activeGeosets);
            ChangeEarrings(activeGeosets);
            ChangeNosePiercing(activeGeosets);
            ChangeNecklace(activeGeosets);
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

        private void ChangeHairDecoration(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair Decoration");
            activeGeosets.RemoveAll(x => x > 99 && x < 200);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeHeaddress(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Headdress");
            activeGeosets.RemoveAll(x => x > 3699 && x < 3800);
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

        private void ChangeHornStyle(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horn Style");
            activeGeosets.RemoveAll(x => x > 2399 && x < 2500);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeHornWraps(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horn Wraps");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Horn Style");
            activeGeosets.RemoveAll(x => x > 4199 && x < 4300);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[Character.Customization[index2]].Geoset1);
        }

        private void ChangeHornDecoration(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horn Decoration");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Horn Style");
            activeGeosets.RemoveAll(x => x > 4299 && x < 4400);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[Character.Customization[index2]].Geoset1);
        }

        private void ChangeFeather(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Feather");
            activeGeosets.RemoveAll(x => x > 3899 && x < 4000);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeEarrings(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Earrings");
            activeGeosets.RemoveAll(x => x > 3499 && x < 3600);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeNosePiercing(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Nose Piercing");
            activeGeosets.RemoveAll(x => x > 1599 && x < 1700);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeNecklace(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Necklace");
            activeGeosets.RemoveAll(x => x > 3599 && x < 3700);
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
            activeGeosets.RemoveAll(x => x > 3999 && x < 4100);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
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
            if (Character.Items[3] == null && Character.Items[4] == null && Character.Items[5] == null)
            {
                Texture2D bra = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture4);
                DrawTexture(texture, bra, 256, 384);
            }
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

using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle nightborne female customization
    public class NightborneFemale : CharacterHelper
    {
        public NightborneFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeEars(activeGeosets);
            ChangeHairStyle(activeGeosets);
            ChangeEyebrows(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEyeColor(activeGeosets);
            ChangeHairDecoration(activeGeosets);
            ChangeHeaddress(activeGeosets);
            ChangeEarrings(activeGeosets);
            ChangeFaceJewelry(activeGeosets);
            ChangeJawJewelry(activeGeosets);
            ChangeNecklace(activeGeosets);
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
            activeGeosets.Add(HideHair ? Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset2 : Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeEyebrows(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Eyebrows");
            activeGeosets.RemoveAll(x => x > 3399 && x < 3500);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeEyes(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Eyes");
            activeGeosets.RemoveAll(x => x > 3199 && x < 3300);
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

        private void ChangeHairDecoration(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair Decoration");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            activeGeosets.RemoveAll(x => x > 3899 && x < 4000);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[Character.Customization[index2]].Geoset1);
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

        private void ChangeFaceJewelry(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Face Jewelry");
            activeGeosets.RemoveAll(x => x > 1599 && x < 1700);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeJawJewelry(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Jaw Jewelry");
            activeGeosets.RemoveAll(x => x > 199 && x < 300);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeNecklace(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Necklace");
            activeGeosets.RemoveAll(x => x > 3599 && x < 3700);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            int index, index2;
            index = Array.FindIndex(Character.Options, o => o.Name == "Face");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            Texture2D face = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
            DrawTexture(texture, face, 512, 0);
            Emission = null;
            index = Array.FindIndex(Character.Options, o => o.Name == "Face Tattoo");
            if (Character.Choices[index][Character.Customization[index]].Textures[0].Texture1 >= 0)
            {
                Texture2D tattoo = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture1);
                DrawTexture(texture, tattoo, 512, 0);
                DrawTexture(texture, tattoo, 512, 0);
                Texture2D temp = Resources.Load<Texture2D>("Materials/Emission");
                Emission = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
                Emission.SetPixels32(temp.GetPixels32());
                tattoo = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture2);
                DrawTexture(Emission, tattoo, 512, 0);
                DrawTexture(Emission, tattoo, 512, 0);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Body Tattoo");
            if (Character.Choices[index][Character.Customization[index]].Textures[0].Texture1 >= 0)
            {
                Texture2D tattoo = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture1);
                DrawTexture(texture, tattoo, 0, 0);
                DrawTexture(texture, tattoo, 0, 0);
                if (Emission == null)
                {
                    Texture2D temp = Resources.Load<Texture2D>("Materials/Emission");
                    Emission = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
                    Emission.SetPixels32(temp.GetPixels32());
                }
                tattoo = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture2);
                DrawTexture(Emission, tattoo, 0, 0);
                DrawTexture(Emission, tattoo, 0, 0);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Luminous Hands");
            if (Character.Choices[index][Character.Customization[index]].Textures[0].Texture1 >= 0)
            {
                Texture2D tattoo = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture1);
                DrawTexture(texture, tattoo, 0, 0);
                DrawTexture(texture, tattoo, 0, 0);
                if (Emission == null)
                {
                    Texture2D temp = Resources.Load<Texture2D>("Materials/Emission");
                    Emission = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
                    Emission.SetPixels32(temp.GetPixels32());
                }
                tattoo = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture2);
                DrawTexture(Emission, tattoo, 0, 0);
                DrawTexture(Emission, tattoo, 0, 0);
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            Texture2D bra = null;
            if (Character.Items[3] == null && Character.Items[4] == null && Character.Items[5] == null)
            {
                bra = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture4);
                DrawTexture(texture, bra, 256, 384);
            }
            Texture2D underwear = null;
            if (!(Character.Items[3] != null && Character.Items[3].UpperLeg !> 0) && Character.Items[10] == null)
            {
                underwear = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture3);
                DrawTexture(texture, underwear, 256, 192);
            }
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
                Character.BlackChest(Emission);
                Character.BlackShirt(Emission);
                Character.BlackTabard(Emission);
                Character.BlackWrist(Emission);
                Character.BlackHands(Emission);
                Character.BlackWaist(Emission);
                Character.BlackLegs(Emission);
                Character.BlackFeet(Emission);
                Emission.Apply();
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

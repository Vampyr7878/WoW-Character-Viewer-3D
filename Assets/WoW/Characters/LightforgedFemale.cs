using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle lightforged female customization
    public class LightforgedFemale : CharacterHelper
    {
        private int[] hair;
        private int[] horn;

        public LightforgedFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            int index = Array.FindIndex(Character.Options, o => o.Name == "Headdress");
            hair = new int[character.Choices[index].Length];
            index = Array.FindIndex(Character.Options, o => o.Name == "Horn Decoration");
            horn = new int[character.Choices[index].Length];
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeEars(activeGeosets);
            ChangeHairStyle(activeGeosets);
            ChangeHorns(activeGeosets);
            ChangeHornDecoration(activeGeosets);
            ChangeHairDecoration(activeGeosets);
            ChangeEyeColor(activeGeosets);
            ChangeTendrils(activeGeosets);
            ChangeRune(activeGeosets);
            ChangeHeaddress(activeGeosets);
            ChangeEarrings(activeGeosets);
            ChangeJawDecoration(activeGeosets);
            ChangeNecklace(activeGeosets);
            ChangeTail(activeGeosets);
        }

        private void ChangeEars(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 699 && x < 800);
            activeGeosets.Add(702);
        }

        private void ChangeHairStyle(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Headdress");
            activeGeosets.RemoveAll(x => x > 0 && x < 100);
            activeGeosets.Add(HideHair ? Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset2 : Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            GetHeaddresses(index);
            if (hair[Character.Customization[index2]] == 0)
            {
                Character.CustomizationDropdowns[index2].value = 0;
            }
            Character.ChangeDropdown(index2, hair);
        }

        private void ChangeHorns(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horns");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Horn Decoration");
            activeGeosets.RemoveAll(x => x > 2399 && x < 2500);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            GetHornDecorations(index);
            if (horn[Character.Customization[index2]] == 0)
            {
                Character.CustomizationDropdowns[index2].value = 0;
            }
            Character.ChangeDropdown(index2, horn);
        }

        private void ChangeHornDecoration(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Horn Decoration");
            activeGeosets.RemoveAll(x => x > 3899 && x < 4000);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeHairDecoration(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hair Decoration");
            int index2 = Array.FindIndex(Character.Options, o => o.Name == "Hair Style");
            activeGeosets.RemoveAll(x => x > 4199 && x < 4300);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[Character.Customization[index2]].Geoset1);
        }

        private void ChangeEyeColor(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 3299 && x < 3400);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset2);
        }

        private void ChangeTendrils(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Tendrils");
            activeGeosets.RemoveAll(x => x > 199 && x < 300);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
        }

        private void ChangeRune(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Rune");
            activeGeosets.RemoveAll(x => x > 2499 && x < 2600);
            activeGeosets.Add(Character.Choices[index][Character.Customization[index]].Geosets[0].Geoset1);
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

        private void ChangeJawDecoration(List<int> activeGeosets)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Jaw Decoration");
            activeGeosets.RemoveAll(x => x > 3999 && x < 4100);
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

        private void GetHeaddresses(int index)
        {
            int flag = Character.Choices[index][Character.Customization[index]].Bone;
            for (int i = 0; i < hair.Length; i++)
            {
                hair[i] = (flag & 1);
                flag >>= 1;
            }
        }

        private void GetHornDecorations(int index)
        {
            int flag = Character.Choices[index][Character.Customization[index]].Bone;
            for (int i = 0; i < horn.Length; i++)
            {
                horn[i] = (flag & 1);
                flag >>= 1;
            }
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            int index, index2;
            index = Array.FindIndex(Character.Options, o => o.Name == "Face");
            index2 = Array.FindIndex(Character.Options, o => o.Name == "Skin Color");
            Texture2D face = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[Character.Customization[index2]].Texture1);
            DrawTexture(texture, face, 512, 0);
            index = Array.FindIndex(Character.Options, o => o.Name == "Tattoo");
            if (Character.Choices[index][Character.Customization[index]].Textures[0].Texture1 >= 0)
            {
                Texture2D tattoo = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture1);
                DrawTexture(texture, tattoo, 0, 0);
                Texture2D temp = Resources.Load<Texture2D>("Materials/Emission");
                Emission = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
                Emission.SetPixels32(temp.GetPixels32());
                tattoo = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture2);
                DrawTexture(Emission, tattoo, 0, 0);
            }
            else
            {
                Emission = null;
            }
            index = Array.FindIndex(Character.Options, o => o.Name == "Jewelry Color");
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
            Texture2D jewelry = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture2);
            DrawTexture(texture, jewelry, 0, 0);
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
            index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color");
            if (Character.Choices[index][Character.Customization[index]].Textures[0].Texture2 >= 0)
            {
                Texture2D eyeglow = Character.TextureFromBLP(Character.Choices[index][Character.Customization[index]].Textures[0].Texture2);
                DrawTexture(texture, eyeglow, 512, 0, 0.5f);
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

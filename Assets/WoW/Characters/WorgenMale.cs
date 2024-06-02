using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle worgen male customization
    public class WorgenMale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> furColorFaces;

        private readonly Dictionary<int, int[]> skinColorFaces;

        private readonly Dictionary<int, int[]> hairStyleColors;

        private readonly Dictionary<int, int[]> beardSideburns;

        public WorgenMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            furColorFaces = new()
            {
                { 134, new int[] { 2246, 2251, 2252, 2253, 2254, 2255 } },
                { 322, new int[] { 2241, 2242, 2243, 2244, 2245, 2246, 2247, 2248, 2249, 2250, 2256, 2257, 2258, 2259 } }
            };
            skinColorFaces = new()
            {
                { 53, new int[] { 20, 22, 31 } },
                { 291, new int[] { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 } },
                { 295, new int[] { 15416, 15417, 15418, 15419, 15420, 15421, 15422, 15423, 15424, 15425, 15426, 15427 } },
                { 296, new int[] { 15428, 15429, 15430, 15431, 15432, 15433, 15434, 15435, 15436, 15437, 15438, 15439 } }
            };
            hairStyleColors = new()
            {
                { 35, new int[] { 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 4988 } },
                { 36, new int[] { 4989, 4990, 4991, 4992 } },
                { 37, new int[] { 4993, 4994, 4995, 4996 } }
            };
            beardSideburns = new()
            {
                { 141, new int[] { 76, 77, 78, 79, 80, 81, 82, 83 } },
                { 225, new int[] { 76, 77, 80, 81, 82, 83 } },
                { 226, new int[] { 76, 77, 81, 82, 83 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            HideOtherFormsOptions();
            switch (Character.Form)
            {
                case 0:
                    WorgenGeosets(activeGeosets);
                    break;
                case 10:
                    HumanGeosets(activeGeosets);
                    break;
            }
        }

        private void WorgenGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ActivateRelatedTextureOptions("Fur Color", "Face", furColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeEyeColor(activeGeosets);
        }

        private void HumanGeosets(List<int> activeGeosets)
        {
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Face Shape");
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Hair Color", hairStyleColors);
            ChangeGeosetOption(activeGeosets, "Mustache");
            ChangeRelatedGeosetOptions(activeGeosets, "Beard", "Sideburns", beardSideburns);
            ChangeGeosetOption(activeGeosets, "Sideburns");
            ChangeGeosetOption(activeGeosets, "Eyebrows");
            ChangeEyeColor(activeGeosets);
        }

        public override void ChangeForm()
        {
            switch(Character.Form)
            {
                case 0:
                    Character.ModelID = 43;
                    Character.ActivateMainMesh();
                    break;
                case 10:
                    Character.ModelID = 1;
                    Character.ActivateExtranMesh();
                    break;
            }
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            switch (Character.Form)
            {
                case 0:
                    WorgenTextures(texture);
                    break;
                case 10:
                    HumanTextures(texture);
                    break;
            }
        }

        private void WorgenTextures(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Fur Color", 512, 0, 512, 512);
            DrawUnderwear(texture, "Fur Color");
            DrawLayer(texture, "Eyesight", "Eye Color", 512, 0, 512, 512);
            DrawArmor(texture, true);
        }

        private void HumanTextures(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            OverlayLayer(texture, "Skin Color", 30, 0, 0, 1024, 512);
            DrawUnderwear(texture);
            DrawLayer(texture, "Eye Color", 36, 512, 0, 512, 512);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawLayer(texture, "Eyebrows", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture);
        }

        protected override int GetSkinColorIndex()
        {
            if (Character.Form == 0)
                return Array.FindIndex(Character.Options, o => o.Name == "Fur Color");
            return base.GetSkinColorIndex();
        }

        protected override int GetSkinExtraIndex()
        {
            if (Character.Form == 0)
                return Array.FindIndex(Character.Options, o => o.Name == "Fur Color");
            return base.GetSkinExtraIndex();
        }
    }
}

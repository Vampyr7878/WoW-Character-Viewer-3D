using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle worgen female customization
    public class WorgenFemale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> furColorFaces;

        private readonly Dictionary<int, int[]> skinColorFaces;

        private readonly Dictionary<int, int[]> hairStyleColors;

        public WorgenFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            furColorFaces = new()
            {
                { 136, new int[] { 2337, 2339, 2343 } },
                { 141, new int[] { 2316, 2317, 2318, 2319, 2320, 2321, 2322, 2323, 2324, 2325, 2326, 2327, 2328, 2329, 2330,
                    2331, 2332, 2333, 2334, 2335, 2336, 2337, 2338, 2339, 2340, 2341, 2342, 2343, 2344, 2345, 2346, 2347 } }
            };
            skinColorFaces = new()
            {
                { 318, new int[] { 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116 } },
                { 319, new int[] { 102, 103, 112 } },
                { 320, new int[] { 15698, 15699, 15700, 15701, 15702, 15703, 15704, 15705, 15706, 15707, 15708, 15709, 15710, 15711, 15712 } },
                { 321, new int[] { 15713, 15714, 15715, 15716, 15717, 15718, 15719, 15720, 15721, 15722, 15723, 15724, 15725, 15726, 15727 } }
            };
            hairStyleColors = new()
            {
                { 41, new int[] { 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 15741, 7473 } },
                { 42, new int[] { 4980, 4981, 4982, 4983 } },
                { 43, new int[] { 4984, 4985, 4986, 4987 } }
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
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeEyeColor(activeGeosets);
        }

        private void HumanGeosets(List<int> activeGeosets)
        {
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeEars(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Hair Color", hairStyleColors);
            ChangeGeosetOption(activeGeosets, "Face Shape");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Piercings");
            ChangeGeosetOption(activeGeosets, "Necklace");
        }

        public override void ChangeForm()
        {
            switch (Character.Form)
            {
                case 0:
                    Character.ModelID = 44;
                    Character.ActivateMainMesh();
                    break;
                case 10:
                    Character.ModelID = 2;
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
            DrawBra(texture, "Fur Color");
            DrawUnderwear(texture, "Fur Color");
            DrawLayer(texture, "Eyesight", "Eye Color", 512, 0, 512, 512);
            DrawArmor(texture, true);
        }

        private void HumanTextures(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            OverlayLayer(texture, "Skin Color", 30, 0, 0, 1024, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Makeup", 29, 512, 0, 512, 512);
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

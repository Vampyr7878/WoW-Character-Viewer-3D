using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle tauren female customization
    public class TaurenFemale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinColorFaces;

        public TaurenFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinColorFaces = new()
            {
                { 105, new int[] { 1139, 1141, 1142 } },
                { 141, new int[] { 1135, 1136, 1137, 1138, 1139, 1140, 1141, 1142 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Horn Style");
            ChangeGeosetOption(activeGeosets, "Foremane");
            ChangeGeosetOption(activeGeosets, "Hair");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Nose Ring");
            ChangeGeosetOption(activeGeosets, "Necklace");
            ChangeGeosetOption(activeGeosets, "Flower");
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Paint Color", "Face Paint", 512, 0, 512, 512);
            DrawLayer(texture, "Paint Color", "Body Paint", 0, 0, 512, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawArmor(texture, true);
        }

        protected override int GetHairColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Horn Color");
        }
    }
}

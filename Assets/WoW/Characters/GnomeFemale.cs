using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle gnome female customization
    public class GnomeFemale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinColorFaces;

        private readonly Dictionary<int, int[]> earringsColors;

        public GnomeFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinColorFaces = new()
            {
                { 119, new int[] { 1253, 1256, 1258 } },
                { 141, new int[] { 1246, 1247, 1248, 1249, 1250, 1251, 1252, 1253, 1254, 1255, 1256, 1257, 1258, 1259 } }
            };
            earringsColors = new()
            {
                { 313, new int[] { 1290, 1291, 1292, 1293, 1294, 1295, 8725 } },
                { 314, new int[] { 8719, 8720, 8721, 8722, 8723, 8724 } },
                { 316, new int[] { 8726 } },
                { 317, new int[] { 1289 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeEyeColor(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Earrings", "Earring Color", earringsColors);
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture);
        }

        protected override int GetJewelryColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Earring Color");
        }
    }
}

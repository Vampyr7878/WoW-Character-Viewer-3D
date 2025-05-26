using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle gnome female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class GnomeFemale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping earring colors to earrings
        private readonly Dictionary<int, int[]> earringsColors;

        public GnomeFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
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

        // Change geosets according to chosen character customization
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

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture);
        }

        // Get id of Jewelry Color option
        public override int GetJewelryColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Earring Color");
        }
    }
}

using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle troll female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class TrollFemale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping face paint colors to face paints
        private readonly Dictionary<int, int[]> facePaintColors;
        // Mapping body paint colors to body paints
        private readonly Dictionary<int, int[]> bodyPaintColors;

        public TrollFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 123, new int[] { 1388, 1391, 1393 } },
                { 141, new int[] { 1382, 1383, 1384, 1385, 1386, 1387, 1388, 1389, 1390, 1391, 1392, 1393 } }
            };
            facePaintColors = new()
            {
                { 185, new int[] { 8557, 8558, 8559 } },
                { 186, new int[] { 8556 } }
            };
            bodyPaintColors = new()
            {
                { 187, new int[] { 8553, 8554, 8555 } },
                { 188, new int[] { 8552 } }
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
            ChangeGeosetOption(activeGeosets, "Tusks");
            ChangeEyeColor(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Face Paint", "Face Paint Color", facePaintColors);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Piercing");
            ChangeRelatedGeosetOptions(activeGeosets, "Body Paint", "Body Paint Color", bodyPaintColors);
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            RectInt body = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Body);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawLayer(texture, "Face Paint Color", "Face Paint", face);
            DrawLayer(texture, "Body Paint Color", "Body Paint", body);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", face);
            DrawArmor(texture, true);
        }
    }
}

using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle goblin male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class GoblinMale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping earrings to ears
        private readonly Dictionary<int, int[]> earringEars;
        // Mapping nose rings to noses
        private readonly Dictionary<int, int[]> noseRingNoses;

        public GoblinMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 126, new int[] { 1451, 1452, 1453 } },
                { 141, new int[] { 1446, 1447, 1448, 1449, 1450, 1451, 1452, 1453, 1454, 1455 } }
            };
            earringEars = new()
            {
                { 141, new int[] { 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497 } },
                { 203, new int[] { 1489 } },
                { 204, new int[] { 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1497 } },
                { 205, new int[] { 1488, 1489, 1490, 1491, 1493, 1494, 1495, 1497 } },
                { 206, new int[] { 1488, 1489, 1490, 1493, 1497 } },
                { 207, new int[] { 1488 } },
                { 424, new int[] { 1491 } }
            };
            noseRingNoses = new()
            {
                { 141, new int[] { 9066, 9067, 9068, 9069, 9070, 9071, 9072, 9073, 9074, 9076, 9077 } },
                { 208, new int[] { 9067, 9068, 9069, 9070, 9071, 9072, 9073, 9074, 9076, 9077 } },
                { 209, new int[] { 9067, 9068, 9069, 9070, 9071, 9072, 9073, 9074, 9076 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Chin");
            ActivateRelatedGeosetOptions(activeGeosets, "Ears", "Earrings", earringEars);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ActivateRelatedGeosetOptions(activeGeosets, "Nose", "Nose Ring", noseRingNoses);
            ChangeGeosetOption(activeGeosets, "Nose Ring");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawUnderwear(texture);
            DrawArmor(texture);
        }
    }
}

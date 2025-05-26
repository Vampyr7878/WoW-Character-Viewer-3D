using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle undead male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class UndeadMale : CharacterHelper
    {
        // Mapping skin colors to skin types
        private readonly Dictionary<int, int[]> skinTypeColors;
        // Mapping face features to jaw features
        private readonly Dictionary<int, int[]> jawFaceFeatures;
        // Mapping eyesight to eye colors
        private readonly Dictionary<int, int[]> eyesightColors;

        public UndeadMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinTypeColors = new()
            {
                { 61, new int[] { 6527 } },
                { 62, new int[] { 6528 } },
                { 63, new int[] { 6529 } }
            };
            jawFaceFeatures = new()
            {
                { 58, new int[] { 967, 968, 975, 977, 979 } },
                { 59, new int[] { 971, 983, 969, 976, 978, 980 } },
                { 141, new int[] { 967, 968, 971, 983, 969, 975, 976, 977, 978, 979, 980 } }
            };
            eyesightColors = new()
            {
                { 141, new int[] { 5331, 5330, 5333, 5334, 5332, 5335, 6304, 5344 } },
                { 144, new int[] { 5331, 5330, 5333, 5334, 5332, 5335 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeEyes(activeGeosets);
            ChangeFace(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Skin Type", "Skin Color", skinTypeColors);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeRelatedGeosetOptions(activeGeosets, "Jaw Features", "Face Features", jawFaceFeatures);
            ChangeGeosetOption(activeGeosets, "Face Features");
            ChangeEyeColor(activeGeosets);
        }

        // Change goesets in according to eye color and make sure left over geosets are removed
        private new void ChangeEyeColor(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 5099 && x < 5200);
            ChangeRelatedGeosetOptions(activeGeosets, "Eye Color", "Eyesight", eyesightColors);
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawLayer(texture, "Jaw Features", 8, 512, 0, 512, 512);
            DrawArmor(texture);
        }
    }
}

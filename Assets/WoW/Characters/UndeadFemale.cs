using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle undead female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class UndeadFemale : CharacterHelper
    {
        // Mapping skin colors to skin types
        private readonly Dictionary<int, int[]> skinTypeColors;
        // Mapping eyesight to eye colors
        private readonly Dictionary<int, int[]> eyesightColors;

        public UndeadFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinTypeColors = new()
            {
                { 64, new int[] { 6526 } },
                { 65, new int[] { 6525 } },
                { 66, new int[] { 6524 } }
            };
            eyesightColors = new()
            {
                { 141, new int[] { 5338, 5337, 5340, 5341, 5339, 5342, 6305, 5345 } },
                { 144, new int[] { 5338, 5337, 5340, 5341, 5339, 5342 } }
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
            ChangeGeosetOption(activeGeosets, "Face Features");
            ChangeGeosetOption(activeGeosets, "Jaw Features");
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
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", face);
            DrawLayer(texture, "Jaw Features", 7, face);
            DrawArmor(texture);
        }
    }
}

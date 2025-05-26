using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle lightforged male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class LightforgedMale : CharacterHelper
    {
        // Mapping decorations to hair styles
        private readonly Dictionary<int, int[]> hairStyleDecorations;

        public LightforgedMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            hairStyleDecorations = new()
            {
                { 141, new int[] { 2925, 2926, 2927, 2928, 2929, 2930, 7491, 18255, 18256, 18257, 18258, 18259, 18260 } },
                { 378, new int[] { 18256 } },
                { 379, new int[] { 18258 } },
                { 380, new int[] { 18259 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Horn Decoration", hairStyleDecorations);
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeGeosetOption(activeGeosets, "Rune");
            ChangeGeosetOption(activeGeosets, "Eyebrows");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Tendrils");
            ChangeGeosetOption(activeGeosets, "Horn Decoration");
            ChangeGeosetOption(activeGeosets, "Tail");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            Emission = null;
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Tattoo", 16, 0, 0, 512, 512);
            DrawEmission("Tattoo", 16, 0, 0, 512, 512);
            DrawUnderwear(texture, "Jewelry Color");
            DrawLayer(texture, "Jewelry Color", 17, 0, 0, 512, 512);
            DrawLayer(texture, "Eye Color", 36, 512, 0, 512, 512);
            DrawLayer(texture, "Eyesight", "Eye Color", 512, 0, 512, 512);
            DrawArmor(texture, true);
        }
    }
}

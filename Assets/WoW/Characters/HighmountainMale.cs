using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle highmountain male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class HighmountainMale : CharacterHelper
    {
        // Mapping horn wraps to horn styles
        private readonly Dictionary<int, int[]> hornStyleWraps;
        // Mapping horn decorations to horn styles
        private readonly Dictionary<int, int[]> hornStyleDecorations;

        public HighmountainMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            hornStyleWraps = new()
            {
                { 141, new int[] { 2769, 2770, 2771, 2772, 2773, 19299, 19300, 19301, 19302 } },
                { 401, new int[] { 2769, 2773, 19300, 19301, 19302 } },
                { 402, new int[] { 2771 } }
            };
            hornStyleDecorations = new()
            {
                { 141, new int[] { 2769, 2770, 2771, 2772, 2773, 19299, 19300, 19301, 19302 } },
                { 402, new int[] { 2771 } },
                { 403, new int[] { 2770 } },
                { 404, new int[] { 2772 } },
                { 405, new int[] { 19299 } },
                { 406, new int[] { 19302 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Foremane");
            ChangeGeosetOption(activeGeosets, "Hair");
            ChangeGeosetOption(activeGeosets, "Beard");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeRelatedGeosetOptions(activeGeosets, "Horn Style", "Horn Wraps", hornStyleWraps);
            ChangeRelatedGeosetOptions(activeGeosets, "Horn Style", "Horn Decoration", hornStyleDecorations);
            ChangeGeosetOption(activeGeosets, "Horn Wraps");
            ChangeGeosetOption(activeGeosets, "Horn Decoration");
            ChangeGeosetOption(activeGeosets, "Feather");
            ChangeGeosetOption(activeGeosets, "Nose Piercing");
            ChangeGeosetOption(activeGeosets, "Tail");
            ChangeGeosetOption(activeGeosets, "Tail Decoration");
            ChangeGeosetOption(activeGeosets, "Tail Decoration", "Tail");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            RectInt full = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Full);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawLayer(texture, "Body Paint Color", "Body Paint", full);
            DrawUnderwear(texture);
            DrawArmor(texture, true);
        }

        // Get id of Hair Color option
        public override int GetHairColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Horn Color");
        }

        // Get id of Second Hair Color option
        protected override int GetHairColor2Index()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Horn Markings");
        }
    }
}

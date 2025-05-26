using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle void elf female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class VoidElfFemale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping eye colors to skin colors
        private readonly Dictionary<int, int[]> skinColorEyes;
        // Mapping tentacles to hair styles
        private readonly Dictionary<int, int[]> HairStyleTentacles;

        public VoidElfFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 218, new int[] { 9791, 9794, 9799 } },
                { 323, new int[] { 2878, 2879, 2880, 2881, 2882, 2883, 2884, 2885 } },
                { 324, new int[] { 9781, 9782, 9783, 9784, 9785, 9786, 9787, 9788, 9789, 9790, 9791, 9792, 9793, 9794, 9795, 9796, 9797, 9798, 9799, 9800 } },
                { 646, new int[] { 2878, 2879, 2880, 2881, 2882, 2883, 2884, 2885, 9781, 9782, 9783, 9784, 9785, 9786, 9787, 9788, 9789, 9790 } }
            };
            skinColorEyes = new()
            {
                { 641, new int[] { 28849 } },
                { 643, new int[] { 2870, 2871, 2872, 2875, 2874, 2873, 9706, 9712, 9705, 9711, 9708, 9710, 9709, 9704, 9713, 9707, 9717, 9718, 9719, 9720 } },
                { 645, new int[] { 2870, 2871, 2872, 2875, 2874, 2873, 9706, 9712, 9705, 9711, 9708, 9710, 9709, 9704, 9713, 9707, 9717, 9718, 9719, 9720, 9714, 9716, 9715 } }
            };
            HairStyleTentacles = new()
            {
                { 141, new int[] { 2886, 2887, 2888, 2889, 2890, 2891, 2892, 2893, 2894, 2895, 7486 } },
                { 411, new int[] { 2886, 2887, 2888, 2889, 2892, 2894 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeHands(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Tentacles", HairStyleTentacles);
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeTentacles(activeGeosets);
        }

        // Change goesets in according to eye color and make sure left over geosets are removed
        private new void ChangeEyeColor(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 5099 && x < 5200);
            ActivateRelatedGeosetOptions(activeGeosets, "Skin Color", "Eye Color", skinColorEyes);
            ChangeGeosetOption(activeGeosets, "Eye Color");
        }
        
        // Change hair tentacles
        private void ChangeTentacles(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 2399 && x < 2500);
            ChangeGeosetOption(activeGeosets, "Tentacles", "Hair Style");
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
            DrawArmor(texture);
        }

        // Get id of Hair Color Extra option
        protected override int GetHairExtraIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Tentacles");
        }
    }
}

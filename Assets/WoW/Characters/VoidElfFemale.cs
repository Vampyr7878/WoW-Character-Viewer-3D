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
        // Mapping tattoo colors to tattoos
        private readonly Dictionary<int, int[]> tattooColors;

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
                { 143, new int[] { 2878, 2879, 2880, 2881, 2882, 2883, 2884, 2885, 9781, 9782, 9783, 9784, 9785, 9786 } },
                { 218, new int[] { 9791, 9794, 9799 } },
                { 323, new int[] { 2878, 2879, 2880, 2881, 2882, 2883, 2884, 2885 } },
                { 324, new int[] { 9781, 9782, 9783, 9784, 9785, 9786, 9787, 9788, 9789, 9790,
                    9791, 9792, 9793, 9794, 9795, 9796, 9797, 9798, 9799, 9800 } },
                { 646, new int[] { 2878, 2879, 2880, 2881, 2882, 2883, 2884, 2885, 9781,
                    9782, 9783, 9784, 9785, 9786, 9787, 9788, 9789, 9790 } }
            };
            skinColorEyes = new()
            {
                { 641, new int[] { 28849 } },
                { 643, new int[] { 2870, 2871, 2872, 2875, 2874, 2873, 9706, 9712, 9705, 9711,
                    9708, 9710, 9709, 9704, 9713, 9707, 9717, 9718, 9719, 9720 } },
                { 645, new int[] { 2870, 2871, 2872, 2875, 2874, 2873, 9706, 9712, 9705, 9711, 9708,
                    9710, 9709, 9704, 9713, 9707, 9717, 9718, 9719, 9720, 9714, 9716, 9715 } },
                { 4602, new int[] { 2870, 2871, 2872, 2873, 2874, 2875, 9704, 9705, 9706, 9707, 9708, 9709, 9710,
                    9711, 9712, 9713, 9717, 9718, 9719, 9720, 60155, 60156, 60157, 60158, 60159, 60160 } }
            };
            HairStyleTentacles = new()
            {
                { 141, new int[] { 2886, 2887, 2888, 2889, 2890, 2891, 2892, 2893, 2894, 2895, 7486 } },
                { 411, new int[] { 2886, 2887, 2888, 2889, 2892, 2894 } }
            };
            tattooColors = new()
            {
                { 4599, new int[] { 60134 } },
                { 4600, new int[] { 60135, 60136, 60137, 60138, 60139, 60140 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(HashSet<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEyebrows(activeGeosets);
            ChangeHands(activeGeosets);
            ChangeDHUnderwear(Character.racial.ActiveGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Tentacles", HairStyleTentacles);
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeTentacles(activeGeosets);
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Horns");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Blindfold");
            ChangeRelatedTextureOptions("Tattoo", "Tattoo Color", tattooColors);
        }

        // Change goesets in according to eye color and make sure left over geosets are removed
        private new void ChangeEyeColor(HashSet<int> activeGeosets)
        {
            activeGeosets.RemoveWhere(x => x > 1699 && x < 1800);
            activeGeosets.RemoveWhere(x => x > 5099 && x < 5200);
            ActivateRelatedGeosetOptions(activeGeosets, "Skin Color", "Eye Color", skinColorEyes);
            ChangeGeosetOption(activeGeosets, "Eye Color");
        }
        
        // Change hair tentacles
        private void ChangeTentacles(HashSet<int> activeGeosets)
        {
            activeGeosets.RemoveWhere(x => x > 2399 && x < 2500);
            ChangeGeosetOption(activeGeosets, "Tentacles", "Hair Style");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            Emission = null;
            var face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            var body = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Body);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawLayer(texture, "Tattoo Color", "Tattoo", body);
            DrawEmission("Tattoo Color", "Tattoo", body);
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

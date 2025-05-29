using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle void elf male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class VoidElfMale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping eye colors to skin colors
        private readonly Dictionary<int, int[]> skinColorEyes;
        // Mapping tentacles to hair styles
        private readonly Dictionary<int, int[]> HairStyleTentacles;

        public VoidElfMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 212, new int[] { 9773, 9775, 9778 } },
                { 325, new int[] { 2832, 2833, 2834, 2835, 2836, 2837, 2838, 2839 } },
                { 326, new int[] { 9761, 9762, 9763, 9764, 9765, 9766, 9767, 9768, 9769, 9770,
                    9771, 9772, 9773, 9774, 9775, 9776, 9777, 9778, 9779, 9780 } },
                { 646, new int[] { 2832, 2833, 2834, 2835, 2836, 2837, 2838, 2839, 9761,
                    9762, 9763, 9764, 9765, 9766, 9767, 9768, 9769, 9770 } }
            };
            skinColorEyes = new()
            {
                { 640, new int[] { 28848 } },
                { 642, new int[] { 2824, 2825, 2826, 2829, 2827, 2828, 9689, 9695, 9694, 9688,
                    9693, 9692, 9691, 9687, 9696, 9690, 9700, 9701, 9702, 9703 } },
                { 644, new int[] { 2824, 2825, 2826, 2829, 2827, 2828, 9689, 9695, 9694, 9688, 9693,
                    9692, 9691, 9687, 9696, 9690, 9700, 9701, 9702, 9703, 9699, 9697, 9698 } }
            };
            HairStyleTentacles = new()
            {
                { 141, new int[] { 2840, 2841, 2842, 2843, 2844, 2845, 2846, 2847, 2848, 2849, 2850, 2851, 7490 } },
                { 410, new int[] { 2842, 2845, 2846, 2847, 2849, 2851 } }
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
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeEyeColor(activeGeosets);
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

﻿using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle night elf female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class NightElfFemale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping eye colors to skin colors
        private readonly Dictionary<int, int[]> skinColorEyes;
        // Mapping markings colors to markings
        private readonly Dictionary<int, int[]> markingsColors;
        // Mapping tattoo colors to tattoos
        private readonly Dictionary<int, int[]> tattooColors;

        public NightElfFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 97, new int[] { 834, 840, 841 } },
                { 138, new int[] { 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835, 836, 837, 838, 839, 840, 841, 842 } },
                { 141, new int[] { 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835, 836, 837, 838, 839, 840, 841, 842 } },
                { 143, new int[] { 825, 826, 827, 828, 829, 830, 831, 832, 833 } },
                { 646, new int[] { 825, 826, 827, 828, 829, 830, 831, 832, 833 } }
            };
            skinColorEyes = new()
            {
                { 655, new int[] { 824 } },
                { 656, new int[] { 801, 809, 803, 802, 804, 807, 808, 8295, 8294, 8296, 806, 805, 823, 811, 810, 812 } },
                { 657, new int[] { 801, 809, 803, 802, 804, 807, 808, 8295, 8294, 8296, 806, 805, 815, 817, 818, 816, 819, 820 } },
                { 658, new int[] { 801, 809, 803, 802, 804, 807, 808, 8295, 8294, 8296, 806, 805, 823 } },
                { 661, new int[] { 801, 809, 803, 802, 804, 807, 808, 8295, 8294, 8296, 806, 805, 823, 811, 810, 812 } }
            };
            markingsColors = new()
            {
                { 173, new int[] { 871, 872, 873, 874, 875, 876, 877, 878, 879, 55353 } },
                { 174, new int[] { 880 } },
                { 175, new int[] { 871, 872, 873, 874, 875, 876, 877, 878, 879, 55353 } },
                { 337, new int[] { 871, 872, 873, 874, 875, 876, 877, 878, 879, 55353 } }
            };
            tattooColors = new()
            {
                { 171, new int[] { 55072, 55356, 55357, 55358 } },
                { 172, new int[] { 882, 883, 884, 885, 886, 3716 } },
                { 4117, new int[] { 881 } },
                { 4264, new int[] { 55072, 55356, 55357, 55358 } },
                { 4265, new int[] { 55072, 55356, 55357, 55358 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            Character.racial.ActiveGeosets.Clear();
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeHands(activeGeosets);
            ChangeDHUnderwear(Character.racial.ActiveGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Vines", "Hair Style");
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeGeosetOption(activeGeosets, "Eyebrows");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Blindfold");
            ChangeEyeColor(activeGeosets);
            ChangeRelatedTextureOptions("Markings", "Markings Color", markingsColors);
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Nose Ring");
            ChangeGeosetOption(activeGeosets, "Necklace");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Horns");
            ChangeRelatedTextureOptions("Tattoo", "Tattoo Color", tattooColors);
        }

        // Change goesets in according to eye color and make sure left over geosets are removed
        private new void ChangeEyeColor(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 5099 && x < 5200);
            ActivateRelatedGeosetOptions(activeGeosets, "Skin Color", "Eye Color", skinColorEyes);
            ChangeGeosetOption(activeGeosets, "Eye Color");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            Emission = null;
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            RectInt body = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Body);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawLayer(texture, "Markings Color", "Markings", face);
            DrawLayer(texture, "Tattoo Color", "Tattoo", body);
            DrawEmission("Tattoo Color", "Tattoo", body);
            OverlayLayer(texture, "Scars", 2, face);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Eye Color", 36, face);
            DrawLayer(texture, "Eyesight", "Eye Color", face);
            DrawLayer(texture, "Hair Style", "Hair Color", face);
            DrawArmor(texture);
        }

        // Get id of Accesories option
        protected override int GetAccessoriesIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Vine Color");
        }
    }
}

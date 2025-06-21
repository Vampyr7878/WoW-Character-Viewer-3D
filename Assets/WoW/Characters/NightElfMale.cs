using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle night elf male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class NightElfMale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping eye colors to skin colors
        private readonly Dictionary<int, int[]> skinColorEyes;
        // Mapping eye colors to blindfolds
        private readonly Dictionary<int, int[]> blindfoldEyes;
        // Mapping tattoo colors to tattoos
        private readonly Dictionary<int, int[]> tattooColors;

        public NightElfMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 95, new int[] { 728, 729, 732 } },
                { 138, new int[] { 719, 720, 722, 725, 726, 727, 721, 26841, 723, 26842, 724, 26843,
                    728, 729, 731, 734, 735, 736, 730, 28242, 732, 28243, 733, 28244 } },
                { 141, new int[] { 719, 720, 722, 725, 726, 727, 721, 26841, 723, 26842, 724, 26843,
                    728, 729, 731, 734, 735, 736, 730, 28242, 732, 28243, 733, 28244 } },
                { 143, new int[] { 719, 720, 722, 725, 726, 727, 721, 26841, 723, 26842, 724, 26843 } },
                { 646, new int[] { 719, 720, 722, 725, 726, 727, 721, 26841, 723, 26842, 724, 26843 } }
            };
            skinColorEyes = new()
            {
                { 647, new int[] { 695, 703, 696, 8304, 697, 8305, 699, 700, 698, 701, 702, 8303, 717, 709, 711, 710, 712, 713, 714 } },
                { 648, new int[] { 695, 703, 696, 8304, 697, 8305, 699, 700, 698, 701, 702, 8303, 717 } },
                { 649, new int[] { 695, 703, 696, 8304, 697, 8305, 699, 700, 698, 701, 702, 8303, 717, 705, 704, 706 } },
                { 650, new int[] { 695, 703, 696, 8304, 697, 8305, 699, 700, 698, 701, 702, 8303, 717, 705, 704, 706 } },
                { 653, new int[] { 695, 703, 696, 8304, 697, 8305, 699, 700, 698, 701, 702, 8303, 717, 705, 704, 706 } },
                { 654, new int[] { 718 } }
            };
            blindfoldEyes = new()
            {
                { 647, new int[] { 789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800 } },
                { 648, new int[] { 789, 795 } },
                { 649, new int[] { 795 } },
                { 650, new int[] { 789 } },
                { 653, new int[] { 789 } },
                { 654, new int[] { 789, 795 } }
            };
            tattooColors = new()
            {
                { 169, new int[] { 769, 8149, 8150, 8151, 8152 } },
                { 170, new int[] { 770, 771, 772, 773, 774, 3715 } }
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
            ChangeGeosetOption(activeGeosets, "Sideburns");
            ChangeGeosetOption(activeGeosets, "Mustache");
            ChangeGeosetOption(activeGeosets, "Beard");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Horns");
            ChangeGeosetOption(activeGeosets, "Eyebrows");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Blindfold");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeGeosetOption(activeGeosets, "Vines", "Hair Style");
            ChangeRelatedTextureOptions("Tattoo", "Tattoo Color", tattooColors);
        }

        // Change goesets in according to eye color and make sure left over geosets are removed
        private new void ChangeEyeColor(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 5099 && x < 5200);
            ActivateRelatedGeosetOptions(activeGeosets, "Skin Color", "Eye Color", skinColorEyes);
            if (Character.Class == WoWHelper.Class.DeathKnight)
            {
                ActivateRelatedGeosetOptions(activeGeosets, "Blindfold", "Eye Color", blindfoldEyes);
            }
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
            MultiplyLayer(texture, "Markings", 17, face);
            MultiplyLayer(texture, "Tattoo", 18, body);
            DrawLayer(texture, "Tattoo Color", "Tattoo", body);
            DrawEmission("Tattoo Color", "Tattoo", body);
            OverlayLayer(texture, "Scars", 2, face);
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

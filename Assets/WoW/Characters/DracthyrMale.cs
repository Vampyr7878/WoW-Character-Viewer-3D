﻿using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle dracthyr male customization
    public class DracthyrMale : CharacterHelper
    {
        // Mapping secondary colors to their strength
        private readonly Dictionary<int, int[]> secondaryColors;
        // Mapping horn jewelry to horns
        private readonly Dictionary<int, int[]> hornJewerly;
        // Mapping jewerely colors to jewelry
        private readonly Dictionary<int, int[]> jewerlyColors;
        // Mapping shoulder spikes to armor
        private readonly Dictionary<int, int[]> shoulderSpikes;
        // Mapping chest spikes to armor
        private readonly Dictionary<int, int[]> chestSpikes;
        // Mapping arm spikes to armor
        private readonly Dictionary<int, int[]> armSpikes;
        // Mapping highlights to hair styles
        private readonly Dictionary<int, int[]> highlightStyles;
        // Mapping horn colors to horns
        private readonly Dictionary<int, int[]> visageHorns;
        // Mapping horn jewelry to horns
        private readonly Dictionary<int, int[]> visageJewelry;
        // Mapping markings to scale patterns
        private readonly Dictionary<int, int[]> scalePatterns;
        // Mapping eyebrows to scale patterns
        private readonly Dictionary<int, int[]> scaleEyebrows;
        // Mapping earrings to ears
        private readonly Dictionary<int, int[]> earringEars;

        public DracthyrMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            secondaryColors = new()
            {
                { 674, new int[] { 19481 } },
                { 675, new int[] { 19482, 19483, 19484 } }
            };
            hornJewerly = new()
            {
                { 141,  new int[] { 19507, 26861, 26862, 26863, 26864, 26865, 26866, 26867, 26868, 26869,
                    26870, 26871, 26872, 26873, 26874, 26875, 26876, 26877, 29775, 29776 } },
                { 3133, new int[] { 26861, 26862, 26863, 26864, 26865, 26866, 26867, 26868, 26869,
                    26870, 26871, 26872, 26873, 26874, 26875, 26876, 26877, 29775, 29776 } }
            };
            jewerlyColors = new()
            {
                { 3134, new int[] { 26879, 26880, 26881 } },
                { 3135, new int[] { 26878 } }
            };
            shoulderSpikes = new()
            {
                { 141,  new int[] { 19554, 19555 } },
                { 3142, new int[] { 19554 } }
            };
            chestSpikes = new()
            {
                { 141,  new int[] { 19554, 19555 } },
                { 3142, new int[] { 19554 } }
            };
            armSpikes = new()
            {
                { 141,  new int[] { 19573, 19574 } },
                { 3143, new int[] { 19573 } },
                { 3144, new int[] { 19574 } }
            };
            highlightStyles = new()
            {
                { 141, new int[] { 27651, 27652, 27653, 27654, 27655, 27656, 27657, 27658, 27659, 27660,
                    27661, 27662, 27663, 27664, 27665, 27666, 27667, 27668, 27669, 27670, 27791 } },
                { 470, new int[] { 27651, 27652, 27653, 27654, 27655, 27656, 27657, 27658, 27659, 27660,
                    27661, 27662, 27663, 27664, 27665, 27666, 27667, 27668, 27669, 27670 } }
            };
            visageHorns = new()
            {
                { 466, new int[] { 28100, 28101, 28102, 28103, 28104, 28105, 28106, 28107, 28108,
                    28109, 28110, 28111, 28112, 28113, 28114, 28115, 28116, 29864, 29865 } },
                { 472, new int[] { 28099 } }
            };
            visageJewelry = new()
            {
                { 141, new int[] { 28099, 28100, 28101, 28102, 28103, 28104, 28105, 28106, 28107, 28108,
                    28109, 28110, 28111, 28112, 28113, 28114, 28115, 28116, 29864, 29865 } },
                { 466, new int[] { 28100, 28101, 28102, 28103, 28104, 28105, 28106, 28107, 28108,
                    28109, 28110, 28111, 28112, 28113, 28114, 28115, 28116, 29864, 29865 } }
            };
            scalePatterns = new()
            {
                { 461, new int[] { 27796 } },
                { 462, new int[] { 27795 } },
                { 463, new int[] { 27794 } },
                { 464, new int[] { 27793 } },
                { 465, new int[] { 27792 } }
            };
            scaleEyebrows = new()
            {
                { 141, new int[] { 27792, 27793, 27794, 27795, 27796 } },
                { 465, new int[] { 27792 } }
            };
            earringEars = new()
            {
                { 141, new int[] { 27713, 27714, 27715 } },
                { 459, new int[] { 27713 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            HideOtherFormsOptions();
            switch (Character.Form)
            {
                case 0:
                    DracthyrGeosets(activeGeosets);
                    break;
                case 1:
                    VisageGeosets(activeGeosets);
                    break;
            }
        }

        // Change Dracthyr form geosets according to chosen character customization
        private void DracthyrGeosets(List<int> activeGeosets)
        {
            Character.racial.ActiveGeosets.Clear();
            Character.armor.ActiveGeosets.Clear();
            ChangeEyes(activeGeosets);
            ChangeWings(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Body Size");
            ChangeGeosetOption(activeGeosets, "Body Size", "Chest Spikes");
            ChangeGeosetOption(activeGeosets, "Body Size", "Leg Spikes");
            ChangeRelatedTextureOptions("Secondary Color Strength", "Secondary Color", secondaryColors);
            ActivateRelatedGeosetOptions(activeGeosets, "Chest Spikes", "Shoulders", shoulderSpikes);
            ActivateRelatedGeosetOptions(activeGeosets, "Chest Spikes", "Chest", chestSpikes);
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Horns");
            ChangeRelatedSkinnedGeosetOption(Character.racial.ActiveGeosets, "Horns", "Horn Jewelry", hornJewerly);
            ChangeRelatedTextureOptions("Horn Jewelry", "Jewelry Color", jewerlyColors);
            ChangeGeosetOption(activeGeosets, "Crest");
            ChangeGeosetOption(activeGeosets, "Snout");
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeGeosetOption(activeGeosets, "Brow");
            ChangeGeosetOption(activeGeosets, "Cheekbone");
            ChangeGeosetOption(activeGeosets, "Cheek");
            ChangeGeosetOption(activeGeosets, "Throat");
            ChangeGeosetOption(activeGeosets, "Helm");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Helm");
            ChangeGeosetOption(activeGeosets, "Shoulders");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Shoulders", "Body Size");
            ChangeGeosetOption(activeGeosets, "Chest");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Chest", "Body Size");
            ChangeGeosetOption(activeGeosets, "Breastplate");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Breastplate", "Body Size");
            ChangeGeosetOption(activeGeosets, "Upper Arms");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Upper Arms", "Body Size");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Lower Arms", "Body Size");
            ActivateRelatedGeosetOptions(activeGeosets, "Arm Spikes", "Lower Arms", armSpikes);
            ChangeGeosetOption(activeGeosets, "Arm Spikes", "Body Size");
            ChangeGeosetOption(activeGeosets, "Waist");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Waist", "Body Size");
            ChangeGeosetOption(activeGeosets, "Breechcloth", "Body Size");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Breechcloth", "Body Size");
            ChangeGeosetOption(activeGeosets, "Thighs");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Thighs");
            ChangeGeosetOption(activeGeosets, "Feet");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Feet", "Body Size");
            ChangeGeosetOption(activeGeosets, "Wing Decoration");
            ChangeSkinnedGeosetOption(Character.armor.ActiveGeosets, "Wing Decoration");
            ChangeGeosetOption(activeGeosets, "Tail");
            ChangeGeosetOption(activeGeosets, "Tail Ridge");
        }

        // Change Visage form geosets according to chosen character customization
        private void VisageGeosets(List<int> activeGeosets)
        {
            Character.extraRacial.ActiveGeosets.Clear();
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeHands(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Hair Highlights", highlightStyles);
            ChangeGeosetOption(activeGeosets, "Hair Decoration", "Hair Style");
            ChangeGeosetOption(activeGeosets, "Mustache");
            ChangeGeosetOption(activeGeosets, "Beard");
            ChangeSkinnedGeosetOption(Character.extraRacial.ActiveGeosets, "Horns");
            ChangeRelatedTextureOptions("Horns", "Horn Color", visageHorns);
            ChangeRelatedSkinnedGeosetOption(Character.extraRacial.ActiveGeosets, "Horns", "Horn Jewelry", visageJewelry);
            ChangeGeosetOption(activeGeosets, "Scale Pattern");
            ChangeRelatedTextureOptions("Scale Pattern", "Scale Markings", scalePatterns);
            ChangeRelatedTextureOptions("Scale Pattern", "Eyebrows", scaleEyebrows);
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeRelatedGeosetOptions(activeGeosets, "Ears", "Earrings", earringEars);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Nose");
            ChangeSkinnedGeosetOption(Character.extraRacial.ActiveGeosets, "Chest Strap");
        }

        // Swap model id for current form
        public override void ChangeForm()
        {
            switch (Character.Form)
            {
                case 0:
                    Character.ModelID = Character.MainFormID;
                    Character.CurrentRace = WoWHelper.Race.Dracthyr;
                    Character.ActivateMainMesh();
                    break;
                case 1:
                    Character.ModelID = 127;
                    Character.CurrentRace = WoWHelper.Race.Visage;
                    Character.ActivateExtraMesh();
                    break;
                default:
                    Character.ModelID = Character.CreatureForms[Character.Form - 1].ID;
                    Character.CurrentRace = WoWHelper.Race.Dracthyr;
                    Character.ActivateCreature();
                    break;
            }
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            switch (Character.Form)
            {
                case 0:
                    DracthyrTextures(texture);
                    break;
                case 1:
                    VisageTextures(texture);
                    break;
            }
        }

        // Generate Dracthyr form skin texture from many layers
        private void DracthyrTextures(Texture2D texture)
        {
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            RectInt body = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Body);
            RectInt full = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Full);
            DrawLayer(texture, "Face", "Primary Color", face);
            DrawLayer(texture, "Markings Color", "Face Markings", 5, full);
            DrawLayer(texture, "Markings Color", "Face Pattern", 6, full);
            DrawLayer(texture, "Horn Color", 11, body);
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 16, full);
            DrawLayer(texture, "Markings Color", "Body Markings", 19, full);
            DrawLayer(texture, "Markings Color", "Body Pattern", 22, full);
        }

        // Generate Visabe form skin texture from many layers
        private void VisageTextures(Texture2D texture)
        {
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            RectInt body = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Body);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawLayer(texture, "Scale Color", "Body Scales", 2, body);
            DrawLayer(texture, "Scale Color", "Scale Markings", 5, face);
            DrawLayer(texture, "Eyebrows", "Hair Color", 7, face);
            DrawBra(texture, "Underclothes Top", "Underclothes Color");
            DrawUnderwear(texture, "Underclothes Bottom", "Underclothes Color");
            DrawArmor(texture);
        }

        // Draw Layers on extra skin texture
        protected override void LayeredExtra1(Texture2D texture)
        {
            RectInt body = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Body);
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 17, body);
            DrawLayer(texture, "Markings Color", "Face Pattern", 23, body);
        }

        // Draw Layers on extra skin texture
        protected override void LayeredExtra2(Texture2D texture)
        {
            RectInt full = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Full);
            DrawLayer(texture, "Horn Color", 12, full);
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 18, full);
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 21, full);
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 24, full);
        }

        // Draw Layers on hair texture
        protected override void LayeredHair(Texture2D texture)
        {
            RectInt rect = new(0, 0, texture.width, texture.height);
            DrawLayer(texture, "Hair Highlights", 12, rect);
        }

        // Get id of Skin Color option
        public override int GetSkinColorIndex()
        {
            if (Character.Form == 0)
            {
                return Array.FindIndex(Character.Options, o => o.Name == "Primary Color");
            }
            return base.GetSkinColorIndex();
        }

        // Get id of Skin Color Extra option
        protected override int GetSkinExtraIndex()
        {
            if (Character.Form == 0)
            {
                return Array.FindIndex(Character.Options, o => o.Name == "Primary Color");
            }
            return base.GetSkinExtraIndex();
        }

        // Get id of Hair Color Extra option
        protected override int GetHairExtraIndex()
        {
            if (Character.Form == 0)
            {
                return Array.FindIndex(Character.Options, o => o.Name == "Primary Color");
            }
            return base.GetHairExtraIndex();
        }

        // Get id of Accesories option
        protected override int GetAccessoriesIndex()
        {
            if (Character.Form == 0)
            {
                return Array.FindIndex(Character.Options, o => o.Name == "Primary Color");
            }
            return base.GetAccessoriesIndex();
        }

        // Get id of Ornament Color option
        public override int GetOrnamentColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Horn Color" && o.Model == Character.ModelID);
        }

        // Get id of Armor Color option
        public override int GetArmorColorIndex()
        {
            if (Character.Form == 0)
            {
                return Array.FindIndex(Character.Options, o => o.Name == "Armor Color");
            }
            return base.GetArmorColorIndex();
        }

        // Get id of Armor Style option
        public override int GetArmorStyleIndex()
        {
            if (Character.Form == 0)
            {
                return Array.FindIndex(Character.Options, o => o.Name == "Armor Style");
            }
            return base.GetArmorStyleIndex();
        }
    }
}

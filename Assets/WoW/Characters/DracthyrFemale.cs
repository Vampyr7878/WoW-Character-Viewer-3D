using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle dracthyr female customization
    public class DracthyrFemale : CharacterHelper
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

        public DracthyrFemale(M2 model, Character character, ComputeShader shader)
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
                { 141,  new int[] { 19507, 26861, 26862, 26863, 26864, 26865, 26866, 26867, 26868, 26869, 26870, 26871, 26872, 26873, 26874, 26875, 26876, 26877, 29775, 29776 } },
                { 3133, new int[] { 26861, 26862, 26863, 26864, 26865, 26866, 26867, 26868, 26869, 26870, 26871, 26872, 26873, 26874, 26875, 26876, 26877, 29775, 29776 } }
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
                { 141, new int[] { 26952, 26953, 26954, 26955, 26956, 26957, 26958, 26959, 26960, 26961, 26962, 26963, 26964, 26965, 26966, 26967, 26968, 26969, 26970, 26971, 26972 } },
                { 448, new int[] { 26953, 26954, 26955, 26956, 26957, 26958, 26959, 26960, 26961, 26962, 26963, 26964, 26965, 26966, 26967, 26968, 26969, 26970, 26971, 26972 } }
            };
            visageHorns = new()
            {
                { 458, new int[] { 27043, 27044, 27045, 27046, 27047, 27048, 27049, 27050, 27051, 27052, 27053, 27054, 27055, 27056, 27057, 27058, 27059, 29866, 29867 } },
                { 473, new int[] { 27042 } }
            };
            visageJewelry = new()
            {
                { 141, new int[] { 27042, 27043, 27044, 27045, 27046, 27047, 27048, 27049, 27050, 27051, 27052, 27053, 27054, 27055, 27056, 27057, 27058, 27059, 29866, 29867 } },
                { 458, new int[] { 27043, 27044, 27045, 27046, 27047, 27048, 27049, 27050, 27051, 27052, 27053, 27054, 27055, 27056, 27057, 27058, 27059, 29866, 29867 } }
            };
            scalePatterns = new()
            {
                { 453, new int[] { 27129 } },
                { 454, new int[] { 27130 } },
                { 455, new int[] { 27131 } },
                { 456, new int[] { 27132 } },
                { 457, new int[] { 27128 } }
            };
            scaleEyebrows = new()
            {
                { 141, new int[] { 27128, 27129, 27130, 27131, 27132 } },
                { 457, new int[] { 27128 } }
            };
            earringEars = new()
            {
                { 141, new int[] { 27004, 27005, 27006 } },
                { 451, new int[] { 27004 } },
                { 452, new int[] { 27004, 27005 } }
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
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Breastplate", "Body Size");
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
            ChangeRelatedTextureOptions("Horns", "Horn Color", visageHorns);
            ChangeSkinnedGeosetOption(Character.extraRacial.ActiveGeosets, "Horns");
            ChangeRelatedSkinnedGeosetOption(Character.extraRacial.ActiveGeosets, "Horns", "Horn Jewelry", visageJewelry);
            ChangeGeosetOption(activeGeosets, "Scale Pattern");
            ChangeRelatedTextureOptions("Scale Pattern", "Scale Markings", scalePatterns);
            ChangeRelatedTextureOptions("Scale Pattern", "Eyebrows", scaleEyebrows);
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeRelatedGeosetOptions(activeGeosets, "Ears", "Earrings", earringEars);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Nose");
            ChangeGeosetOption(activeGeosets, "Chin");
            ChangeSkinnedGeosetOption(Character.extraRacial.ActiveGeosets, "Necklace");
        }

        // Swap model id for current form
        public override void ChangeForm()
        {
            switch (Character.Form)
            {
                case 0:
                    Character.ModelID = Character.MainFormID;
                    Character.ActivateMainMesh();
                    break;
                case 1:
                    Character.ModelID = 128;
                    Character.ActivateExtraMesh();
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
            DrawLayer(texture, "Face", "Primary Color", 512, 0, 512, 512);
            DrawLayer(texture, "Markings Color", "Face Markings", 5, 0, 0, 1024, 512);
            DrawLayer(texture, "Markings Color", "Face Pattern", 6, 0, 0, 1024, 512);
            DrawLayer(texture, "Horn Color", 11, 0, 0, 512, 512);
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 16, 0, 0, 1024, 512);
            DrawLayer(texture, "Markings Color", "Body Markings", 19, 0, 0, 1024, 512);
            DrawLayer(texture, "Markings Color", "Body Pattern", 22, 0, 0, 1024, 512);
        }

        // Generate Visabe form skin texture from many layers
        private void VisageTextures(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Scale Color", "Body Scales", 2, 0, 0, 512, 512);
            DrawLayer(texture, "Scale Color", "Scale Markings", 5, 512, 0, 512, 512);
            DrawLayer(texture, "Eyebrows", "Hair Color", 7, 512, 0, 512, 512);
            DrawBra(texture, "Underclothes Top", "Underclothes Color");
            DrawUnderwear(texture, "Underclothes Bottom", "Underclothes Color");
            DrawArmor(texture);
        }

        // Draw Layers on extra skin texture
        protected override void LayeredExtra1(Texture2D texture)
        {
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 17, 0, 0, 512, 512);
            DrawLayer(texture, "Markings Color", "Face Pattern", 23, 0, 0, 512, 512);
        }

        // Draw Layers on extra skin texture
        protected override void LayeredExtra2(Texture2D texture)
        {
            DrawLayer(texture, "Horn Color", 12, 0, 0, 1024, 512);
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 18, 0, 0, 1024, 512);
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 21, 0, 0, 1024, 512);
            DrawLayer(texture, "Secondary Color", "Secondary Color Strength", 24, 0, 0, 1024, 512);
        }

        // Draw Layers on hair texture
        protected override void LayeredHair(Texture2D texture)
        {
            DrawLayer(texture, "Hair Highlights", 12, 0, 0, texture.width, texture.height);
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
            return base.GetOrnamentColorIndex();
        }

        // Get id of Armor Style option
        public override int GetArmorStyleIndex()
        {
            if (Character.Form == 0)
            {
                return Array.FindIndex(Character.Options, o => o.Name == "Armor Style");
            }
            return base.GetOrnamentColorIndex();
        }
    }
}

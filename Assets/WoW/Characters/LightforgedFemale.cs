using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle lightforged female customization
    public class LightforgedFemale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> hairStyleDecorations;

        private readonly Dictionary<int, int[]> hairStyleHeaddress;

        private readonly Dictionary<int, int[]> hornsDecorations;

        public LightforgedFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            hairStyleDecorations = new()
            {
                { 141, new int[] { 2970, 2971, 2972, 2973, 2974, 2975, 2976, 7483, 18261, 18262, 18263, 18264, 18265 } },
                { 373, new int[] { 2971 } },
                { 374, new int[] { 2972 } },
                { 375, new int[] { 18262 } }
            };
            hairStyleHeaddress = new()
            {
                { 141, new int[] { 2970, 2971, 2972, 2973, 2974, 2975, 2976, 7483, 18261, 18262, 18263, 18264, 18265 } },
                { 376, new int[] { 2971, 2972, 2973, 2974, 7483 } },
                { 377, new int[] { 2972, 2973, 2974, 7483 } }
            };
            hornsDecorations = new()
            {
                { 141, new int[] { 2983, 2984, 2985, 2986, 2987, 2988, 18266, 18267, 18268, 18269, 18270, 18271, 18272 } },
                { 367, new int[] { 18266 } },
                { 369, new int[] { 18270 } },
                { 370, new int[] { 18272 } },
                { 400, new int[] { 18267, 18268, 18269 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Hair Decoration", hairStyleDecorations);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Headdress", hairStyleHeaddress);
            ChangeRelatedGeosetOptions(activeGeosets, "Horns", "Horn Decoration", hornsDecorations);
            ChangeGeosetOption(activeGeosets, "Horn Decoration");
            ChangeGeosetOption(activeGeosets, "Hair Decoration");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Tendrils");
            ChangeGeosetOption(activeGeosets, "Rune");
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Jaw Decoration");
            ChangeGeosetOption(activeGeosets, "Necklace");
            ChangeGeosetOption(activeGeosets, "Tail");
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            Emission = null;
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Tattoo", 16, 0, 0, 512, 512);
            DrawEmission("Tattoo", 16, 0, 0, 512, 512);
            DrawBra(texture, "Jewelry Color");
            DrawUnderwear(texture, "Jewelry Color");
            DrawLayer(texture, "Jewelry Color", 17, 0, 0, 512, 512);
            DrawLayer(texture, "Eye Color", 36, 512, 0, 512, 512);
            DrawLayer(texture, "Eyesight", "Eye Color", 512, 0, 512, 512);
            DrawArmor(texture, true);
        }
    }
}

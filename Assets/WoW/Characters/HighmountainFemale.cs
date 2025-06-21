using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle highmountain female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class HighmountainFemale : CharacterHelper
    {
        public HighmountainFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Foremane");
            ChangeGeosetOption(activeGeosets, "Hair");
            ChangeGeosetOption(activeGeosets, "Hair Decoration");
            ChangeGeosetOption(activeGeosets, "Feather");
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Horn Style");
            ChangeGeosetOption(activeGeosets, "Horn Wraps");
            ChangeGeosetOption(activeGeosets, "Horn Wraps", "Horn Style");
            ChangeGeosetOption(activeGeosets, "Horn Decoration");
            ChangeGeosetOption(activeGeosets, "Horn Decoration", "Horn Style");
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Nose Piercing");
            ChangeGeosetOption(activeGeosets, "Necklace");
            ChangeGeosetOption(activeGeosets, "Tail");
            ChangeGeosetOption(activeGeosets, "Tail Decoration");
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
            DrawBra(texture);
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

using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle tauren male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class TaurenMale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;

        public TaurenMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 103, new int[] { 1079, 1081, 1083 } },
                { 141, new int[] { 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Horn Style");
            ChangeGeosetOption(activeGeosets, "Foremane");
            ChangeGeosetOption(activeGeosets, "Hair");
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeGeosetOption(activeGeosets, "Nose Ring");
            ChangeGeosetOption(activeGeosets, "Necklace");
            ChangeGeosetOption(activeGeosets, "Flower");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            RectInt body = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Body);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawLayer(texture, "Paint Color", "Face Paint", face);
            DrawLayer(texture, "Paint Color", "Body Paint", body);
            DrawUnderwear(texture);
            DrawArmor(texture, true);
        }

        // Get id of Hair Color option
        public override int GetHairColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Horn Color");
        }
    }
}

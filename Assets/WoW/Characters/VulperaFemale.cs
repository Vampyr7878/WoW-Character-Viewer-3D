using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle vulpera female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class VulperaFemale : CharacterHelper
    {
        public VulperaFemale(M2 model, Character character, ComputeShader shader)
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
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeGeosetOption(activeGeosets, "Snout");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Earrings", "Ears");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            DrawLayer(texture, "Face", "Fur Color", 512, 0, 512, 512);
            DrawLayer(texture, "Pattern", "Fur Color", 0, 0, 1024, 512);
            DrawUnderwear(texture, "Fur Color");
            DrawBra(texture, "Fur Color");
            DrawArmor(texture, true);
        }

        // Get id of Skin Color option
        public override int GetSkinColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Fur Color");
        }
    }
}

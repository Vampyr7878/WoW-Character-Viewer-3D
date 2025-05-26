using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle nightborne male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class NightborneMale : CharacterHelper
    {
        public NightborneMale(M2 model, Character character, ComputeShader shader)
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
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Eyebrows");
            ChangeGeosetOption(activeGeosets, "Eyes");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Mustache");
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Face Jewelry");
            ChangeGeosetOption(activeGeosets, "Jaw Jewelry");
            ChangeGeosetOption(activeGeosets, "Chin Decoration");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            Emission = null;
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DoubleLayer(texture, "Face Tattoo", 19, 512, 0, 512, 512);
            DoubleEmission("Face Tattoo", 19, 512, 0, 512, 512);
            DoubleLayer(texture, "Body Tattoo", 16, 0, 0, 512, 512);
            DoubleEmission("Body Tattoo", 16, 0, 0, 512, 512);
            DoubleLayer(texture, "Luminous Hands", 22, 0, 0, 1024, 512);
            DoubleEmission("Luminous Hands", 22, 0, 0, 1024, 512);
            DrawUnderwear(texture);
            DrawLayer(texture, "Eyesight", "Eye Color", 512, 0, 512, 512);
            DrawArmor(texture);
        }
    }
}

using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle zandalari male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class ZandalariMale : CharacterHelper
    {
        public ZandalariMale(M2 model, Character character, ComputeShader shader)
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
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Tusks");
            ChangeGeosetOption(activeGeosets, "Piercings");
            ChangeGeosetOption(activeGeosets, "Ear Gauge");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            Emission = null;
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            RectInt full = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Full);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawLayer(texture, "Tattoo", 16, full);
            DrawEmission("Tattoo", 16, full);
            DrawUnderwear(texture);
            DrawLayer(texture, "Eye Color", 36, face);
            DrawLayer(texture, "Eyesight", "Eye Color", face);
            DrawArmor(texture, true);
        }
    }
}

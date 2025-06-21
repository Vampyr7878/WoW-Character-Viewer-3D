using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle pandaren male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class PandarenMale : CharacterHelper
    {
        public PandarenMale(M2 model, Character character, ComputeShader shader)
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
            ChangeGeosetOption(activeGeosets, "Mustache");
            ChangeGeosetOption(activeGeosets, "Beard");
            ChangeGeosetOption(activeGeosets, "Eyebrows");
            ChangeEyeColor(activeGeosets);
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Skin Color", face);
            DrawArmor(texture);
        }
    }
}

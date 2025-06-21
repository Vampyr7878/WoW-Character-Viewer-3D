using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle kul tiran male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class KulTiranMale : CharacterHelper
    {
        public KulTiranMale(M2 model, Character character, ComputeShader shader)
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
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Mustache");
            ChangeGeosetOption(activeGeosets, "Beard");
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
            DrawLayer(texture, "Hair Style", "Hair Color", face);
            DrawLayer(texture, "Hair Color", 8, face);
            DrawArmor(texture);
        }
    }
}

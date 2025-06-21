using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle pandaren female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class PandarenFemale : CharacterHelper
    {
        public PandarenFemale(M2 model, Character character, ComputeShader shader)
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
            ChangeUnderwear(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Tail");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawArmor(texture);
        }
    }
}

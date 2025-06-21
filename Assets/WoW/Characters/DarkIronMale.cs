using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle dark iron male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class DarkIronMale : CharacterHelper
    {
        public DarkIronMale(M2 model, Character character, ComputeShader shader)
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
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeGeosetOption(activeGeosets, "Piercings");
            ChangeEyeColor(activeGeosets);
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
            DrawLayer(texture, "Tattoo", 19, full);
            DrawUnderwear(texture);
            DrawArmor(texture);
        }
    }
}

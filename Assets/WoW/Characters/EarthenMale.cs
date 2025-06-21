using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle earthen male customization
    public class EarthenMale : CharacterHelper
    {
        public EarthenMale(M2 model, Character character, ComputeShader shader)
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
            Character.racial.ActiveGeosets.Clear();
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Hair Style");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Beard Style");
            ChangeGeosetOption(activeGeosets, "Eyebrows");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Left Shoulder");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Right Shoulder");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Arms");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Hands");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Torso");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Belt");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Legs");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            RectInt face = new(1024, 0, 1024, 1024);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawArmor(texture);
        }

        // Get id of Ornament Color option
        public override int GetOrnamentColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Gem Color");
        }
    }
}

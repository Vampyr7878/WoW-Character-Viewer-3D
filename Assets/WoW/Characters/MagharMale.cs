﻿using M2Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle mag'har male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class MagharMale : CharacterHelper
    {
        public MagharMale(M2 model, Character character, ComputeShader shader)
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
            ChangeGeosetOption(activeGeosets, "Beard");
            ChangeGeosetOption(activeGeosets, "Sideburns");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Piercings");
            ChangeGeosetOption(activeGeosets, "Tusks");
            ChangeHunched();
        }

        // Set model based on value of Hunched
        private void ChangeHunched()
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hunched");
            if (Character.Customization[index] == Character.Options[index].Choices.First().Key)
            {
                Character.ActivateMainMesh();
            }
            else
            {
                Character.ActivateExtraMesh();
            }
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
            DrawArmor(texture, true);
        }
    }
}

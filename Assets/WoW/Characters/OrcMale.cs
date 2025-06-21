using M2Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace WoW.Characters
{
    // Class to handle orc male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class OrcMale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;

        public OrcMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 80, new int[] { 393, 394, 398 } },
                { 141, new int[] { 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Beard");
            ChangeGeosetOption(activeGeosets, "Sideburns");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Nose Ring");
            ChangeGeosetOption(activeGeosets, "Tusks");
            ChangeUpright();
        }

        // Set model based on value of Upright
        private void ChangeUpright()
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Upright");
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
            RectInt full = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Full);
            DrawLayer(texture, "Face", "Skin Color", face);
            MultiplyLayer(texture, "Tattoo", 16, full);
            DrawLayer(texture, "War Paint Color", "War Paint", full);
            OverlayLayer(texture, "Scars", 2, full);
            OverlayLayer(texture, "Grime", 3, full);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", face);
            DrawLayer(texture, "Beard", "Hair Color", face);
            DrawArmor(texture);
        }
    }
}

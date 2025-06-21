using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle orc female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class OrcFemale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;

        public OrcFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 89, new int[] { 481, 488, 489 } },
                { 141, new int[] { 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Nose Ring");
            ChangeGeosetOption(activeGeosets, "Necklace");
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
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", face);
            DrawArmor(texture);
        }
    }
}

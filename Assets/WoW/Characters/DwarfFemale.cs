using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle dwarf female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class DwarfFemale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping earrings to hair styles
        private readonly Dictionary<int, int[]> hairStyleEarrings;

        public DwarfFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 93, new int[] { 644, 649, 650 } },
                { 141, new int[] { 634, 635, 636, 637, 638, 639, 640, 641, 642,
                    643, 644, 645, 646, 647, 648, 649, 650, 651, 652, 653 } }
            };
            hairStyleEarrings = new()
            {
                { 141, new int[] { 654, 655, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666,
                    667, 668, 669, 670, 671, 672, 673, 674, 7128, 7129, 7130, 7131, 7472 } },
                { 236, new int[] { 654, 657, 658, 659, 660, 667, 668, 671, 672, 7128, 7130, 7131, 7472 } },
                { 237, new int[] { 654, 656, 657, 658, 659, 660, 661, 662, 663, 664, 665, 666,
                    667, 668, 669, 670, 671, 672, 673, 674, 7128, 7129, 7130, 7131, 7472 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ActivateRelatedGeosetOptions(activeGeosets, "Hair Style", "Earrings", hairStyleEarrings);
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Piercings");
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
            DrawLayer(texture, "Tattoo Color", "Tattoo", full);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", face);
            DrawArmor(texture);
        }
    }
}

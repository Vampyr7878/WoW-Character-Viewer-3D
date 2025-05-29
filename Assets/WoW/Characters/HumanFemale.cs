using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle human female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class HumanFemale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping hair colors to hair styles
        private readonly Dictionary<int, int[]> hairStyleColors;

        public HumanFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 318, new int[] { 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116 } },
                { 319, new int[] { 102, 103, 112 } },
                { 320, new int[] { 15698, 15699, 15700, 15701, 15702, 15703, 15704,
                    15705, 15706, 15707, 15708, 15709, 15710, 15711, 15712 } },
                { 321, new int[] { 15713, 15714, 15715, 15716, 15717, 15718, 15719,
                    15720, 15721, 15722, 15723, 15724, 15725, 15726, 15727 } }
            };
            hairStyleColors = new()
            {
                { 41, new int[] { 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144,
                    145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 15741, 7473 } },
                { 42, new int[] { 4980, 4981, 4982, 4983 } },
                { 43, new int[] { 4984, 4985, 4986, 4987 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeEyes(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Ears");
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Hair Color", hairStyleColors);
            ChangeGeosetOption(activeGeosets, "Face Shape");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Piercings");
            ChangeGeosetOption(activeGeosets, "Necklace");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            OverlayLayer(texture, "Skin Color", 30, 0, 0, 1024, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Makeup", 29, 512, 0, 512, 512);
            DrawLayer(texture, "Eye Color", 36, 512, 0, 512, 512);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawLayer(texture, "Eyebrows", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture);
        }
    }
}

using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle goblin female customization
    public class GoblinFemale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinColorFaces;

        private readonly Dictionary<int, int[]> earringEars;

        private readonly Dictionary<int, int[]> noseRingNoses;

        public GoblinFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinColorFaces = new()
            {
                { 128, new int[] { 1536, 1542, 15730 } },
                { 141, new int[] { 1526, 1527, 1528, 1529, 1530, 1531, 1532, 1533, 1534, 1535, 1536, 1537, 1538, 1539, 1540, 1541, 1542, 1543, 15729, 15730 } }
            };
            earringEars = new()
            {
                { 141, new int[] { 1574, 1575, 1576, 1577, 1578, 1579 } },
                { 192, new int[] { 1574 } },
                { 193, new int[] { 1574, 1575, 1576 } },
                { 194, new int[] { 1574, 1575, 1576 } },
                { 195, new int[] { 1575 } },
                { 196, new int[] { 1575, 1576 } },
                { 197, new int[] { 1577 } },
                { 198, new int[] { 1579 } }
            };
            noseRingNoses = new()
            {
                { 141, new int[] { 9090, 9091, 9092, 9093, 9094, 9095, 9096, 9097, 9098 } },
                { 199, new int[] { 9091, 9092, 9093, 9094, 9095, 9096, 9097, 9098 } },
                { 200, new int[] { 9091, 9092, 9096, 9097, 9098 } },
                { 201, new int[] { 9093 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Chin");
            ActivateRelatedGeosetOptions(activeGeosets, "Ears", "Earrings", earringEars);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ActivateRelatedGeosetOptions(activeGeosets, "Nose", "Nose Ring", noseRingNoses);
            ChangeGeosetOption(activeGeosets, "Nose Ring");
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawLayer(texture, "Face", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture);
        }
    }
}

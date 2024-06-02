using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle goblin male customization
    public class GoblinMale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinColorFaces;

        private readonly Dictionary<int, int[]> earringEars;

        private readonly Dictionary<int, int[]> noseRingNoses;

        public GoblinMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinColorFaces = new()
            {
                { 126, new int[] { 1451, 1452, 1453 } },
                { 141, new int[] { 1446, 1447, 1448, 1449, 1450, 1451, 1452, 1453, 1454, 1455 } }
            };
            earringEars = new()
            {
                { 141, new int[] { 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497 } },
                { 203, new int[] { 1489 } },
                { 204, new int[] { 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1497 } },
                { 205, new int[] { 1488, 1489, 1490, 1491, 1493, 1494, 1495, 1497 } },
                { 206, new int[] { 1488, 1489, 1490, 1493, 1497 } },
                { 207, new int[] { 1488 } },
                { 424, new int[] { 1491 } }
            };
            noseRingNoses = new()
            {
                { 141, new int[] { 9066, 9067, 9068, 9069, 9070, 9071, 9072, 9073, 9074, 9076, 9077 } },
                { 208, new int[] { 9067, 9068, 9069, 9070, 9071, 9072, 9073, 9074, 9076, 9077 } },
                { 209, new int[] { 9067, 9068, 9069, 9070, 9071, 9072, 9073, 9074, 9076 } }
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
            DrawUnderwear(texture);
            DrawArmor(texture);
        }
    }
}

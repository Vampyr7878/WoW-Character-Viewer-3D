using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle troll male customization
    public class TrollMale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinColorFaces;

        private readonly Dictionary<int, int[]> facePaintColors;

        private readonly Dictionary<int, int[]> bodyPaintColors;

        public TrollMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinColorFaces = new()
            {
                { 121, new int[] { 1320, 1321, 1323 } },
                { 141, new int[] { 1315, 1316, 1317, 1318, 1319, 1320, 1321, 1322, 1323, 1324 } }
            };
            facePaintColors = new()
            {
                { 180, new int[] { 8539, 8540, 8541, 8542, 8543, 8544 } },
                { 181, new int[] { 8545, 8546, 8547 } },
                { 182, new int[] { 8538 } },
                { 332, new int[] { 8539, 8540, 8541, 8542, 8543, 8544 } }
            };
            bodyPaintColors = new()
            {
                { 183, new int[] { 8548 } },
                { 184, new int[] { 8549, 8550, 8551 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Tusks");
            ChangeEyeColor(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Face Paint", "Face Paint Color", facePaintColors);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Piercing");
            ChangeRelatedGeosetOptions(activeGeosets, "Body Paint", "Body Paint Color", bodyPaintColors);
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Face Paint Color", "Face Paint", 512, 0, 512, 512);
            DrawLayer(texture, "Body Paint Color", "Body Paint", 0, 0, 512, 512);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture, true);
        }
    }
}

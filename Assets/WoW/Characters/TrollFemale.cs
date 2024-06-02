using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle troll female customization
    public class TrollFemale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinColorFaces;

        private readonly Dictionary<int, int[]> facePaintColors;

        private readonly Dictionary<int, int[]> bodyPaintColors;

        public TrollFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinColorFaces = new()
            {
                { 123, new int[] { 1388, 1391, 1393 } },
                { 141, new int[] { 1382, 1383, 1384, 1385, 1386, 1387, 1388, 1389, 1390, 1391, 1392, 1393 } }
            };
            facePaintColors = new()
            {
                { 185, new int[] { 8557, 8558, 8559 } },
                { 186, new int[] { 8556 } }
            };
            bodyPaintColors = new()
            {
                { 187, new int[] { 8553, 8554, 8555 } },
                { 188, new int[] { 8552 } }
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
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture, true);
        }
    }
}

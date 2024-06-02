using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle undead female customization
    public class UndeadFemale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinTypeColors;

        private readonly Dictionary<int, int[]> eyesightColors;

        public UndeadFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinTypeColors = new()
            {
                { 64, new int[] { 6526 } },
                { 65, new int[] { 6525 } },
                { 66, new int[] { 6524 } }
            };
            eyesightColors = new()
            {
                { 141, new int[] { 5338, 5337, 5340, 5341, 5339, 5342, 6305, 5345 } },
                { 144, new int[] { 5338, 5337, 5340, 5341, 5339, 5342 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeEyes(activeGeosets);
            ChangeFace(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Skin Type", "Skin Color", skinTypeColors);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Face Features");
            ChangeGeosetOption(activeGeosets, "Jaw Features");
            ChangeEyeColor(activeGeosets);
        }

        private new void ChangeEyeColor(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 5099 && x < 5200);
            ChangeRelatedGeosetOptions(activeGeosets, "Eye Color", "Eyesight", eyesightColors);
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawLayer(texture, "Jaw Features", 7, 512, 0, 512, 512);
            DrawArmor(texture);
        }
    }
}

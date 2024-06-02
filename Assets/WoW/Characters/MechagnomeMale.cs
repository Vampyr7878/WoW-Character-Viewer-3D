using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle mechagnome male customization
    public class MechagnomeMale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinColorFaces;

        public MechagnomeMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinColorFaces = new()
            {
                { 141, new int[] { 8966, 8967, 8968, 8969, 8970, 8971, 8972, 8973, 8974, 8975, 8976, 8977, 8978, 8979 } },
                { 191, new int[] { 8973, 8975, 8976 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            Character.racial.ActiveGeosets.Clear();
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeModification(activeGeosets);
            ChangeEyeColor(activeGeosets);
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Arm Upgrade");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Leg Upgrade");
        }

        private void ChangeModification(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 699 && x < 800);
            ChangeGeosetOption(activeGeosets, "Modification");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Modification");
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Color", 8, 512, 0, 512, 512);
            DrawArmor(texture, true);
        }

        protected override int GetSkinExtraIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Paint");
        }
    }
}

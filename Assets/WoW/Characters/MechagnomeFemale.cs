using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle mechagnome female customization
    public class MechagnomeFemale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinColorFaces;

        public MechagnomeFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinColorFaces = new()
            {
                { 141, new int[] { 8980, 8981, 8982, 8983, 8984, 8985, 8986, 8987, 8988, 8989, 8990, 8991, 8992, 8993 } },
                { 190, new int[] { 8987, 8990, 8992 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            Character.racial.ActiveGeosets.Clear();
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
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
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture, true);
        }

        protected override int GetSkinExtraIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Paint");
        }
    }
}

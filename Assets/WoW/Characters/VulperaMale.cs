using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle vulpera male customization
    public class VulperaMale : CharacterHelper
    {
        public VulperaMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeGeosetOption(activeGeosets, "Snout");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Fur Color", 512, 0, 512, 512);
            DrawLayer(texture, "Pattern", "Fur Color", 512, 0, 512, 512);
            DrawUnderwear(texture, "Fur Color");
            DrawArmor(texture, true);
        }

        protected override int GetSkinColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Fur Color");
        }
    }
}

using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle dark iron male customization
    public class DarkIronMale : CharacterHelper
    {
        public DarkIronMale(M2 model, Character character)
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
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeGeosetOption(activeGeosets, "Piercings");
            ChangeEyeColor(activeGeosets);
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Tattoo", 19, 0, 0, 1024, 512);
            DrawUnderwear(texture);
            DrawArmor(texture);
        }
    }
}

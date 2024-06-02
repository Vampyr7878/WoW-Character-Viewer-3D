using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle dark iron female customization
    public class DarkIronFemale : CharacterHelper
    {
        public DarkIronFemale(M2 model, Character character)
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
            ChangeGeosetOption(activeGeosets, "Piercings");
            ChangeEyeColor(activeGeosets);
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Tattoo", 16, 0, 0, 1024, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawArmor(texture);
        }
    }
}

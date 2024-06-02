using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle mag'har male customization
    public class MagharMale : CharacterHelper
    {
        public MagharMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Beard");
            ChangeGeosetOption(activeGeosets, "Sideburns");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Piercings");
            ChangeGeosetOption(activeGeosets, "Tusks");
            ChangeHunched();
        }

        private void ChangeHunched()
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Hunched");
            if (Character.Customization[index] == 1)
            {
                Character.ActivateExtranMesh();
            }
            else
            {
                Character.ActivateMainMesh();
            }
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
        }
    }
}

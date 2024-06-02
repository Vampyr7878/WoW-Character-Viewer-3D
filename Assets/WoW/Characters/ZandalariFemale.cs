using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle zandalari female customization
    public class ZandalariFemale : CharacterHelper
    {
        public ZandalariFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Tusks");
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Ear Gauge");
            ChangeGeosetOption(activeGeosets, "Piercings");
            ChangeGeosetOption(activeGeosets, "Necklace");
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            Emission = null;
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Tattoo", 16, 0, 0, 1024, 512);
            DrawEmission("Tattoo", 16, 0, 0, 1024, 512);
            DrawUnderwear(texture);
            DrawBra(texture);
            DrawLayer(texture, "Eye Color", 36, 512, 0, 512, 512);
            DrawLayer(texture, "Eyesight", "Eye Color", 512, 0, 512, 512);
            DrawArmor(texture, true);
        }
    }
}

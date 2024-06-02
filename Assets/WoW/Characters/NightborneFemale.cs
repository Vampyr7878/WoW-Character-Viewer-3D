using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle nightborne female customization
    public class NightborneFemale : CharacterHelper
    {
        public NightborneFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Eyebrows");
            ChangeGeosetOption(activeGeosets, "Eyes");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Hair Decoration", "Hair Style");
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Face Jewelry");
            ChangeGeosetOption(activeGeosets, "Jaw Jewelry");
            ChangeGeosetOption(activeGeosets, "Necklace");
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            Emission = null;
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DoubleLayer(texture, "Face Tattoo", 19, 512, 0, 512, 512);
            DoubleEmission("Face Tattoo", 19, 512, 0, 512, 512);
            DoubleLayer(texture, "Body Tattoo", 16, 0, 0, 512, 512);
            DoubleEmission("Body Tattoo", 16, 0, 0, 512, 512);
            DoubleLayer(texture, "Luminous Hands", 22, 0, 0, 512, 512);
            DoubleEmission("Luminous Hands", 22, 0, 0, 512, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawLayer(texture, "Eyesight", "Eye Color", 512, 0, 512, 512);
            DrawArmor(texture);
        }
    }
}

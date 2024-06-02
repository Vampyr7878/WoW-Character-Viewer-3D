using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle highmountain female customization
    public class HighmountainFemale : CharacterHelper
    {
        public HighmountainFemale(M2 model, Character character)
        {
            Model = model;
            Character = character;
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Foremane");
            ChangeGeosetOption(activeGeosets, "Hair");
            ChangeGeosetOption(activeGeosets, "Hair Decoration");
            ChangeGeosetOption(activeGeosets, "Feather");
            ChangeGeosetOption(activeGeosets, "Headdress");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Horn Style");
            ChangeGeosetOption(activeGeosets, "Horn Wraps");
            ChangeGeosetOption(activeGeosets, "Horn Wraps", "Horn Style");
            ChangeGeosetOption(activeGeosets, "Horn Decoration");
            ChangeGeosetOption(activeGeosets, "Horn Decoration", "Horn Style");
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Nose Piercing");
            ChangeGeosetOption(activeGeosets, "Necklace");
            ChangeGeosetOption(activeGeosets, "Tail");
            ChangeGeosetOption(activeGeosets, "Tail Decoration");
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Body Paint Color", "Body Paint", 0, 0, 1024, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawArmor(texture, true);
        }

        protected override int GetHairColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Horn Color");
        }

        protected override int GetHairColor2Index()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Horn Markings");
        }
    }
}

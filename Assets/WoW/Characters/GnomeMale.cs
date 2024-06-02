using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    //Class to handle gnome male customization
    public class GnomeMale : CharacterHelper
    {
        private readonly Dictionary<int, int[]> skinColorFaces;

        public GnomeMale(M2 model, Character character)
        {
            Model = model;
            Character = character;
            skinColorFaces = new()
            {
                { 117, new int[] { 1192, 1194, 1195 } },
                { 141, new int[] { 1185, 1186, 1187, 1188, 1189, 1190, 1191, 1192, 1193, 1194, 1195, 1196, 1197, 1198 } }
            };
        }

        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Mustache");
            ChangeGeosetOption(activeGeosets, "Beard");
            ChangeGeosetOption(activeGeosets, "Eyebrows");
            ChangeEyeColor(activeGeosets);
        }

        protected override void LayeredTexture(Texture2D texture)
        {
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture);
        }
    }
}

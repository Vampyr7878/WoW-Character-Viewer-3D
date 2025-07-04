﻿using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle draenei female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class DraeneiFemale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping circlets to hair styles
        private readonly Dictionary<int, int[]> hairStyleCirclets;
        // Mapping decorations to hair styles
        private readonly Dictionary<int, int[]> hairStyleDecorations;

        public DraeneiFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 132, new int[] { 1973, 1977, 1981 } },
                { 141, new int[] { 1963, 1964, 1965, 1966, 1967, 1968, 1969, 1970, 1971, 1972,
                    1973, 1974, 1975, 1976, 1977, 1978, 1979, 1980, 1981, 1982 } },
                { 4112, new int[] { 1963, 1964, 1965, 1966, 1967, 1968, 1969, 1970, 1971, 1972,
                    1973, 1974, 1975, 1976, 1977, 1978, 1979, 1980, 1981, 1982 } }
            };
            hairStyleCirclets = new()
            {
                { 141, new int[] { 1983, 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994,
                    1995, 1996, 1997, 1998, 1999, 7484, 7779, 7780, 7781, 7782, 7783, 7784 } },
                { 159, new int[] { 1983, 1984, 1985, 1986, 1989, 1990, 1991, 1992, 1993,
                    1995, 1999, 7484, 7779, 7780, 7781, 7782, 7783, 7784 } },
            };
            hairStyleDecorations = new()
            {
                { 141, new int[] { 1983, 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994,
                    1995, 1996, 1997, 1998, 1999, 7484, 7779, 7780, 7781, 7782, 7783, 7784 } },
                { 160, new int[] { 7782 } },
                { 161, new int[] { 7783 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeEars(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Circlet", hairStyleCirclets);
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Hair Decoration", hairStyleDecorations);
            ChangeGeosetOption(activeGeosets, "Horns");
            ChangeEyeColor(activeGeosets);
            ChangeGeosetOption(activeGeosets, "Tendrils");
            ChangeGeosetOption(activeGeosets, "Circlet");
            ChangeGeosetOption(activeGeosets, "Hair Decoration");
            ChangeGeosetOption(activeGeosets, "Tail");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawArmor(texture, true);
        }
    }
}

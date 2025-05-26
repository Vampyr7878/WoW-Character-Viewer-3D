using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle draenei male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class DraeneiMale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping circlets to hair styles
        private readonly Dictionary<int, int[]> hairStyleCirclets;
        // Mapping decorations to hair styles
        private readonly Dictionary<int, int[]> hairStyleDecorations;
        // Mapping jewelry colors to circlets
        private readonly Dictionary<int, int[]> circletColors;

        public DraeneiMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 130, new int[] { 1901, 1904, 1909 } },
                { 141, new int[] { 1890, 1891, 1892, 1893, 1894, 1895, 1896, 1897, 1898, 1899, 1900, 1901, 1902, 1903, 1904, 1905, 1906, 1907, 1908, 1909 } },
                { 4112, new int[] { 1890, 1891, 1892, 1893, 1894, 1895, 1896, 1897, 1898, 1899, 1900, 1901, 1902, 1903, 1904, 1905, 1906, 1907, 1908, 1909 } }
            };
            hairStyleCirclets = new()
            {
                { 141, new int[] { 1910, 1911, 1912, 1913, 1914, 1915, 1916, 1917, 1918, 1919, 1920, 1921, 1922, 1923, 7770, 7771, 7772, 7773, 7774, 7775, 7776, 7777, 7778 } },
                { 163, new int[] { 1910, 1911, 1912, 1914, 1915, 1917, 1918, 1920, 1921, 1923, 7770, 7772, 7773, 7774, 7775, 7776, 7777, 7778 } },
            };
            hairStyleDecorations = new()
            {
                { 141, new int[] { 1910, 1911, 1912, 1913, 1914, 1915, 1916, 1917, 1918, 1919, 1920, 1921, 1922, 1923, 7770, 7771, 7772, 7773, 7774, 7775, 7776, 7777, 7778 } },
                { 164, new int[] { 7774 } },
                { 165, new int[] { 7777 } }
            };
            circletColors = new()
            {
                { 334, new int[] { 7801 } },
                { 333, new int[] { 7802 } },
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
            ChangeRelatedGeosetOptions(activeGeosets, "Hair Style", "Horn Decoration", hairStyleDecorations);
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeEyeColor(activeGeosets);
            ChangeRelatedGeosetOptions(activeGeosets, "Circlet", "Jewelry Color", circletColors);
            ChangeGeosetOption(activeGeosets, "Horn Decoration");
            ChangeGeosetOption(activeGeosets, "Tendrils");
            ChangeGeosetOption(activeGeosets, "Tail");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawUnderwear(texture);
            DrawArmor(texture, true);
        }
    }
}

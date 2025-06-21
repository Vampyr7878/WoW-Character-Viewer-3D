using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle blood elf male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class BloodElfMale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping eye colors to skin colors
        private readonly Dictionary<int, int[]> skinColorEyes;
        // Mapping tattoo colors to tattoos
        private readonly Dictionary<int, int[]> tattooColors;

        public BloodElfMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 109, new int[] { 1638, 1640, 1643 } },
                { 141, new int[] { 1626, 1627, 1628, 1629, 1630, 1631, 1632, 1633, 1634, 1635,
                    1636, 1637, 1638, 1639, 1640, 1641, 1642, 1643, 1644, 1645 } },
                { 143, new int[] { 1626, 1627, 1628, 1629, 1630, 1631 } },
                { 646, new int[] { 1626, 1627, 1628, 1629, 1630, 1631, 1632, 1633, 1634, 1635 } }
            };
            skinColorEyes = new()
            {
                { 662, new int[] { 1625 } },
                { 663, new int[] { 1601, 1607, 1606, 1600, 1605, 1604, 1603, 1599,
                    1608, 1602, 6698, 6697, 6696, 6695, 1617, 1615, 1616 } },
                { 664, new int[] { 1601, 1607, 1606, 1600, 1605, 1604, 1603, 1599, 1608, 1602,
                    6698, 6697, 6696, 6695, 1618, 1620, 1623, 1619, 1621, 1622 } },
                { 665, new int[] { 1601, 1607, 1606, 1600, 1605, 1604, 1603,
                    1599, 1608, 1602, 6698, 6697, 6696, 6695 } }
            };
            tattooColors = new()
            {
                { 227, new int[] { 1700 } },
                { 228, new int[] { 1701, 1702, 1703, 1704, 1705, 3713 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            Character.racial.ActiveGeosets.Clear();
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ChangeHands(activeGeosets);
            ChangeDHUnderwear(Character.racial.ActiveGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Horns");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Blindfold");
            ChangeRelatedTextureOptions("Tattoo", "Tattoo Color", tattooColors);
            ChangeEyeColor(activeGeosets);
        }

        // Change goesets in according to eye color and make sure left over geosets are removed
        private new void ChangeEyeColor(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 5099 && x < 5200);
            ActivateRelatedGeosetOptions(activeGeosets, "Skin Color", "Eye Color", skinColorEyes);
            ChangeGeosetOption(activeGeosets, "Eye Color");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            Emission = null;
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            RectInt body = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Body);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawLayer(texture, "Tattoo Color", "Tattoo", body);
            DrawEmission("Tattoo Color", "Tattoo", body);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Style", "Hair Color", face);
            DrawArmor(texture);
        }
    }
}

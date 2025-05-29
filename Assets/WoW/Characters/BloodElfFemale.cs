using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle blood elf female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class BloodElfFemale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;
        // Mapping eye colors to skin colors
        private readonly Dictionary<int, int[]> skinColorEyes;
        // Mapping tattoo colors to tattoos
        private readonly Dictionary<int, int[]> tattooColors;
        // Mapping earrings to ears
        private readonly Dictionary<int, int[]> earringEars;

        public BloodElfFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 115, new int[] { 1768, 1771, 1776 } },
                { 141, new int[] { 1758, 1759, 1760, 1761, 1762, 1763, 1764, 1765, 1766, 1767,
                    1768, 1769, 1770, 1771, 1772, 1773, 1774, 1775, 1776, 1777 } },
                { 143, new int[] { 1758, 1759, 1760, 1761, 1762, 1763 } },
                { 646, new int[] { 1758, 1759, 1760, 1761, 1762, 1763, 1764, 1765, 1766, 1767 } }
            };
            skinColorEyes = new()
            {
                { 666, new int[] { 1747 } },
                { 667, new int[] { 1734, 1740, 1733, 1739, 1736, 1738, 1737, 1732,
                    1741, 1735, 6690, 6689, 6688, 6687, 1748, 1750, 1749 } },
                { 668, new int[] { 1734, 1740, 1733, 1739, 1736, 1738, 1737, 1732, 1741, 1735,
                    6690, 6689, 6688, 6687, 1756, 1751, 1755, 1753, 1752, 1754 } },
                { 669, new int[] { 1734, 1740, 1733, 1739, 1736, 1738, 1737,
                    1732, 1741, 1735, 6690, 6689, 6688, 6687 } }
            };
            tattooColors = new()
            {
                { 229, new int[] { 1837 } },
                { 230, new int[] { 1838, 1839, 1840, 1841, 1842, 3714 } }
            };
            earringEars = new()
            {
                { 137, new int[] { 6719 } },
                { 141, new int[] { 6719, 6720, 6721 } }
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
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Horns");
            ChangeEyeColor(activeGeosets);
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Blindfold");
            ActivateRelatedGeosetOptions(activeGeosets, "Ears", "Earrings", earringEars);
            ChangeGeosetOption(activeGeosets, "Earrings");
            ChangeGeosetOption(activeGeosets, "Necklace");
            ChangeRelatedTextureOptions("Tattoo", "Tattoo Color", tattooColors);
            ChangeGeosetOption(activeGeosets, "Armbands");
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
            DrawLayer(texture, "Face", "Skin Color", 512, 0, 512, 512);
            DrawLayer(texture, "Tattoo Color", "Tattoo", 0, 0, 512, 512);
            DrawEmission("Tattoo Color", "Tattoo", 0, 0, 512, 512);
            DrawBra(texture);
            DrawUnderwear(texture);
            DrawJewelry(texture, "Bracelets", 26, 0, 0, 512, 512);
            DrawLayer(texture, "Hair Style", "Hair Color", 512, 0, 512, 512);
            DrawArmor(texture);
        }
    }
}

using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle mechagnome male customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class MechagnomeMale : CharacterHelper
    {
        // Mapping faces to skin colors
        private readonly Dictionary<int, int[]> skinColorFaces;

        public MechagnomeMale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
            skinColorFaces = new()
            {
                { 141, new int[] { 8966, 8967, 8968, 8969, 8970, 8971, 8972, 8973, 8974, 8975, 8976, 8977, 8978, 8979 } },
                { 191, new int[] { 8973, 8975, 8976 } }
            };
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(List<int> activeGeosets)
        {
            Character.racial.ActiveGeosets.Clear();
            ChangeFace(activeGeosets);
            ChangeEyes(activeGeosets);
            ActivateRelatedTextureOptions("Skin Color", "Face", skinColorFaces);
            ChangeGeosetOption(activeGeosets, "Hair Style");
            ChangeGeosetOption(activeGeosets, "Facial Hair");
            ChangeModification(activeGeosets);
            ChangeEyeColor(activeGeosets);
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Arm Upgrade");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Leg Upgrade");
        }

        // Chamge head modification
        private void ChangeModification(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 699 && x < 800);
            ChangeGeosetOption(activeGeosets, "Modification");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Modification");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            RectInt face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawUnderwear(texture);
            DrawLayer(texture, "Hair Color", 8, face);
            DrawArmor(texture, true);
        }

        // Get id of Skin Color Extra option
        protected override int GetSkinExtraIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Paint");
        }

        // Get id of Armor Color option
        public override int GetArmorColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Paint");
        }
    }
}

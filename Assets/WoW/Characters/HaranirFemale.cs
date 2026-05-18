using M2Lib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoW.Characters
{
    // Class to handle troll female customization
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class HaranirFemale : CharacterHelper
    {
        public HaranirFemale(M2 model, Character character, ComputeShader shader)
        {
#if UNITY_EDITOR
            textures = new();
#endif
            Model = model;
            Character = character;
            layerShader = shader;
        }

        // Change geosets according to chosen character customization
        public override void ChangeGeosets(HashSet<int> activeGeosets)
        {
            ChangeEyes(activeGeosets);
            ChangeUnderwear(Character.racial.ActiveGeosets);
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Hair Style");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Eyebrows");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Eyelashes");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Hair Spine Style", "Hair Style");
            ChangeGeosetOption(activeGeosets, "Nose");
            ChangeGeosetOption(activeGeosets, "Tusk");
            ChangeGeosetOption(activeGeosets, "Ears");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Earrings", "Ears");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Necklace");
            ChangeSkinnedGeosetOption(Character.racial.ActiveGeosets, "Shoulder Spine");
            ChangeGeosetOption(activeGeosets, "Underclothes Top");
        }

        // Generate skin texture from many layers
        public override void LayeredTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            textures.Clear();
#endif
            var face = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Face);
            var body = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Body);
            var full = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Full);
            var foot = WoWHelper.ComponentRect(WoWHelper.ComponentSection.Foot);
            DrawLayer(texture, "Face", "Skin Color", face);
            DrawLayer(texture, "Feet", "Skin Color", foot);
            DrawLayer(texture, "Fur Color", "Face Fur Pattern", face);
            DrawLayer(texture, "Fur Color", "Body Fur Pattern", full);
            DrawLayer(texture, "Paint Color", "Face Paint", face);
            DrawLayer(texture, "Paint Color", "Body Paint", body);
            DrawBra(texture, "Underclothes Color");
            DrawUnderwear(texture, "Underclothes Color");
            DrawArmor(texture, true);
        }

        // Draw layers on hair texture
        public override void LayeredHair(Texture2D texture)
        {
            RectInt rect = new(0, 0, texture.width, texture.height);
            DrawLayer(texture, "Hair Highlight Color", 11, rect);
        }

        // Get id of Skin Color Extra option
        public override int GetSkinExtraIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Hair Spine Color");
        }

        // Get id of Ornament Color option
        public override int GetOrnamentColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Underclothes Color");
        }

        // Get id of Jewelry Color option
        public override int GetJewelryColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Jewelry");
        }

        // Get id of Armor Color option
        public override int GetArmorColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Shoulder Spine Color");
        }
    }
}

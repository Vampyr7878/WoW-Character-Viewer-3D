﻿using Assets.WoW;
using M2Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace WoW
{
    // Abstact class for handling character customization for each specific race
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public abstract class CharacterHelper
    {
#if UNITY_EDITOR
        // List of loaded textures for debugging preview
        public List<Texture2D> textures;
#endif

        // Compute shader to handle texture layers
        protected ComputeShader layerShader;

        // All the data from m2 file
        public M2 Model { get; set; }
        // Emission texture used for glowing tattoos
        public Texture2D Emission { get; protected set; }
        // Reference to a main class that handles character models
        public Character Character { get; protected set; }

        // Draw Texture on top of another
        public void DrawTexture(Texture2D texture, Texture2D layer, int x, int y)
        {
#if UNITY_EDITOR
            textures.Add(layer);
#endif
            RenderTexture result = new(texture.width, texture.height, 0)
            {
                enableRandomWrite = true
            };
            result.Create();
            Graphics.Blit(texture, result);
            int kernel = layerShader.FindKernel("DrawTexture");
            layerShader.SetTexture(kernel, "Result", result);
            layerShader.SetTexture(kernel, "Layer", layer);
            layerShader.SetInts("Offset", x, y);
            layerShader.SetInts("Size", layer.width, layer.height);
            layerShader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);
            RenderTexture.active = result;
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
        }

        // Draw Texture twice on top of another
        public void DoubleTexture(Texture2D texture, Texture2D layer, int x, int y)
        {
            RenderTexture result = new(texture.width, texture.height, 0)
            {
                enableRandomWrite = true
            };
            result.Create();
            Graphics.Blit(texture, result);
            int kernel = layerShader.FindKernel("DoubleTexture");
            layerShader.SetTexture(kernel, "Result", result);
            layerShader.SetTexture(kernel, "Layer", layer);
            layerShader.SetInts("Offset", x, y);
            layerShader.SetInts("Size", layer.width, layer.height);
            layerShader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);
            RenderTexture.active = result;
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
        }

        // Overlay textures together
        protected void OverlayTexture(Texture2D texture, Texture2D layer, int x, int y)
        {
            RenderTexture result = new(texture.width, texture.height, 0)
            {
                enableRandomWrite = true
            };
            result.Create();
            Graphics.Blit(texture, result);
            int kernel = layerShader.FindKernel("OverlayTexture");
            layerShader.SetTexture(kernel, "Result", result);
            layerShader.SetTexture(kernel, "Layer", layer);
            layerShader.SetInts("Offset", x, y);
            layerShader.SetInts("Size", layer.width, layer.height);
            layerShader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);
            RenderTexture.active = result;
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
        }

        // Multiply textures
        protected void MultiplyTexture(Texture2D texture, Texture2D layer, int x, int y)
        {
            RenderTexture result = new(texture.width, texture.height, 0)
            {
                enableRandomWrite = true
            };
            result.Create();
            Graphics.Blit(texture, result);
            int kernel = layerShader.FindKernel("MultiplyTexture");
            layerShader.SetTexture(kernel, "Result", result);
            layerShader.SetTexture(kernel, "Layer", layer);
            layerShader.SetInts("Offset", x, y);
            layerShader.SetInts("Size", layer.width, layer.height);
            layerShader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);
            RenderTexture.active = result;
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
        }

        // Generate black texture
        public void BlackTexture(Texture2D src, Texture2D dst)
        {
            RenderTexture result = new(dst.width, dst.height, 0)
            {
                enableRandomWrite = true
            };
            result.Create();
            Graphics.Blit(dst, result);
            int kernel = layerShader.FindKernel("BlackTexture");
            layerShader.SetTexture(kernel, "Result", result);
            layerShader.SetTexture(kernel, "Layer", src);
            layerShader.Dispatch(kernel, dst.width / 8, dst.height / 8, 1);
            RenderTexture.active = result;
            dst.ReadPixels(new Rect(0, 0, dst.width, dst.height), 0, 0);
            dst.Apply();
            RenderTexture.active = null;
        }

        // Multiply textures
        protected Texture2D ScaleTexture(Texture2D texture, int width, int height)
        {
            if (texture.width == width && texture.height == height)
            {
                return texture;
            }
            RenderTexture result = new(width, height, 0)
            {
                enableRandomWrite = true
            };
            result.Create();
            int kernel = layerShader.FindKernel("ScaleTexture");
            layerShader.SetTexture(kernel, "Result", result);
            layerShader.SetTexture(kernel, "Layer", texture);
            layerShader.SetInts("Size", texture.width, texture.height);
            layerShader.SetInts("Target", width, height);
            layerShader.Dispatch(kernel, width / 8, height / 8, 1);
            Texture2D scaled = new(width, height, TextureFormat.ARGB32, true);
            RenderTexture.active = result;
            scaled.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            scaled.Apply();
            RenderTexture.active = null;
            return scaled;
        }

        // Draw texture layer with given name and target
        protected void DrawLayer(Texture2D texture, string name, int target, RectInt rect)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.LastOrDefault(t => t.Target == target && t.Usage == 0);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID);
                layerTexture = ScaleTexture(layerTexture, rect.width, rect.height);
                DrawTexture(texture, layerTexture, rect.x, rect.y);
            }
        }

        // Draw texture layer with given names
        protected void DrawLayer(Texture2D texture, string name, string name2, RectInt rect)
        {
            int index, index2;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.
                FirstOrDefault(t => t.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID);
                layerTexture = ScaleTexture(layerTexture, rect.width, rect.height);
                DrawTexture(texture, layerTexture, rect.x, rect.y);
            }
        }

        // Draw texture layer with given names and target
        protected void DrawLayer(Texture2D texture, string name, string name2, int target, RectInt rect)
        {
            int index, index2;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.
                FirstOrDefault(t => t.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID && t.Target == target);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID);
                layerTexture = ScaleTexture(layerTexture, rect.width, rect.height);
                DrawTexture(texture, layerTexture, rect.x, rect.y);
            }
        }

        // Draw texture layer twice with given name and target
        protected void DoubleLayer(Texture2D texture, string name, int target, RectInt rect)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 0);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID);
                layerTexture = ScaleTexture(layerTexture, rect.width, rect.height);
                DoubleTexture(texture, layerTexture, rect.x, rect.y);
            }
        }

        // Overlay texture layer with given name and target
        protected void OverlayLayer(Texture2D texture, string name, int target, RectInt rect)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 0);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID);
                layerTexture = ScaleTexture(layerTexture, rect.width, rect.height);
                OverlayTexture(texture, layerTexture, rect.x, rect.y);
            }
        }

        // Multiply texture layer with given name and target
        protected void MultiplyLayer(Texture2D texture, string name, int target, RectInt rect)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 0);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID);
                layerTexture = ScaleTexture(layerTexture, rect.width, rect.height);
                MultiplyTexture(texture, layerTexture, rect.x, rect.y);
            }
        }

        // Initialize emission texture if it's null
        private void InitEmissionTexture()
        {
            if (Emission != null)
            {
                return;
            }
            Texture2D temp = Resources.Load<Texture2D>("Materials/Emission");
            Emission = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
            Emission.SetPixels32(temp.GetPixels32());
            Emission.Apply();
        }

        // Draw layer on emission texture with given name and target
        protected void DrawEmission(string name, int target, RectInt rect)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 2);
            if (layer != null)
            {
                InitEmissionTexture();
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID);
                layerTexture = ScaleTexture(layerTexture, rect.width, rect.height);
                DrawTexture(Emission, layerTexture, rect.x, rect.y);
            }
        }

        // Draw layer on emission texture with given names
        protected void DrawEmission(string name, string name2, RectInt rect)
        {
            int index, index2;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.
                FirstOrDefault(t => t.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID && t.Usage == 2);
            if (layer != null)
            {
                InitEmissionTexture();
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID);
                layerTexture = ScaleTexture(layerTexture, rect.width, rect.height);
                DrawTexture(Emission, layerTexture, rect.x, rect.y);
            }
        }

        // Draw layer on emission texture twcie with given name and target
        protected void DoubleEmission(string name, int target, RectInt rect)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 2);
            if (layer != null)
            {
                InitEmissionTexture();
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID);
                layerTexture = ScaleTexture(layerTexture, rect.width, rect.height);
                DoubleTexture(Emission, layerTexture, rect.x, rect.y);
            }
        }

        // Draw jewelry layer and use it to mask the emission
        protected void DrawJewelry(Texture2D texture, string name, int target, RectInt rect)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target);
            if (layer != null)
            {
                Texture2D jewelry = Character.TextureFromBLP(layer.ID);
                jewelry = ScaleTexture(jewelry, rect.width, rect.height);
                DrawTexture(texture, jewelry, rect.x, rect.y);
                if (Emission != null)
                {
                    Texture2D temp = new(jewelry.width, jewelry.height, TextureFormat.ARGB32, false);
                    BlackTexture(jewelry, temp);
                    temp.Apply();
                    DrawTexture(Emission, temp, rect.x, rect.y);
                }
            }
        }

        // Draw bra layer and use it to mask the emission
        protected void DrawBra(Texture2D texture, string name = "Skin Color")
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            if (Character.Items[(int)WoWHelper.SlotIndex.Chest] == null && Character.Items[(int)WoWHelper.SlotIndex.Shirt]
                == null && Character.Items[(int)WoWHelper.SlotIndex.Tabard] == null)
            {
                layer = Character.Options[index].Choices[Character.Customization[index]].Textures.First(t => t.Target == 14);
                RectInt rect = WoWHelper.ComponentRect(WoWHelper.ComponentSection.TorsoUpper);
                Texture2D bra = Character.TextureFromBLP(layer.ID);
                bra = ScaleTexture(bra, rect.width, rect.height);
                DrawTexture(texture, bra, rect.x, rect.y);
                if (Emission != null)
                {
                    Texture2D temp = new(bra.width, bra.height, TextureFormat.ARGB32, false);
                    BlackTexture(bra, temp);
                    temp.Apply();
                    DrawTexture(Emission, temp, rect.x, rect.y);
                }
            }
        }

        // Draw bra layer and use it to mask the emission
        protected void DrawBra(Texture2D texture, string name, string name2)
        {
            int index, index2;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            if (Character.Items[(int)WoWHelper.SlotIndex.Chest] == null && Character.Items[(int)WoWHelper.SlotIndex.Shirt]
                == null && Character.Items[(int)WoWHelper.SlotIndex.Tabard] == null)
            {
                layer = Character.Options[index].Choices[Character.Customization[index]].Textures.
                    FirstOrDefault(t => t.Target == 14 && t.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID);
                if (layer == null)
                {
                    return;
                }
                RectInt rect = WoWHelper.ComponentRect(WoWHelper.ComponentSection.TorsoUpper);
                Texture2D bra = Character.TextureFromBLP(layer.ID);
                bra = ScaleTexture(bra, rect.width, rect.height);
                DrawTexture(texture, bra, rect.x, rect.y);
                if (Emission != null)
                {
                    Texture2D temp = new(bra.width, bra.height, TextureFormat.ARGB32, false);
                    BlackTexture(bra, temp);
                    temp.Apply();
                    DrawTexture(Emission, temp, rect.x, rect.y);
                }
            }
        }

        // Draw underwear layer and use it to mask the emission
        protected void DrawUnderwear(Texture2D texture, string name = "Skin Color")
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            if (!(Character.Items[(int)WoWHelper.SlotIndex.Chest] != null && Character.Items[(int)WoWHelper.SlotIndex.Chest].Item.ItemSlot
                == WoWHelper.ItemSlot.Robe) && Character.Items[(int)WoWHelper.SlotIndex.Legs] == null)
            {
                layer = Character.Options[index].Choices[Character.Customization[index]].Textures.First(t => t.Target == 13);
                RectInt rect = WoWHelper.ComponentRect(WoWHelper.ComponentSection.LegUpper);
                Texture2D underwear = Character.TextureFromBLP(layer.ID);
                underwear = ScaleTexture(underwear, rect.width, rect.height);
                DrawTexture(texture, underwear, rect.x, rect.y);
                if (Emission != null)
                {
                    Texture2D temp = new(underwear.width, underwear.height, TextureFormat.ARGB32, false);
                    BlackTexture(underwear, temp);
                    temp.Apply();
                    DrawTexture(Emission, temp, rect.x, rect.y);
                }
            }
        }

        // Draw underwear layer and use it to mask the emission
        protected void DrawUnderwear(Texture2D texture, string name, string name2)
        {
            int index, index2;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            if (!(Character.Items[(int)WoWHelper.SlotIndex.Chest] != null && Character.Items[(int)WoWHelper.SlotIndex.Chest].Item.ItemSlot
                == WoWHelper.ItemSlot.Robe) && Character.Items[(int)WoWHelper.SlotIndex.Legs] == null)
            {
                layer = Character.Options[index].Choices[Character.Customization[index]].Textures.
                    First(t => t.Target == 13 && t.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID);
                RectInt rect = WoWHelper.ComponentRect(WoWHelper.ComponentSection.LegUpper);
                Texture2D underwear = Character.TextureFromBLP(layer.ID);
                underwear = ScaleTexture(underwear, rect.width, rect.height);
                DrawTexture(texture, underwear, rect.x, rect.y);
                if (Emission != null)
                {
                    Texture2D temp = new(underwear.width, underwear.height, TextureFormat.ARGB32, false);
                    BlackTexture(underwear, temp);
                    temp.Apply();
                    DrawTexture(Emission, temp, rect.x, rect.y);
                }
            }
        }

        // Draw armor textures
        protected void DrawArmor(Texture2D texture, bool showFeet = false)
        {
            bool shirtSleeves = CheckSleeves((int)WoWHelper.SlotIndex.Chest);
            bool gloveSleeves = CheckSleeves((int)WoWHelper.SlotIndex.Hands);
            bool isKilt = IsKilt((int)WoWHelper.SlotIndex.Legs);
            if (Emission != null)
            {
                BlackArmor(WoWHelper.SlotIndex.Chest, Emission);
                BlackArmor(WoWHelper.SlotIndex.Shirt, Emission);
                BlackArmor(WoWHelper.SlotIndex.Tabard, Emission);
                BlackArmor(WoWHelper.SlotIndex.Wrist, Emission);
                BlackArmor(WoWHelper.SlotIndex.Hands, Emission);
                BlackArmor(WoWHelper.SlotIndex.Waist, Emission);
                BlackArmor(WoWHelper.SlotIndex.Legs, Emission);
                BlackArmor(WoWHelper.SlotIndex.Feet, Emission, showFeet);
                Emission.Apply();
            }
            TextureArmor(WoWHelper.SlotIndex.Shirt ,texture);
            if (!isKilt)
            {
                TextureArmor(WoWHelper.SlotIndex.Legs, texture);
            }
            TextureArmor(WoWHelper.SlotIndex.Feet, texture, showFeet);
            if(isKilt)
            {
                TextureArmor(WoWHelper.SlotIndex.Legs, texture);
            }
            if (shirtSleeves)
            {
                TextureArmor(WoWHelper.SlotIndex.Wrist, texture);
            }
            if (!gloveSleeves)
            {
                TextureArmor(WoWHelper.SlotIndex.Hands, texture);
            }
            TextureArmor(WoWHelper.SlotIndex.Chest, texture);
            if (!shirtSleeves)
            {
                TextureArmor(WoWHelper.SlotIndex.Wrist, texture);
            }
            if (gloveSleeves)
            {
                TextureArmor(WoWHelper.SlotIndex.Hands, texture);
            }
            TextureArmor(WoWHelper.SlotIndex.Tabard, texture);
            TextureArmor(WoWHelper.SlotIndex.Waist, texture);
        }

        // Draw texture layer for armor slot
        private void TextureArmor(WoWHelper.SlotIndex slot, Texture2D texture, bool showFeet = false)
        {
            ItemInstance item = Character.Items[(int)slot];
            if (item == null || item.Item.Appearances[item.Appearance].DisplayInfo == null)
            {
                return;
            }
            int? file = null;
            foreach (var component in item.Item.Appearances[item.Appearance].DisplayInfo.Components)
            {
                if (showFeet && component.ComponentSection == WoWHelper.ComponentSection.Foot)
                {
                    continue;
                }
                file = component.Textures.FirstOrDefault(t => (int)t.Race == Character.ModelID
                    && (t.Gender == (Character.Gender ? 0 : 1) || t.Gender == 3))?.ID;
                file ??= component.Textures.FirstOrDefault(t => t.Class == Character.Class
                    && (t.Gender == (Character.Gender ? 0 : 1) || t.Gender == 3))?.ID;
                file ??= component.Textures.FirstOrDefault(t => t.Gender == (Character.Gender ? 0 : 1) || t.Gender == 3)?.ID;
                if (file != null)
                {
                    RectInt rect = WoWHelper.ComponentRect(component.ComponentSection);
                    Texture2D layer = Character.TextureFromBLP(file.Value);
                    layer = ScaleTexture(layer, rect.width, rect.height);
                    DrawTexture(texture, layer, rect.x, rect.y);
                }
            }
        }

        // Check type of sleeves to decide order of texture layers
        private bool CheckSleeves(int slot)
        {
            ItemInstance item = Character.Items[slot];
            return item != null && item.Item.Appearances[item.Appearance].DisplayInfo.Geosets[0] != 0;
        }

        // Check if character wears a kilt
        private bool IsKilt(int slot)
        {
            ItemInstance item = Character.Items[slot];
            return item != null && item.Item.Appearances[item.Appearance].DisplayInfo.Geosets[2] != 0;
        }

        private void BlackArmor(WoWHelper.SlotIndex slot, Texture2D texture, bool showFeet = false)
        {
            ItemInstance item = Character.Items[(int)slot];
            if (item == null || item.Item.Appearances[item.Appearance].DisplayInfo == null)
            {
                return;
            }
            int? file = null;
            foreach (var component in item.Item.Appearances[item.Appearance].DisplayInfo.Components)
            {
                if (showFeet && component.ComponentSection == WoWHelper.ComponentSection.Foot)
                {
                    continue;
                }
                file = component.Textures.FirstOrDefault(t => (int)t.Race == Character.ModelID
                    && (t.Gender == (Character.Gender ? 0 : 1) || t.Gender == 3))?.ID;
                file ??= component.Textures.FirstOrDefault(t => t.Class == Character.Class
                    && (t.Gender == (Character.Gender ? 0 : 1) || t.Gender == 3))?.ID;
                file ??= component.Textures.FirstOrDefault(t => t.Gender == (Character.Gender ? 0 : 1) || t.Gender == 3)?.ID;
                if (file != null)
                {
                    RectInt rect = WoWHelper.ComponentRect(component.ComponentSection);
                    Texture2D layer = Character.TextureFromBLP(file.Value);
                    layer = ScaleTexture(layer, rect.width, rect.height);
                    BlackTexture(layer, layer);
                    layer.Apply();
                    DrawTexture(texture, layer, rect.x, rect.y);
                }
            }
        }

        // Add geoset to rendered list
        private void AddGeoset(List<int> activeGeosets, int type, int geoset, int modifier)
        {
            ItemInstance item = Character.Items[(int)WoWHelper.SlotIndex.Head];
            activeGeosets.RemoveAll(x => x > Math.Clamp(type * 100 - 1, 0, 9999) && x < (type + 1) * 100);
            if (item != null && item.Item.Appearances[item.Appearance].DisplayInfo?.HelmetGeosets[Character.Gender ? 0 : 1]
                .FirstOrDefault(h => h.Race == Character.Race && h.Geoset == type) != null)
            {
                activeGeosets.Add(modifier == -1 ? type * 100 + (type == 0 ? 1 : 0) : type * 100 + modifier);
            }
            else
            {
                activeGeosets.Add(type * 100 + geoset);
            }
        }

        // Make sure head geoset is visible
        protected void ChangeFace(List<int> activeGeosets)
        {
            AddGeoset(activeGeosets, 32, 2, 1);
        }

        // Make sure eyes geosets are visible
        protected void ChangeEyes(List<int> activeGeosets)
        {
            AddGeoset(activeGeosets, 33, 1, -1);
        }

        // Make sure ears geosets are visible
        protected void ChangeEars(List<int> activeGeosets)
        {
            AddGeoset(activeGeosets, 7, 2, 1);
        }

        // Make sure hand geosets are visible
        protected void ChangeHands(List<int> activeGeosets)
        {
            AddGeoset(activeGeosets, 23, 1, -1);
        }

        // Make sure underwear geoset is visible
        protected void ChangeUnderwear(List<int> activeGeosets)
        {
            AddGeoset(activeGeosets, 14, 1, -1);
        }

        // Make sure underwear geoset is visible for demon hunter
        protected void ChangeDHUnderwear(List<int> activeGeosets)
        {
            if (Character.Class == WoWHelper.Class.DemonHunter)
            {
                AddGeoset(activeGeosets, 14, 1, -1);
            }
            else
            {
                AddGeoset(activeGeosets, 14, 0, -1);
            }
        }

        // Change goesets in according to eye color and make sure left over geosets are removed
        protected void ChangeEyeColor(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 5099 && x < 5200);
            ChangeGeosetOption(activeGeosets, "Eye Color");
        }

        // Make sure wings geoset is visible
        protected void ChangeWings(List<int> activeGeosets)
        {
            AddGeoset(activeGeosets, 46, 1, -1);
        }

        // Activate and deactivate dropdown options based on dependencies
        protected void ActivateRelatedTextureOptions(string name, string name2)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int id = Character.Options[index].Choices[Character.Customization[index]].ID;
            if (Character.Options[index2].Choices[Character.Customization[index2]].Textures.FirstOrDefault(t => t.Related == id) == null)
            {
                var choice = Character.Options[index2].Choices.FirstOrDefault(c => c.Value.Textures.FirstOrDefault(t => t.Related == id) != null);
                if (choice.Value != null)
                {
                    Character.CustomizationDropdowns[index2].SetValue(choice.Key);
                }
            }
            Character.ActivateRelatedChoices(index, index2);
        }

        // Activate and deactivate dropdown options based on list of ids
        public void ActivateRelatedTextureOptions(string name, string name2, Dictionary<int, int[]> reqRelations)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int[] choices = reqRelations[Character.Options[index].Choices[Character.Customization[index]].Requirement];
            if (!choices.Contains(Character.Options[index2].Choices[Character.Customization[index2]].ID))
            {
                var choice = Character.Options[index2].Choices.First(c => choices.Contains(c.Value.ID));
                Character.CustomizationDropdowns[index2].SetValue(choice.Key);
            }
            Character.ActivateUsingIds(index2, choices);
        }

        // Make sure dropdown only contains options that match given requirements
        public void ChangeRelatedTextureOptions(string name, string name2, Dictionary<int, int[]> reqRelations)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int[] requirements = reqRelations.Where(x => x.Value.Contains(Character.Options[index].Choices[Character.Customization[index]].ID)).
                Select(x => x.Key).ToArray();
            Character.Options[index2].Choices = Character.Options[index2].AllChoices.
                Where(c => requirements.Contains(c.Value.Requirement)).ToDictionary(c => c.Key, c => c.Value);
            Character.ChangeDropdownOptions(index2);
        }

        // Activate and deactivate dropdown options based on requirements
        protected void ActivateRelatedGeosetOptions(List<int> activeGeosets, string name, string name2, Dictionary<int, int[]> reqRelations)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int[] requirements = reqRelations.Where(x => x.Value.Contains(Character.Options[index].Choices[Character.Customization[index]].ID)).
                Select(x => x.Key).ToArray();
            if (!requirements.Contains(Character.Options[index2].Choices[Character.Customization[index2]].Requirement))
            {
                var choice = Character.Options[index2].Choices.First(c => requirements.Contains(c.Value.Requirement));
                Character.CustomizationDropdowns[index2].SetValue(choice.Key);
            }
            Character.ActivateUsingRequirmenets(index2, requirements);
            ChangeGeosetOption(activeGeosets, name);
        }

        // Make sure dropdown only contains options that match given requirements
        protected void ChangeRelatedGeosetOptions(List<int> activeGeosets, string name, string name2, Dictionary<int, int[]> reqRelations)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int[] requirements = reqRelations.Where(x => x.Value.Contains(Character.Options[index].Choices[Character.Customization[index]].ID)).
                Select(x => x.Key).ToArray();
            Character.Options[index2].Choices = Character.Options[index2].AllChoices.
                Where(c => requirements.Contains(c.Value.Requirement)).ToDictionary(c => c.Key, c => c.Value);
            Character.ChangeDropdownOptions(index2);
            ChangeGeosetOption(activeGeosets, name);
        }

        // Make sure dropdown only contains options for current race
        public void ChangeRacialOptions(string name, int[] requirements)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name);
            Character.Options[index].Choices = Character.Options[index].AllChoices.
                Where(c => requirements.Contains(c.Value.Requirement)).ToDictionary(c => c.Key, c => c.Value);
            Character.ChangeDropdownOptions(index);
        }

        // Change visible geosets for given option
        public void ChangeGeosetOption(List<int> activeGeosets, string name)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            var geosets = Character.Options[index].Choices[Character.Customization[index]].Geosets.Where(g => g.Related == 0);
            foreach (var geoset in geosets)
            {
                AddGeoset(activeGeosets, geoset.Type, geoset.ID, geoset.Modifier);
            }
        }

        // Change visible geosets for given option
        protected void ChangeGeosetOption(List<int> activeGeosets, string name, string name2)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            var geosets = Character.Options[index].Choices[Character.Customization[index]].Geosets.
                Where(g => g.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID);
            foreach (var geoset in geosets)
            {
                AddGeoset(activeGeosets, geoset.Type, geoset.ID, geoset.Modifier);
            }
        }

        // Change visible geosets for given option on skinned model
        protected void ChangeSkinnedGeosetOption(List<int> activeGeosets, string name)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            var geosets = Character.Options[index].Choices[Character.Customization[index]].SkinnedGeosets.Where(g => g.Related == 0);
            foreach (var geoset in geosets)
            {
                AddGeoset(activeGeosets, geoset.Type, geoset.ID, geoset.Modifier);
            }
        }

        // Change visible geosets for given option on skinned model
        protected void ChangeSkinnedGeosetOption(List<int> activeGeosets, string name, string name2)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            var geosets = Character.Options[index].Choices[Character.Customization[index]].SkinnedGeosets.
                Where(g => g.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID);
            foreach (var geoset in geosets)
            {
                AddGeoset(activeGeosets, geoset.Type, geoset.ID, geoset.Modifier);
            }
        }

        // Make sure dropdown only contains options that match given requirements
        protected void ChangeRelatedSkinnedGeosetOption(List<int> activeGeosets, string name, string name2, Dictionary<int, int[]> reqRelations)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int[] requirements = reqRelations.Where(x => x.Value.Contains(Character.Options[index].Choices[Character.Customization[index]].ID)).
                Select(x => x.Key).ToArray();
            Character.Options[index2].Choices = Character.Options[index2].AllChoices.Where(c => requirements.Contains(c.Value.Requirement)).
                ToDictionary(c => c.Key, c => c.Value);
            Character.ChangeDropdownOptions(index2);
            ChangeSkinnedGeosetOption(activeGeosets, name2, name);
        }

        // Hide options available to other form
        protected void HideOtherFormsOptions()
        {
            for (int i = 0; i < Character.Options.Length; i++)
            {
                bool active = Character.Options[i].Model == Character.ModelID && Character.Options[i].Category == Character.Category
                    && Character.Options[i].Choices.Count > 1;
                if (Character.Options[i].Type == 0)
                {
                    Character.CustomizationDropdowns[i].transform.parent.gameObject.SetActive(active);
                }
                else
                {
                    Character.CustomizationToggles[i].transform.parent.gameObject.SetActive(active);
                }
            }
        }

        // Swap model id for current form, needs to be overriden for some races
        public virtual void ChangeForm()
        {
            switch (Character.Form)
            {
                case 0:
                    Character.ModelID = Character.MainFormID;
                    Character.ActivateMainMesh();
                    break;
                default:
                    Character.ModelID = Character.CreatureForms[Character.Form - 1].ID;
                    Character.ActivateCreature();
                    break;
            }
        }

        // Load textures and store them to be used while rendering
        public void LoadTextures(Texture2D[] textures)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                int file = LoadTexture(Model.Textures[i], i, out WoWHelper.LayeredTexture layered);
                if (file <= 0)
                {
                    textures[i] = new Texture2D(200, 200);
                }
                else
                {
                    Texture2D texture = Character.TextureFromBLP(file);
                    textures[i] = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
                    textures[i].SetPixels32(texture.GetPixels32());
                    textures[i].wrapModeU = (Model.Textures[i].Flags & 1) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                    textures[i].wrapModeV = (Model.Textures[i].Flags & 2) != 0 ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                    textures[i].Apply();
                    switch (layered)
                    {
                        case WoWHelper.LayeredTexture.Skin:
                            LayeredTexture(textures[i]);
                            break;
                        case WoWHelper.LayeredTexture.Eye:
                            EyeTexture(textures[i]);
                            break;
                        case WoWHelper.LayeredTexture.Extra1:
                            LayeredExtra1(textures[i]);
                            break;
                        case WoWHelper.LayeredTexture.Extra2:
                            LayeredExtra2(textures[i]);
                            break;
                        case WoWHelper.LayeredTexture.Hair:
                            LayeredHair(textures[i]);
                            break;
                    }
                }
            }
        }

        // Generate eye texture
        protected void EyeTexture(Texture2D texture)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == "Eyesight" && o.Model == Character.ModelID);
            CustomizationTexture layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == 44);
            if (layer != null)
            {
                Texture2D eyesight = Character.TextureFromBLP(layer.ID);
                DrawTexture(texture, eyesight, 0, 0);
            }
        }

        // Load textures based on their types
        protected int LoadTexture(M2Texture texture, int i, out WoWHelper.LayeredTexture layered)
        {
            int? file = null;
            int index, index2;
            layered = WoWHelper.LayeredTexture.None;
            UnityEngine.Debug.Log($"Texture: {texture.Type}");
            switch (texture.Type)
            {
                case 0:
                    file = Model.TextureIDs[i];
                    break;
                case 1:
                    index = GetSkinColorIndex();
                    file = Character.Options[index].Choices[Character.Customization[index]].Textures.First(t => t.Target == 1).ID;
                    layered = WoWHelper.LayeredTexture.Skin;
                    break;
                case 2:
                    ItemInstance item = Character.Items[(int)WoWHelper.SlotIndex.Back];
                    file = item != null && item.Item.Appearances[item.Appearance].DisplayInfo.Materials[0] > 0
                        ? item.Item.Appearances[item.Appearance].DisplayInfo.Textures[0][0].ID : -1;
                    break;
                case 6:
                    index = GetHairColorIndex();
                    index2 = GetHairColor2Index();
                    file = index2 == -1 ? Character.Options[index].Choices[Character.Customization[index]].Textures[0].ID :
                        Character.Options[index].Choices[Character.Customization[index]].Textures.
                        FirstOrDefault(t => t.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID)?.ID;
                    layered = WoWHelper.LayeredTexture.Hair;
                    break;
                case 7:
                    index = GetHairExtraIndex();
                    file = Character.Race == WoWHelper.Race.Dracthyr ? Character.Options[index].Choices[Character.Customization[index]].Textures.
                        First(t => t.Target == 2).ID : Character.Options[index].Choices[Character.Customization[index]].Textures.
                        FirstOrDefault(t => t.Target == 11)?.ID;
                    layered = WoWHelper.LayeredTexture.Extra1;
                    break;
                case 8:
                    index = GetSkinExtraIndex();
                    file = Character.Options[index].Choices[Character.Customization[index]].Textures.First(t => t.Target == 2).ID;
                    layered = WoWHelper.LayeredTexture.Extra1;
                    break;
                case 10:
                    index = GetAccessoriesIndex();
                    file = Character.Race == WoWHelper.Race.Dracthyr ? Character.Options[index].Choices[Character.Customization[index]].Textures.
                        First(t => t.Target == 3).ID : Character.Options[index].Choices[Character.Customization[index]].Textures[0].ID;
                    layered = WoWHelper.LayeredTexture.Extra2;
                    break;
                case 19:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color" && o.Model == Character.ModelID);
                    index2 = Array.FindIndex(Character.Options, o => o.Name == "Eye Style" && o.Model == Character.ModelID);
                    file = Character.ModelID == 89 ? Character.Options[index].Choices[Character.Customization[index]].Textures.
                        First(t => t.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID)?.ID :
                        Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == 25)?.ID;
                    layered = WoWHelper.LayeredTexture.Eye;
                    break;
                case 20:
                    index = GetJewelryColorIndex();
                    file = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == 38)?.ID;
                    break;
            }
            return file == null ? -1 : file.Value;
        }

        // Get id of Skin Color option, can be overriden for specific race if name is different
        public virtual int GetSkinColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Skin Color" && o.Model == Character.ModelID);
        }

        // Get id of Skin Color Extra option, can be overriden for specific race if name is different
        protected virtual int GetSkinExtraIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Skin Color" && o.Model == Character.ModelID);
        }

        // Get id of Hair Color option, can be overriden for specific race if name is different
        public virtual int GetHairColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Hair Color" && o.Model == Character.ModelID);
        }

        // Get id of Second Hair Color option, can be overriden for specific race if name is different
        protected virtual int GetHairColor2Index()
        {
            return -1;
        }

        // Get id of Hair Color Extra option, can be overriden for specific race if name is different
        protected virtual int GetHairExtraIndex()
        {
            return 0;
        }

        // Get id of Accesories option, can be overriden for specific race if name is different
        protected virtual int GetAccessoriesIndex()
        {
            return 0;
        }

        // Get id of Jewelry Color option, can be overriden for specific race if name is different
        public virtual int GetJewelryColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Jewelry Color" && o.Model == Character.ModelID);
        }

        // Get id of Ornament Color option, can be overriden for specific race if name is different
        public virtual int GetOrnamentColorIndex()
        {
            return 0;
        }

        // Get id of Armor Color option, can be overriden for specific race if name is different
        public virtual int GetArmorColorIndex()
        {
            return 0;
        }

        // Get id of Armor Style option, can be overriden for specific race if name is different
        public virtual int GetArmorStyleIndex()
        {
            return 0;
        }

        // Draw Layers on extra skin texture
        protected virtual void LayeredExtra1(Texture2D texture)
        {
            return;
        }

        // Draw Layers on extra skin texture
        protected virtual void LayeredExtra2(Texture2D texture)
        {
            return;
        }

        // Draw Layers on hair texture
        protected virtual void LayeredHair(Texture2D texture)
        {
            return;
        }

        // Change geosets according to chosen character customization
        public abstract void ChangeGeosets(List<int> activeGeosets);

        // Generate skin texture from many layers
        public abstract void LayeredTexture(Texture2D texture);
    }
}

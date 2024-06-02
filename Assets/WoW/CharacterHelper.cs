using M2Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace WoW
{
    //Abstact class for handling character customization for each specific race
    public abstract class CharacterHelper
    {
        //All the data from m2 file
        public M2 Model { get; set; }
        //Emission texture used for glowing tattoos
        public Texture2D Emission { get; protected set; }
        //Reference to a main class that handles character models
        public Character Character { get; protected set; }
        //Hide hair when wearing helmet
        public bool HideHair { get; set; }

        //Draw Texture on top of another
        public void DrawTexture(Texture2D texture, Texture2D layer, int x, int y)
        {
            Color[] textureColors = texture.GetPixels(x, y, layer.width, layer.height);
            Color32[] layerColors = layer.GetPixels32();
            Color32[] resultColors = new Color32[layerColors.Length];
            for (int i = 0; i < layerColors.Length; i++)
            {
                Color32 textureColor = textureColors[i];
                Color32 layerColor = layerColors[i];
                resultColors[i] = Color32.Lerp(textureColor, layerColor, layerColor.a / 255f);
            }
            texture.SetPixels32(x, y, layer.width, layer.height, resultColors);
        }

        //Draw Texture twice on top of another
        public void DoubleTexture(Texture2D texture, Texture2D layer, int x, int y)
        {
            Color[] textureColors = texture.GetPixels(x, y, layer.width, layer.height);
            Color32[] layerColors = layer.GetPixels32();
            Color32[] resultColors = new Color32[layerColors.Length];
            for (int i = 0; i < layerColors.Length; i++)
            {
                Color32 textureColor = textureColors[i];
                Color32 layerColor = layerColors[i];
                resultColors[i] = Color32.Lerp(textureColor, layerColor, layerColor.a / 255f);
                textureColor = resultColors[i];
                resultColors[i] = Color32.Lerp(textureColor, layerColor, layerColor.a / 255f);
            }
            texture.SetPixels32(x, y, layer.width, layer.height, resultColors);
        }

        //Overlay textures together
        protected void OverlayTexture(Texture2D texture, Texture2D layer, int x, int y)
        {
            Color[] textureColors = texture.GetPixels(x, y, layer.width, layer.height);
            Color32[] layerColors = layer.GetPixels32();
            Color32[] resultColors = new Color32[layerColors.Length];
            for (int i = 0; i < layerColors.Length; i++)
            {
                Color32 textureColor = textureColors[i];
                Color32 layerColor = layerColors[i];
                textureColor.r = (byte)(textureColor.r / 255f * (textureColor.r + 2 * layerColor.r / 255f * (255f - textureColor.r)));
                textureColor.g = (byte)(textureColor.g / 255f * (textureColor.g + 2 * layerColor.g / 255f * (255f - textureColor.g)));
                textureColor.b = (byte)(textureColor.b / 255f * (textureColor.b + 2 * layerColor.b / 255f * (255f - textureColor.b)));
                textureColor.a = (byte)(textureColor.a / 255f * (textureColor.a + 2 * layerColor.a / 255f * (255f - textureColor.a)));
                resultColors[i] = new Color32(textureColor.r, textureColor.g, textureColor.b, textureColor.a);
            }
            texture.SetPixels32(x, y, layer.width, layer.height, resultColors);
        }

        //Multiply textures
        protected void MultiplyTexture(Texture2D texture, Texture2D layer, int x, int y)
        {
            Color[] textureColors = texture.GetPixels(x, y, layer.width, layer.height);
            Color[] layerColors = layer.GetPixels();
            Color32[] resultColors = new Color32[layerColors.Length];
            for (int i = 0; i < layerColors.Length; i++)
            {
                Color textureColor = textureColors[i];
                Color layerColor = layerColors[i];
                resultColors[i] = textureColor * layerColor;
            }
            texture.SetPixels32(x, y, layer.width, layer.height, resultColors);
        }

        //Generate black texture
        public void BlackTexture(Texture2D src, Texture2D dst)
        {
            Color32[] srcColors = src.GetPixels32();
            Color32[] dstColors = new Color32[srcColors.Length];
            for (int i = 0; i < srcColors.Length; i++)
            {
                Color32 color = srcColors[i];
                if (color.a > 0.5f)
                {
                    dstColors[i] = Color.black;
                }
                else
                {
                    color = Color.black;
                    color.a = 0;
                    dstColors[i] = color;
                }
            }
            dst.SetPixels32(dstColors);
        }

        //Draw texture layer with given name and target
        protected void DrawLayer(Texture2D texture, string name, int target, int x, int y, int width, int height)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.LastOrDefault(t => t.Target == target && t.Usage == 0);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID, width, height);
                DrawTexture(texture, layerTexture, x, y);
            }
        }

        //Draw texture layer with given names
        protected void DrawLayer(Texture2D texture, string name, string name2, int x, int y, int width, int height)
        {
            int index, index2;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.
                FirstOrDefault(t => t.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID, width, height);
                DrawTexture(texture, layerTexture, x, y);
            }
        }

        //Draw texture layer twice with given name and target
        protected void DoubleLayer(Texture2D texture, string name, int target, int x, int y, int width, int height)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 0);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID, width, height);
                DoubleTexture(texture, layerTexture, x, y);
            }
        }

        //Overlay texture layer with given name and target
        protected void OverlayLayer(Texture2D texture, string name, int target, int x, int y, int width, int height)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 0);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID, width, height);
                OverlayTexture(texture, layerTexture, x, y);
            }
        }

        //Multiply texture layer with given name and target
        protected void MultiplyLayer(Texture2D texture, string name, int target, int x, int y, int width, int height)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 0);
            if (layer != null)
            {
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID, width, height);
                MultiplyTexture(texture, layerTexture, x, y);
            }
        }

        //Initialize emission texture if it's null
        private void InitEmissionTexture()
        {
            if (Emission != null)
            {
                return;
            }
            Texture2D temp = Resources.Load<Texture2D>("Materials/Emission");
            Emission = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, false);
            Emission.SetPixels32(temp.GetPixels32());
        }

        //Draw layer on emission texture with given name and target
        protected void DrawEmission(string name, int target, int x, int y, int width, int height)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 2);
            if (layer != null)
            {
                InitEmissionTexture();
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID, width, height);
                DrawTexture(Emission, layerTexture, x, y);
            }
        }

        //Draw layer on emission texture twcie with given name and target
        protected void DoubleEmission(string name, int target, int x, int y, int width, int height)
        {
            int index;
            CustomizationTexture layer;
            Stopwatch stopwatch = new Stopwatch();
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target && t.Usage == 2);
            if (layer != null)
            {
                InitEmissionTexture();
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID, width, height);
                DoubleTexture(Emission, layerTexture, x, y);
            }
        }

        //Draw layer on emission texture with given names
        protected void DrawEmission(string name, string name2, int x, int y, int width, int height)
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
                Texture2D layerTexture = Character.TextureFromBLP(layer.ID, width, height);
                DrawTexture(Emission, layerTexture, x, y);
            }
        }

        //Draw jewelry layer and use it to mask the emission
        protected void DrawJewelry(Texture2D texture, string name, int target, int x, int y, int width, int height)
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == target);
            if (layer != null)
            {
                Texture2D jewelry = Character.TextureFromBLP(layer.ID, width, height);
                DrawTexture(texture, jewelry, x, y);
                if (Emission != null)
                {
                    Texture2D temp = new(jewelry.width, jewelry.height, TextureFormat.ARGB32, false);
                    BlackTexture(jewelry, temp);
                    temp.Apply();
                    DrawTexture(Emission, temp, x, y);
                }
            }
        }

        //Draw bra layer and use it to mask the emission
        protected void DrawBra(Texture2D texture, string name = "Skin Color")
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            //if (Character.Items[3] == null && Character.Items[4] == null && Character.Items[5] == null)
            //{
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.First(t => t.Target == 14);
            Texture2D bra = Character.TextureFromBLP(layer.ID, 256, 128);
            DrawTexture(texture, bra, 256, 384);
            if (Emission != null)
            {
                Texture2D temp = new(bra.width, bra.height, TextureFormat.ARGB32, false);
                BlackTexture(bra, temp);
                temp.Apply();
                DrawTexture(Emission, temp, 256, 384);
            }
            //}
        }

        //Draw underwear layer and use it to mask the emission
        protected void DrawUnderwear(Texture2D texture, string name = "Skin Color")
        {
            int index;
            CustomizationTexture layer;
            index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            //if (!(Character.Items[3] != null && Character.Items[3].UpperLeg !> 0) && Character.Items[10] == null)
            //{
            layer = Character.Options[index].Choices[Character.Customization[index]].Textures.First(t => t.Target == 13);
            Texture2D underwear = Character.TextureFromBLP(layer.ID, 256, 128);
            DrawTexture(texture, underwear, 256, 192);
            if (Emission != null)
            {
                Texture2D temp = new(underwear.width, underwear.height, TextureFormat.ARGB32, false);
                BlackTexture(underwear, temp);
                temp.Apply();
                DrawTexture(Emission, temp, 256, 192);
            }
            //}
        }

        //Draw armor textures
        protected void DrawArmor(Texture2D texture, bool shotFeet = false)
        {
            if (Emission != null)
            {
                //Character.BlackChest(Emission);
                //Character.BlackShirt(Emission);
                //Character.BlackTabard(Emission);
                //Character.BlackWrist(Emission);
                //Character.BlackHands(Emission);
                //Character.BlackWaist(Emission);
                //Character.BlackLegs(Emission);
                //Character.BlackFeet(Emission);
                Emission.Apply();
            }
            //Character.TextureShirt(texture);
            //if (!(Character.Items[4] != null && Character.Items[4].Geoset1 != 0))
            //{
            //    Character.TextureWrist(texture);
            //}
            //Character.TextureLegs(texture);
            //Character.TextureFeet(texture, showFeet);
            //Character.TextureChest(texture);
            //if (!(Character.Items[3] != null && Character.Items[3].Geoset1 != 0))
            //{
            //    Character.TextureWrist(texture);
            //}
            //Character.TextureHands(texture);
            //if (!(Character.Items[8] != null && Character.Items[8].Geoset1 != 0))
            //{
            //    Character.TextureChest(texture);
            //}
            //Character.TextureTabard(texture);
            //Character.TextureWaist(texture);
        }

        //Make sure head geoset is visible
        protected void ChangeFace(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 3199 && x < 3300);
            activeGeosets.Add(3202);
        }

        //Make sure eyes geosets are visible
        protected void ChangeEyes(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 3299 && x < 3400);
            activeGeosets.Add(3301);
        }

        //Make sure ears geosets are visible
        protected void ChangeEars(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 699 && x < 800);
            activeGeosets.Add(702);
        }

        //Make sure hand geosets are visible
        protected void ChangeHands(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 2299 && x < 2400);
            activeGeosets.Add(2301);
        }

        //Make sure underwear geoset is visible
        protected void ChangeUnderwear(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
            activeGeosets.Add(1401);
        }

        //Make sure underwear geoset is visible for demon hunter
        protected void ChangeDHUnderwear(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1399 && x < 1500);
            if (Character.Class == 12)
            {
                activeGeosets.Add(1401);
            }
            else
            {
                activeGeosets.Add(1400);
            }
        }

        //Change goesets in according to eye color and make sure left over geosets are removed
        protected void ChangeEyeColor(List<int> activeGeosets)
        {
            activeGeosets.RemoveAll(x => x > 1699 && x < 1800);
            activeGeosets.RemoveAll(x => x > 5099 && x < 5200);
            ChangeGeosetOption(activeGeosets, "Eye Color");
        }

        //Activate and deactivate dropdown options based on dependencies
        protected void ActivateRelatedTextureOptions(string name, string name2)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int id = Character.Options[index].Choices[Character.Customization[index]].ID;
            if (Character.Options[index2].Choices[Character.Customization[index2]].Textures.FirstOrDefault(t => t.Related == id) == null)
            {
                var choice = Character.Options[index2].Choices.FirstOrDefault(c => c.Textures.FirstOrDefault(t => t.Related == id) != null);
                if (choice != null)
                {
                    Character.CustomizationDropdowns[index2].SetValue(Array.IndexOf(Character.Options[index2].Choices, choice));
                }
            }
            Character.ActivateRelatedChoices(index, index2);
        }

        //Activate and deactivate dropdown options based on list of ids
        protected void ActivateRelatedTextureOptions(string name, string name2, Dictionary<int, int[]> reqRelations)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int[] choices = reqRelations[Character.Options[index].Choices[Character.Customization[index]].Requirement];
            if (!choices.Contains(Character.Options[index2].Choices[Character.Customization[index2]].ID))
            {
                var choice = Character.Options[index2].Choices.First(c => choices.Contains(c.ID));
                Character.CustomizationDropdowns[index2].SetValue(Array.IndexOf(Character.Options[index2].Choices, choice));
            }
            Character.ActivateUsingIds(index2, choices);
        }

        //Make sure dropdown only contains options that match given requirements
        protected void ChangeRelatedTextureOptions(string name, string name2, Dictionary<int, int[]> reqRelations)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int requirement = reqRelations.First(x => x.Value.Contains(Character.Options[index].Choices[Character.Customization[index]].ID)).Key;
            Character.Options[index2].SetChoices(Character.Options[index2].AllChoices.Where(c => c.Requirement == requirement).ToArray());
            Character.ChangeDropdownOptions(index2);
        }

        //Activate and deactivate dropdown options based on requirements
        protected void ActivateRelatedGeosetOptions(List<int> activeGeosets, string name, string name2, Dictionary<int, int[]> reqRelations)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int[] requirements = reqRelations.Where(x => x.Value.Contains(Character.Options[index].Choices[Character.Customization[index]].ID)).Select(x => x.Key).ToArray();
            if (!requirements.Contains(Character.Options[index2].Choices[Character.Customization[index2]].Requirement))
            {
                var choice = Character.Options[index2].Choices.First(c => requirements.Contains(c.Requirement));
                Character.CustomizationDropdowns[index2].SetValue(Array.IndexOf(Character.Options[index2].Choices, choice));
            }
            Character.ActivateUsingRequirmenets(index2, requirements);
            ChangeGeosetOption(activeGeosets, name);
        }

        //Make sure dropdown only contains options that match given requirements
        protected void ChangeRelatedGeosetOptions(List<int> activeGeosets, string name, string name2, Dictionary<int, int[]> reqRelations)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            int[] requirements = reqRelations.Where(x => x.Value.Contains(Character.Options[index].Choices[Character.Customization[index]].ID)).Select(x => x.Key).ToArray();
            Character.Options[index2].SetChoices(Character.Options[index2].AllChoices.Where(c => requirements.Contains(c.Requirement)).ToArray());
            Character.ChangeDropdownOptions(index2);
            ChangeGeosetOption(activeGeosets, name);
        }

        //Change visible geosets for given option
        protected void ChangeGeosetOption(List<int> activeGeosets, string name)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            var geosets = Character.Options[index].Choices[Character.Customization[index]].Geosets.Where(g => g.Related == 0);
            foreach (var geoset in geosets)
            {
                activeGeosets.RemoveAll(x => x > Math.Clamp(geoset.Type * 100 - 1, 0, 9999) && x < ((geoset.Type + 1) * 100));
                activeGeosets.Add(geoset.Type * 100 + geoset.ID);
            }
        }

        //Change visible geosets for given option
        protected void ChangeGeosetOption(List<int> activeGeosets, string name, string name2)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            int index2 = Array.FindIndex(Character.Options, o => o.Name == name2 && o.Model == Character.ModelID);
            var geosets = Character.Options[index].Choices[Character.Customization[index]].Geosets.
                Where(g => g.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID);
            foreach (var geoset in geosets)
            {
                activeGeosets.RemoveAll(x => x > Math.Clamp(geoset.Type * 100 - 1, 0, 9999) && x < ((geoset.Type + 1) * 100));
                activeGeosets.Add(geoset.Type * 100 + geoset.ID);
            }
        }

        //Change visible geosets for given option on skinned model
        protected void ChangeSkinnedGeosetOption(List<int> activeGeosets, string name)
        {
            int index = Array.FindIndex(Character.Options, o => o.Name == name && o.Model == Character.ModelID);
            var geosets = Character.Options[index].Choices[Character.Customization[index]].SkinnedGeosets.Where(g => g.Related == 0);
            foreach (var geoset in geosets)
            {
                activeGeosets.Add(geoset.Type * 100 + geoset.ID);
            }
        }

        //Hide options available to other form
        protected void HideOtherFormsOptions()
        {
            for (int i = 0; i < Character.Options.Length; i++)
            {
                bool active = Character.Options[i].Model == Character.ModelID && Character.Options[i].Category == Character.Category && Character.Options[i].Choices.Length > 1;
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

        //Swap model id for current form, needs to be overriden for specific race
        public virtual void ChangeForm()
        {
            Character.ModelID = Character.ModelID;
        }

        //Load textures and store them to be used while rendering
        public void LoadTextures(Texture2D[] textures)
        {
            bool skin, eye;
            for (int i = 0; i < textures.Length; i++)
            {
                int file = LoadTexture(Model.Textures[i], i, out skin, out eye);
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
                    if (skin)
                    {
                        LayeredTexture(textures[i]);
                    }
                    else if (eye)
                    {
                        EyeTexture(textures[i]);
                    }
                    textures[i].Apply();
                }
            }
        }

        //Generate eye texture
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

        //Load textures based on their types
        protected int LoadTexture(M2Texture texture, int i, out bool skin, out bool eye)
        {
            int? file = null;
            int index, index2;
            skin = false;
            eye = false;
            switch (texture.Type)
            {
                case 0:
                    file = Model.TextureIDs[i];
                    break;
                case 1:
                    index = GetSkinColorIndex();
                    file = Character.Options[index].Choices[Character.Customization[index]].Textures.First(t => t.Target == 1).ID;
                    skin = true;
                    break;
                //           case 2:
                //               file = Character.Items[2] != null ? Character.Items[2].LeftTexture2 : -1;
                //               break;
                case 6:
                    index = GetHairColorIndex();
                    index2 = GetHairColor2Index();
                    file = index2 == -1 ? Character.Options[index].Choices[Character.Customization[index]].Textures[0].ID : 
                        Character.Options[index].Choices[Character.Customization[index]].Textures.
                        FirstOrDefault(t => t.Related == Character.Options[index2].Choices[Character.Customization[index2]].ID)?.ID;
                    break;
                case 7:
                    index = GetHairExtraIndex();
                    file = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == 11)?.ID;
                    break;
                case 8:
                    index = GetSkinExtraIndex();
                    file = Character.Options[index].Choices[Character.Customization[index]].Textures.First(t => t.Target == 2).ID;
                    break;
                case 10:
                    index = GetAccessoriesIndex();
                    file = Character.Options[index].Choices[Character.Customization[index]].Textures[0].ID;
                    break;
                case 19:
                    index = Array.FindIndex(Character.Options, o => o.Name == "Eye Color" && o.Model == Character.ModelID);
                    file = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == 25)?.ID;
                    eye = true;
                    break;
                case 20:
                    index = GetJewelryColorIndex();
                    file = Character.Options[index].Choices[Character.Customization[index]].Textures.FirstOrDefault(t => t.Target == 38)?.ID;
                    break;
            }
            return file == null ? -1 : file.Value;
        }

        //Get id of Skin Color option, can be overriden for specific race if name is different
        protected virtual int GetSkinColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Skin Color" && o.Model == Character.ModelID);
        }

        //Get id of Hair Color option, can be overriden for specific race if name is different
        protected virtual int GetHairColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Hair Color" && o.Model == Character.ModelID);
        }

        //Get id of Second Hair Color option, can be overriden for specific race if name is different
        protected virtual int GetHairColor2Index()
        {
            return -1;
        }

        //Get id of Hair Color Extra option, can be overriden for specific race if name is different
        protected virtual int GetHairExtraIndex()
        {
            return 0;
        }

        //Get id of Skin Color Extra option, can be overriden for specific race if name is different
        protected virtual int GetSkinExtraIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Skin Color" && o.Model == Character.ModelID);
        }

        //Get id of Accesories option, can be overriden for specific race if name is different
        protected virtual int GetAccessoriesIndex()
        {
            return 0;
        }

        //Get id of Jewelry Color option, can be overriden for specific race if name is different
        protected virtual int GetJewelryColorIndex()
        {
            return Array.FindIndex(Character.Options, o => o.Name == "Jewelry Color" && o.Model == Character.ModelID);
        }

        //Change geosets according to chosen character customization
        public abstract void ChangeGeosets(List<int> activeGeosets);

        //Generate skin texture from many layers
        protected abstract void LayeredTexture(Texture2D texture);
    }
}

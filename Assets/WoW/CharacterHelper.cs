using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW
{
    //Abstact class for handling character customization for each specific race
    public abstract class CharacterHelper
    {
        //All the data from m2 file
        public M2 Model { get; protected set; }
        //Emission texture used for glowing tattoos
        public Texture2D Emission { get; protected set; }
        //Reference to a main class that handles character models
        public Character Character { get; protected set; }

        //Draw Texture on top of another
        public void DrawTexture(Texture2D texture, Texture2D layer, int i, int j, float cover = 1f)
        {
            for (int x = 0; x < layer.width; x++)
            {
                for (int y = 0; y < layer.height; y++)
                {
                    Color color = layer.GetPixel(x, y);
                    texture.SetPixel(x + i, y + j, Color.Lerp(texture.GetPixel(x + i, y + j), color, color.a * cover));
                }
            }
        }

        //Overlay textures together
        protected void OverlayTexture(Texture2D texture, Texture2D layer, int i, int j)
        {
            for (int x = 0; x < layer.width; x++)
            {
                for (int y = 0; y < layer.height; y++)
                {
                    Color color = texture.GetPixel(x + i, y + j);
                    Color color2 = layer.GetPixel(x, y);
                    float r = color.r * 255f;
                    float g = color.g * 255f;
                    float b = color.b * 255f;
                    float r2 = color2.r * 255f;
                    float g2 = color2.g * 255f;
                    float b2 = color2.b * 255f;
                    r = (r / 255f) * (r + ((2 * r2) / 255f) * (255f - r));
                    g = (g / 255f) * (g + ((2 * g2) / 255f) * (255f - g));
                    b = (b / 255f) * (b + ((2 * b2) / 255f) * (255f - b));
                    Color result = new Color(r / 255f, g / 255f, b / 255f);
                    texture.SetPixel(x + i, y + j, result);
                }
            }
        }

        //Multiply textures
        protected void MultiplyTexture(Texture2D texture, Texture2D layer, int i, int j)
        {
            for (int x = 0; x < layer.width; x++)
            {
                for (int y = 0; y < layer.height; y++)
                {
                    Color color = layer.GetPixel(x, y);
                    texture.SetPixel(x + i, y + j, texture.GetPixel(x + i, y + j) * color);
                }
            }
        }

        //Generate black texture
        public void BlackTexture(Texture2D src, Texture2D dst)
        {
            for (int x = 0; x < src.width; x++)
            {
                for (int y = 0; y < src.height; y++)
                {
                    Color color = src.GetPixel(x, y);
                    if (color.a > 0.5f)
                    {
                        dst.SetPixel(x, y, Color.black);
                    }
                    else
                    {
                        color = Color.black;
                        color.a = 0f;
                        dst.SetPixel(x, y, color);
                    }
                }
            }
        }

        //Load textures and store them to be used while rendering
        public void LoadTextures(Texture2D[] textures)
        {
            bool skin;
            for (int i = 0; i < textures.Length; i++)
            {
                int file = LoadTexture(Model.Textures[i], i, out skin);
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
                    textures[i].Apply();
                }
            }
        }

        //Change geosets according to chosen character customization
        public abstract void ChangeGeosets(List<int> activeGeosets);

        //Generate skin texture from many layers
        protected abstract void LayeredTexture(Texture2D texture);

        //Load texture from casc
        protected abstract int LoadTexture(M2Texture texture, int i , out bool skin);
    }
}

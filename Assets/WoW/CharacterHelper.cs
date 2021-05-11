using BLPLib;
using CASCLib;
using M2Lib;
using System.Collections.Generic;
using UnityEngine;

namespace WoW
{
    public abstract class CharacterHelper
    {
        public M2 Model { get; protected set; }
        public Texture2D Emission { get; protected set; }
        public Character Character { get; protected set; }

        public string ModelsPath { get; set; }
        public string RacePath { get; set; }

        protected CASCHandler casc;
        protected System.Drawing.ImageConverter converter;

        protected void DrawTexture(Texture2D texture, Texture2D layer, int i, int j, float cover = 1f)
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

        protected Texture2D TextureFromBLP(int file)
        {
            BLP blp = new BLP(casc.OpenFile(file));
            System.Drawing.Bitmap image = blp.GetImage();
            Texture2D texture = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, true);
            texture.LoadImage((byte[])converter.ConvertTo(image, typeof(byte[])));
            texture.alphaIsTransparency = true;
            return texture;
        }

        public abstract void ChangeGeosets(List<int> activeGeosets);

        protected abstract void LayeredTexture(Texture2D texture);

        protected abstract int LoadTexture(M2Texture texture, int i , out bool skin);

        public abstract void LoadTextures(Texture2D[] textures);
    }
}

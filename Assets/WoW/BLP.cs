using System;
using System.IO;
using UnityEngine;

namespace Assets.WoW
{
    // Class to Load BLP file into Texture2D
    public class BLP
    {
        // Type of compression used in a file
        private byte compression;
        // Amount of bits used for alpha channel
        private byte alpha;
        // Format of coding alpha channel
        private byte format;
        // Does it have mipmaps
        private byte mips;
        // Width of the picture
        private int width;
        // Height of the picture
        private int height;
        // Offsets for mipmaps in the file
        private uint[] offsets;
        // Lengths of mipmaps in the file
        private uint[] lengths;
        // Color pallet
        private Color32[] pallete;
        // Picture pixels
        private Color32[] pixels;

        // Load BLP from file
        public BLP(string filename)
        {
            using BinaryReader reader = new(File.Open(filename, FileMode.Open));
            LoadFile(reader);
        }

        // Load BLP from stream
        public BLP(Stream stream)
        {
            using BinaryReader reader = new(stream);
            LoadFile(reader);
        }

        // Load file
        void LoadFile(BinaryReader reader)
        {
            int i;
            byte r, g, b, a;
            reader.ReadInt32();
            reader.ReadInt32();
            compression = reader.ReadByte();
            alpha = reader.ReadByte();
            format = reader.ReadByte();
            mips = reader.ReadByte();
            width = reader.ReadInt32();
            height = reader.ReadInt32();
            offsets = new uint[16];
            for (i = 0; i < 16; i++)
            {
                offsets[i] = reader.ReadUInt32();
            }
            lengths = new uint[16];
            for (i = 0; i < 16; i++)
            {
                lengths[i] = reader.ReadUInt32();
            }
            pallete = new Color32[256];
            for (i = 0; i < 256; i++)
            {
                b = reader.ReadByte();
                g = reader.ReadByte();
                r = reader.ReadByte();
                a = reader.ReadByte();
                pallete[i] = new Color32(r, g, b, a);
            }
            pixels = new Color32[width * height];
            if (compression == 1)
            {
                ReadBLP(reader);
            }
            else if (compression == 2)
            {
                if (format == 0)
                {
                    ReadDXT1(reader);
                }
                else if (format == 1)
                {
                    ReadDXT3(reader);
                }
                else if (format == 7)
                {
                    ReadDXT5(reader);
                }
            }
        }

        // / Load pixel data for BLP compression
        void ReadBLP(BinaryReader reader)
        {
            byte index, a, value;
            int i, j, k;
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    index = reader.ReadByte();
                    pixels[(height - 1 - i) * width + j] = pallete[index];
                    pixels[(height - 1 - i) * width + j].a = 255;
                }
            }
            if (alpha == 1)
            {
                for (i = 0; i < height; i++)
                {
                    for (j = 0; j < width;)
                    {
                        a = reader.ReadByte();
                        for (k = 0; k < 8; k++, j++)
                        {
                            value = (byte)(((a >> k) & 1) * 255);
                            pixels[(height - 1 - i) * width + j].a = value;
                        }
                    }
                }
            }
            else if (alpha == 4)
            {
                for (i = 0; i < height; i++)
                {
                    for (j = 0; j < width;)
                    {
                        a = reader.ReadByte();
                        for (k = 0; k < 2; k++, j++)
                        {
                            value = (byte)(((a >> (4 * k)) & 15) * 17);
                            pixels[(height - 1 - i) * width + j].a = value;
                        }
                    }
                }
            }
            else if (alpha == 8)
            {
                for (i = 0; i < height; i++)
                {
                    for (j = 0; j < width; j++)
                    {
                        a = reader.ReadByte();
                        pixels[(height - 1 - i) * width + j].a = a;
                    }
                }
            }
        }

        // / Load pixel data for DXT1 compression.
        void ReadDXT1(BinaryReader reader)
        {
            ushort color0;
            ushort color1;
            int indices, i, j, k, l;
            Color32[] colors = new Color32[4];
            int index;
            for (i = 0; i < height; i += 4)
            {
                for (j = 0; j < width; j += 4)
                {
                    color0 = reader.ReadUInt16();
                    color1 = reader.ReadUInt16();
                    colors[0] = new Color32((byte)(((color0 >> 11) & 31) * 255 / 31),(byte)(((color0 >> 5) & 63) * 255 / 63),
                        (byte)((color0 & 31) * 255 / 31), 255);
                    colors[1] = new Color32((byte)(((color1 >> 11) & 31) * 255 / 31), (byte)(((color1 >> 5) & 63) * 255 / 63),
                        (byte)((color1 & 31) * 255 / 31), 255);
                    if (color0 > color1)
                    {
                        colors[2] = new Color32((byte)((2 * colors[0].r + colors[1].r) / 3), (byte)((2 * colors[0].g + colors[1].g) / 3),
                            (byte)((2 * colors[0].b + colors[1].b) / 3), 255);
                        colors[3] = new Color32((byte)((colors[0].r + 2 * colors[1].r) / 3), (byte)((colors[0].g + 2 * colors[1].g) / 3),
                            (byte)((colors[0].b + 2 * colors[1].b) / 3), 255);
                    }
                    else
                    {
                        colors[2] = new Color32((byte)((colors[0].r + colors[1].r) / 2), (byte)((colors[0].g + colors[1].g) / 2),
                            (byte)((colors[0].b + colors[1].b) / 2), 255);
                        colors[3] = new Color32(0, 0, 0, 0);
                    }
                    indices = reader.ReadInt32();
                    for (k = 0; k < 4; k++)
                    {
                        for (l = 0; l < 4; l++)
                        {
                            index = (indices >> (8 * k + 2 * l)) & 3;
                            pixels[(height - 1 - i - k) * width + j + l] = colors[index];
                        }
                    }
                }
            }
        }

        // / <summary>
        // / Load pixel data for DXT3 compression.
        // / </summary>
        // / <param name="reader">The <see cref="System.IO.BinaryReader"/> object that currently has access to the file.</param>
        void ReadDXT3(BinaryReader reader)
        {
            ushort color0;
            ushort color1;
            int indices, i, j, k, l;
            Color32[] colors = new Color32[4];
            int index;
            byte value;
            long a;
            for (i = 0; i < height; i += 4)
            {
                for (j = 0; j < width; j += 4)
                {
                    a = reader.ReadInt64();
                    for (k = 0; k < 4; k++)
                    {
                        for (l = 0; l < 4; l++)
                        {
                            value = (byte)((int)((a >> (16 * k + 4 * l)) & 15) * 17);
                            pixels[(height - 1 - i - k) * width + j + l] = new Color32(0, 0, 0, value);
                        }
                    }
                    color0 = reader.ReadUInt16();
                    color1 = reader.ReadUInt16();
                    colors[0] = new Color32((byte)(((color0 >> 11) & 31) * 255 / 31), (byte)(((color0 >> 5) & 63) * 255 / 63),
                        (byte)((color0 & 31) * 255 / 31), 255);
                    colors[1] = new Color32((byte)(((color1 >> 11) & 31) * 255 / 31), (byte)(((color1 >> 5) & 63) * 255 / 63),
                        (byte)((color1 & 31) * 255 / 31), 255);
                    colors[2] = new Color32((byte)((2 * colors[0].r + colors[1].r) / 3), (byte)((2 * colors[0].g + colors[1].g) / 3),
                        (byte)((2 * colors[0].b + colors[1].b) / 3), 255);
                    colors[3] = new Color32((byte)((colors[0].r + 2 * colors[1].r) / 3), (byte)((colors[0].g + 2 * colors[1].g) / 3),
                        (byte)((colors[0].b + 2 * colors[1].b) / 3), 255);
                    indices = reader.ReadInt32();
                    for (k = 0; k < 4; k++)
                    {
                        for (l = 0; l < 4; l++)
                        {
                            index = (indices >> (8 * k + 2 * l)) & 3;
                            pixels[(height - 1 - i - k) * width + j + l].r = colors[index].r;
                            pixels[(height - 1 - i - k) * width + j + l].g = colors[index].g;
                            pixels[(height - 1 - i - k) * width + j + l].b = colors[index].b;
                        }
                    }
                }
            }
        }

        // Load pixel data for DXT5 compression
        void ReadDXT5(BinaryReader reader)
        {
            ushort color0;
            ushort color1;
            int indices, i, j, k, l;
            Color32[] colors = new Color32[4];
            int index;
            byte[] alpha = new byte[8];
            byte[] data = new byte[8];
            int[,] lookup = new int[4, 4];
            long bits;
            for (i = 0; i < height; i += 4)
            {
                for (j = 0; j < width; j += 4)
                {
                    alpha[0] = reader.ReadByte();
                    alpha[1] = reader.ReadByte();
                    if (alpha[0] > alpha[1])
                    {
                        for (k = 0; k < 6; k++)
                        {
                            alpha[k + 2] = (byte)(((6 - k) * alpha[0] + (1 + k) * alpha[1]) / 7);
                        }
                    }
                    else
                    {
                        for (k = 0; k < 4; k++)
                        {
                            alpha[k + 2] = (byte)(((4 - k) * alpha[0] + (1 + k) * alpha[1]) / 5);
                        }
                        alpha[6] = 0;
                        alpha[7] = 255;
                    }
                    for (k = 0; k < 6; k++)
                    {
                        data[k] = reader.ReadByte();
                    }
                    bits = BitConverter.ToInt64(data, 0);
                    for (k = 0; k < 4; k++)
                    {
                        for (l = 0; l < 4; l++)
                        {
                            lookup[k, l] = (int)(bits >> (12 * k + 3 * l)) & 7;
                        }
                    }
                    color0 = reader.ReadUInt16();
                    color1 = reader.ReadUInt16();
                    colors[0] = new Color32((byte)(((color0 >> 11) & 31) * 255 / 31), (byte)(((color0 >> 5) & 63) * 255 / 63),
                        (byte)((color0 & 31) * 255 / 31), 255);
                    colors[1] = new Color32((byte)(((color1 >> 11) & 31) * 255 / 31), (byte)(((color1 >> 5) & 63) * 255 / 63),
                        (byte)((color1 & 31) * 255 / 31), 255);
                    colors[2] = new Color32((byte)((2 * colors[0].r + colors[1].r) / 3), (byte)((2 * colors[0].g + colors[1].g) / 3),
                        (byte)((2 * colors[0].b + colors[1].b) / 3), 255);
                    colors[3] = new Color32((byte)((colors[0].r + 2 * colors[1].r) / 3), (byte)((colors[0].g + 2 * colors[1].g) / 3),
                        (byte)((colors[0].b + 2 * colors[1].b) / 3), 255);
                    indices = reader.ReadInt32();
                    for (k = 0; k < 4; k++)
                    {
                        for (l = 0; l < 4; l++)
                        {
                            index = (indices >> (8 * k + 2 * l)) & 3;
                            pixels[(height - 1 - i - k) * width + j + l] = new Color32(colors[index].r, colors[index].g, colors[index].b, alpha[lookup[k, l]]);
                        }
                    }
                }
            }
        }

        // Get the picture
        public Texture2D GetImage()
        {
            Texture2D texture = new(width, height, TextureFormat.ARGB32, false);
            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }
    }
}

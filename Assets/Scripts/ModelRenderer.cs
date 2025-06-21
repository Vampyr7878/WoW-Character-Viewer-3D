using Assets.WoW;
using CASCLib;
using M2Lib;
using SkelLib;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class ModelRenderer : MonoBehaviour
{
    // refernce to invisible material to hide some geosets
    public Material hiddenMaterial;

    // Loaded textures
#if UNITY_EDITOR
    public Texture2D[] textures;
#else
    protected Texture2D[] textures;
#endif
#if UNITY_EDITOR
    // Listfile dictionary to speed up debugging
    protected Dictionary<int, string> listFile;
    // Path to locally unpacked game files
    protected string dataPath;
#else
    // Referenece to the opened CASC storage
    protected CASCHandler casc;
#endif
    // Image converter for loading textures
    protected System.Drawing.ImageConverter converter;
    // Reference to the instantiated prefab
    protected GameObject mesh;
    // Colors used by the model
    protected Color[] colors;
    // Current time for color animations
    protected float[] colorTime;
    // Current texture color frame
    protected int[] colorFrame;
    // Current time for transparency animations
    protected float[] transparencyTime;
    // Current texture transparency frame
    protected int[] transparencyFrame;
    // Path from where to laod the models
    protected string modelsPath;
    // Current time for texture animations
    protected float[] textureTime;
    // Current texture animation frame
    protected int[] textureFrame;
    // Thread to load binary data
    protected Thread loadBinaries;
    // Renderer object for this model
    protected new SkinnedMeshRenderer renderer;
    // Animator object for this model
    protected Animator animator;
    // List of geosets that are enabled for loading
    protected List<int> activeGeosets;

    // Reference to the binary data
    public M2 Model { get; protected set; }
    // Indicate if the all the data has been loaded
    public bool Loaded { get; protected set; }
    // Trigger changes to rendered model
    public bool Change { get; set; }

    // Animate current texture unit
    protected void AnimateTextures(SkinnedMeshRenderer renderer, int i)
    {
        if (renderer.materials[i].shader != hiddenMaterial.shader)
        {
            int index = Model.TextureAnimationsLookup[Model.Skin.Textures[i].TextureAnimation];
            string texture = "_Texture1";
            if (index >= 0)
            {
                Vector2 offset = renderer.materials[i].GetTextureOffset(texture);
                offset = AnimateTexture(index, offset);
                renderer.materials[i].SetTextureOffset(texture, offset);
            }
            if (Model.Skin.Textures[i].TextureCount > 1)
            {
                index = Model.TextureAnimationsLookup[Model.Skin.Textures[i].TextureAnimation + 1];
                texture = "_Texture2";
                if (index >= 0)
                {
                    Vector2 offset = renderer.materials[i].GetTextureOffset(texture);
                    offset = AnimateTexture(index, offset);
                    renderer.materials[i].SetTextureOffset(texture, offset);
                }
            }
        }
    }

    // Animate current texture
    protected Vector2 AnimateTexture(int index, Vector2 offset)
    {
        if (index < Model.TextureAnimations.Length)
        {
            TextureAnimation animation = Model.TextureAnimations[index];
            if (animation.Translation.Timestamps.Length > 0 && animation.Translation.Timestamps[0].Length > 1)
            {
                if (textureTime[index] >= animation.Translation.Timestamps[0][textureFrame[index]] / 1000f)
                {
                    textureFrame[index]++;
                    if (textureFrame[index] == animation.Translation.Timestamps[0].Length)
                    {
                        textureFrame[index] = 0;
                        textureTime[index] = 0f;
                    }
                }
                if (textureFrame[index] == 0)
                {
                    offset.x = animation.Translation.Values[0][textureFrame[index]].X;
                    offset.y = animation.Translation.Values[0][textureFrame[index]].Y;
                }
                else
                {
                    float textureTimestamp = (animation.Translation.Timestamps[0][textureFrame[index]] - animation.Translation.Timestamps[0][textureFrame[index] - 1]) / 1000f;
                    textureTimestamp = (textureTime[index] - animation.Translation.Timestamps[0][textureFrame[index] - 1] / 1000f) / textureTimestamp;
                    offset.x = Mathf.Lerp(animation.Translation.Values[0][textureFrame[index] - 1].X, animation.Translation.Values[0][textureFrame[index]].X, textureTimestamp);
                    offset.y = -Mathf.Lerp(animation.Translation.Values[0][textureFrame[index] - 1].Y, animation.Translation.Values[0][textureFrame[index]].Y, textureTimestamp);
                }
                offset.x = offset.x > 1 ? offset.x - 1 : offset.x;
                offset.x = offset.x < -1 ? offset.x + 1 : offset.x;
                offset.y = offset.y > 1 ? offset.y - 1 : offset.y;
                offset.y = offset.y < -1 ? offset.y + 1 : offset.y;
            }
        }
        return offset;
    }

    // Animate current texture unit
    protected void AnimateColors(SkinnedMeshRenderer renderer, int i)
    {
        if (renderer.materials[i].shader != hiddenMaterial.shader)
        {
            int index = Model.Skin.Textures[i].Color;
            if (index >= 0)
            {
                if (Model.Colors[index].Color.Values[0].Length > 1)
                {
                    Color color = renderer.materials[i].GetColor("_Color");
                    color = AnimateColor(index, color);
                    renderer.materials[i].SetColor("_Color", color);
                }
                if (Model.Colors[index].Transparency.Values[0].Length > 1)
                {
                    Color color = renderer.materials[i].GetColor("_Color");
                    color = AnimateTransparency(index, color);
                    renderer.materials[i].SetColor("_Color", color);
                }
            }
        }
    }

    // Animate current colors
    protected Color AnimateColor(int index, Color color)
    {
        if (index < Model.TextureAnimations.Length)
        {
            Track<Vector3D> animation = Model.Colors[index].Color;
            if (animation.Timestamps.Length > 0 && animation.Timestamps[0].Length > 1)
            {
                if (colorTime[index] >= animation.Timestamps[0][colorFrame[index]] / 1000f)
                {
                    colorFrame[index]++;
                    if (colorFrame[index] == animation.Timestamps[0].Length)
                    {
                        colorFrame[index] = 0;
                        colorTime[index] = 0f;
                    }
                }
                if (colorFrame[index] == 0)
                {
                    color.r = animation.Values[0][colorFrame[index]].X;
                    color.g = animation.Values[0][colorFrame[index]].Y;
                    color.b = animation.Values[0][colorFrame[index]].Z;
                }
                else
                {
                    float colorTimestamp = (animation.Timestamps[0][colorFrame[index]] - animation.Timestamps[0][colorFrame[index] - 1]) / 1000f;
                    colorTimestamp = (colorTime[index] - animation.Timestamps[0][colorFrame[index] - 1] / 1000f) / colorTimestamp;
                    color.r = Mathf.Lerp(animation.Values[0][colorFrame[index] - 1].X, animation.Values[0][colorFrame[index]].X, colorTimestamp);
                    color.g = Mathf.Lerp(animation.Values[0][colorFrame[index] - 1].Y, animation.Values[0][colorFrame[index]].Y, colorTimestamp);
                    color.b = Mathf.Lerp(animation.Values[0][colorFrame[index] - 1].Z, animation.Values[0][colorFrame[index]].Z, colorTimestamp);
                }
            }
        }
        return color;
    }

    // Animate current transparency
    protected Color AnimateTransparency(int index, Color color)
    {
        if (index < Model.TextureAnimations.Length)
        {
            Track<float> animation = Model.Colors[index].Transparency;
            if (animation.Timestamps.Length > 0 && animation.Timestamps[0].Length > 1)
            {
                if (transparencyTime[index] >= animation.Timestamps[0][transparencyFrame[index]] / 1000f)
                {
                    transparencyFrame[index]++;
                    if (transparencyFrame[index] == animation.Timestamps[0].Length)
                    {
                        transparencyFrame[index] = 0;
                        transparencyTime[index] = 0f;
                    }
                }
                if (transparencyFrame[index] == 0)
                {
                    color.a = animation.Values[0][transparencyFrame[index]];
                }
                else
                {
                    float transparencyTimestamp = (animation.Timestamps[0][transparencyFrame[index]] - animation.Timestamps[0][transparencyFrame[index] - 1]) / 1000f;
                    transparencyTimestamp = (transparencyTime[index] - animation.Timestamps[0][transparencyFrame[index] - 1] / 1000f) / transparencyTimestamp;
                    color.a = Mathf.Lerp(animation.Values[0][transparencyFrame[index] - 1], animation.Values[0][transparencyFrame[index]], transparencyTimestamp);
                }
            }
        }
        return color;
    }

    // Get proper source color blend option for alpha blending
    protected BlendMode SrcColorBlend(short value)
    {
        BlendMode blend = BlendMode.One;
        switch (value)
        {
            case 0:
            case 1:
            case 3:
                blend = BlendMode.One;
                break;
            case 2:
            case 4:
                blend = BlendMode.SrcAlpha;
                break;
            case 5:
                blend = BlendMode.OneMinusSrcAlpha;
                break;
            case 6:
                blend = BlendMode.DstColor;
                break;
            case 7:
                blend = BlendMode.SrcColor;
                break;
        }
        return blend;
    }

    // Get proper destination color blend option for alpha blending
    protected BlendMode DstColorBlend(short value)
    {
        BlendMode blend = BlendMode.Zero;
        switch (value)
        {
            case 0:
            case 1:
                blend = BlendMode.Zero;
                break;
            case 2:
            case 7:
                blend = BlendMode.OneMinusSrcAlpha;
                break;
            case 3:
            case 4:
                blend = BlendMode.One;
                break;
            case 5:
            case 6:
                blend = BlendMode.SrcColor;
                break;
        }
        return blend;
    }

    // Get proper source alpha blend option for alpha blending
    protected BlendMode SrcAlphaBlend(short value)
    {
        BlendMode blend = BlendMode.One;
        switch (value)
        {
            case 0:
            case 1:
            case 2:
                blend = BlendMode.One;
                break;
            case 3:
            case 4:
                blend = BlendMode.Zero;
                break;
            case 5:
            case 7:
                blend = BlendMode.SrcAlpha;
                break;
            case 6:
                blend = BlendMode.DstAlpha;
                break;
        }
        return blend;
    }

    // Get proper destination alpha blend option for alpha blending
    protected BlendMode DstAlphaBlend(short value)
    {
        BlendMode blend = BlendMode.Zero;
        switch (value)
        {
            case 0:
            case 1:
            case 5:
                blend = BlendMode.Zero;
                break;
            case 2:
            case 7:
                blend = BlendMode.OneMinusSrcAlpha;
                break;
            case 3:
            case 4:
                blend = BlendMode.One;
                break;
            case 6:
                blend = BlendMode.SrcAlpha;
                break;
        }
        return blend;
    }

    // Select material for particle
    public Material ParticleMaterial(int blend)
    {
        string material = blend switch
        {
            2 => "particlefademultiplymaterial",
            4 => "particleadditivemultiplymaterial",
            //7 => "particleaddtivecolormaterial",
            _ => "particletransparentcolormaterial",
        };
        if (material == "")
        {
            return null;
        }
        return Resources.Load<Material>(@$"Materials\{material}");
    }

    // Load model colors
    protected void LoadColors()
    {
        colors = new Color[Model.Colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(Model.Colors[i].Color.Values[0][0].X, Model.Colors[i].Color.Values[0][0].Y, Model.Colors[i].Color.Values[0][0].Z,
                Model.Colors[i].Transparency.Values[0].Length > 0 ? Model.Colors[i].Transparency.Values[0][0] : 1f);
        }
    }

    // Create texture from BLP file
    public Texture2D TextureFromBLP(int file)
    {
#if UNITY_EDITOR
        BLP blp = new($@"{dataPath}\{listFile[file]}");
#else
        BLP blp = new(casc.OpenFile(file));
#endif
        Texture2D texture = blp.GetImage();
        return texture;
    }

    // Set material with proper shader
    protected abstract void SetMaterial(SkinnedMeshRenderer renderer, int i);

    // Setup all the material properties
    protected abstract void SetTexture(Material material, int i);
}

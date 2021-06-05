using BLPLib;
using CASCLib;
using M2Lib;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class ModelRenderer : MonoBehaviour
{
    public Material hiddenMaterial;

    //Reference to loaded casc data
    protected CASCHandler casc;
    //Image converter for loading textures
    protected System.Drawing.ImageConverter converter;
    //Reference to the instantiated prefab
    protected GameObject mesh;
    //Colors used by the model
    protected Color[] colors;
    //Path from where to laod the models
    protected string modelsPath;
    //Loaded textures
    protected Texture2D[] textures;
    //Current time for texture animations
    protected float[] time;
    //Current texture animation frame
    protected int[] frame;
    //Thread to load binary data
    protected Thread loadBinaries;
    //Renderer object for this model
    protected new SkinnedMeshRenderer renderer;
    //Animator object for this model
    protected Animator animator;

    //Reference to the binary data
    public M2 Model { get; protected set; }
    //Indicate if the all the data has been loaded
    public bool Loaded { get; protected set; }
    //Trigger changes to rendered model
    public bool Change { get; set; }

    //Animate current texture unit
    protected void AnimateTextures(SkinnedMeshRenderer renderer, int i)
    {
        if (renderer.materials[Model.Skin.Textures[i].Id].shader != hiddenMaterial.shader)
        {
            int index = Model.TextureAnimationsLookup[Model.Skin.Textures[i].TextureAnimation];
            string texture = "_Texture1";
            if (index >= 0)
            {
                Vector2 offset = renderer.materials[Model.Skin.Textures[i].Id].GetTextureOffset(texture);
                offset = AnimateTexture(index, offset);
                renderer.materials[Model.Skin.Textures[i].Id].SetTextureOffset(texture, offset);
            }
            index = Model.TextureAnimationsLookup[Model.Skin.Textures[i].TextureAnimation + 1];
            texture = "_Texture2";
            if (index >= 0)
            {
                Vector2 offset = renderer.materials[Model.Skin.Textures[i].Id].GetTextureOffset(texture);
                offset = AnimateTexture(index, offset);
                renderer.materials[Model.Skin.Textures[i].Id].SetTextureOffset(texture, offset);
            }
        }
    }

    //Animate current texture
    protected Vector2 AnimateTexture(int index, Vector2 offset)
    {
        if (index < Model.TextureAnimations.Length)
        {
            TextureAnimation animation = Model.TextureAnimations[index];
            if (animation.Translation.Timestamps.Length > 0 && animation.Translation.Timestamps[0].Length > 1)
            {
                if (time[index] >= animation.Translation.Timestamps[0][frame[index]] / 1000f)
                {
                    frame[index]++;
                    if (frame[index] == animation.Translation.Timestamps[0].Length)
                    {
                        frame[index] = 0;
                        time[index] = 0f;
                    }
                }
                if (frame[index] == 0)
                {
                    offset.x = animation.Translation.Values[0][frame[index]].X;
                    offset.y = animation.Translation.Values[0][frame[index]].Y;
                }
                else
                {
                    float timestamp = (animation.Translation.Timestamps[0][frame[index]] - animation.Translation.Timestamps[0][frame[index] - 1]) / 1000f;
                    timestamp = (time[index] - animation.Translation.Timestamps[0][frame[index] - 1] / 1000f) / timestamp;
                    offset.x = Mathf.Lerp(animation.Translation.Values[0][frame[index] - 1].X, animation.Translation.Values[0][frame[index]].X, timestamp);// * Time.deltaTime);
                    offset.y = Mathf.Lerp(animation.Translation.Values[0][frame[index] - 1].Y, animation.Translation.Values[0][frame[index]].Y, timestamp);// * Time.deltaTime);
                }
                offset.x = offset.x > 1 ? offset.x - 1 : offset.x;
                offset.x = offset.x < -1 ? offset.x + 1 : offset.x;
                offset.y = offset.y > 1 ? offset.y - 1 : offset.y;
                offset.y = offset.y < -1 ? offset.y + 1 : offset.y;
            }
        }
        return offset;
    }

    //Get proper source blend option for alpha blending
    protected BlendMode SrcBlend(short value)
    {
        BlendMode blend = BlendMode.One;
        switch (value)
        {
            case 0:
            case 1:
            case 7:
                blend = BlendMode.One;
                break;
            case 2:
            case 4:
                blend = BlendMode.SrcAlpha;
                break;
            case 3:
                blend = BlendMode.SrcColor;
                break;
            case 5:
            case 6:
                blend = BlendMode.DstColor;
                break;
        }
        return blend;
    }

    //Get proper destination blend option for alpha blending
    protected BlendMode DstBlend(short value)
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
                blend = BlendMode.SrcColor;
                break;
        }
        return blend;
    }

    public Material ParticleMaterial(int blend)
    {
        string material = "";
        switch(blend)
        {
            case 0:
                material = "particleopaquematerial";
                break;
            case 1:
                material = "particlecutoutmaterial";
                break;
            case 2:
                material = "particlefadematerial";
                break;
            case 3:
            case 4:
                material = "particleadditivematerial";
                break;
            case 5:
            case 6:
                material = "particlemodulatematerial";
                break;
            case 7:
                material = "particletransparentmaterial";
                break;
        }
        if (material == "")
        {
            return null;
        }
        return Resources.Load<Material>(@$"Materials\{material}");
    }

    //Load model colors
    protected void LoadColors()
    {
        colors = new Color[Model.Colors.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(Model.Colors[i].R / 255f, Model.Colors[i].G / 255f, Model.Colors[i].B / 255f, 1.0f);
        }
    }

    //Create texture from BLP file
    public Texture2D TextureFromBLP(int file)
    {
        BLP blp = new BLP(casc.OpenFile(file));
        System.Drawing.Bitmap image = blp.GetImage();
        Texture2D texture = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, true);
        texture.LoadImage((byte[])converter.ConvertTo(image, typeof(byte[])));
        return texture;
    }

    //Set material with proper shader
    protected abstract void SetMaterial(SkinnedMeshRenderer renderer, int i);

    //Setup all the material properties
    protected abstract void SetTexture(Material material, int i);
}

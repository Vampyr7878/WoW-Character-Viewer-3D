using UnityEngine;

namespace Crosstales.Common.Util
{
    /// <summary>Random color changer.</summary>
    //[HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_util_1_1_random_color.html")] //TODO update URL
    [RequireComponent(typeof(Renderer))]
    public class RandomColor : MonoBehaviour
    {
        #region Variables

        public bool UseInterval = true;
        public Vector2 ChangeInterval = new Vector2(5, 15);

        //public Vector2 ColorRange = new Vector2(0f, 360f);

        public Vector2 HueRange = new Vector2(0f, 1f);
        public Vector2 SaturationRange = new Vector2(1f, 1f);
        public Vector2 ValueRange = new Vector2(1f, 1f);
        public Vector2 AlphaRange = new Vector2(1f, 1f);

        public bool GrayScale = false;

        //[Range(0f, 1f)]
        //public float Saturation = 1f;

        //[Range(0f, 1f)]
        //public float Value = 1f;

        //[Range(0f, 1f)]
        //public float Opacity = 1f;

        public bool ChangeMaterial = false;
        public Material Material;

        public bool RandomColorAtStart = false;

        private float elapsedTime = 0f;
        private float changeTime = 0f;
        private Renderer currentRenderer;

        private Color32 startColor;
        private Color32 endColor;

        private float lerpProgress = 0f;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            elapsedTime = changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);

            if (RandomColorAtStart)
            {
                if (GrayScale)
                {
                    //int grayScale = (int)Random.Range(ColorRange.x, ColorRange.y);
                    //startColor = new Color32((byte)grayScale, (byte)grayScale, (byte)grayScale, (byte)1);

                    float grayScale = Random.Range(HueRange.x, HueRange.y);
                    startColor = new Color(grayScale, grayScale, grayScale, Random.Range(AlphaRange.x, AlphaRange.y));
                }
                else
                {
                    //startColor = BaseHelper.HSVToRGB(Random.Range(ColorRange.x, ColorRange.y), Saturation, Value, Opacity);
                    startColor = Random.ColorHSV(HueRange.x, HueRange.y, SaturationRange.x, SaturationRange.y, ValueRange.x, ValueRange.y, AlphaRange.x, AlphaRange.y);
                }

                if (ChangeMaterial)
                {
                    Material.SetColor("_Color", startColor);
                }
                else
                {
                    currentRenderer = GetComponent<Renderer>();
                    currentRenderer.material.color = startColor;
                }
            }
            else
            {
                if (ChangeMaterial)
                {
                    startColor = Material.GetColor("_Color");
                }
                else
                {
                    currentRenderer = GetComponent<Renderer>();
                    startColor = currentRenderer.material.color;
                }
            }
        }

        public void Update()
        {
            if (UseInterval)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime > changeTime)
                {
                    lerpProgress = elapsedTime = 0f;

                    if (GrayScale)
                    {
                        //int grayScale = (int)Random.Range(ColorRange.x, ColorRange.y);
                        //endColor = new Color32((byte)grayScale, (byte)grayScale, (byte)grayScale, (byte)1);

                        float grayScale = Random.Range(HueRange.x, HueRange.y);
                        endColor = new Color(grayScale, grayScale, grayScale, Random.Range(AlphaRange.x, AlphaRange.y));
                    }
                    else
                    {
                        //endColor = BaseHelper.HSVToRGB(Random.Range(ColorRange.x, ColorRange.y), Saturation, Value, Opacity);
                        endColor = Random.ColorHSV(HueRange.x, HueRange.y, SaturationRange.x, SaturationRange.y, ValueRange.x, ValueRange.y, AlphaRange.x, AlphaRange.y);
                    }

                    changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);
                }

                if (ChangeMaterial)
                {
                    Material.SetColor("_Color", Color.Lerp(startColor, endColor, lerpProgress));
                }
                else
                {
                    currentRenderer.material.color = Color.Lerp(startColor, endColor, lerpProgress);
                }

                if (lerpProgress < 1f)
                {
                    lerpProgress += Time.deltaTime / (changeTime - 0.1f);
                    //lerpProgress += Time.deltaTime / changeTime;
                }
                else
                {
                    if (ChangeMaterial)
                    {
                        startColor = Material.GetColor("_Color");
                    }
                    else
                    {
                        startColor = currentRenderer.material.color;
                    }
                }
            }
        }

        #endregion
    }
}
// © 2015-2018 crosstales LLC (https://www.crosstales.com)
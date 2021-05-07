using UnityEngine;

namespace Crosstales.Common.Util
{
    /// <summary>Random scale changer.</summary>
    //[HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_util_1_1_random_scaler.html")] //TODO update URL
    public class RandomScaler : MonoBehaviour
    {
        #region Variables

        public bool UseInterval = true;
        public Vector2 ChangeInterval = new Vector2(5, 15);
        public Vector3 ScaleMin = new Vector3(0.1f, 0.1f, 0.1f);
        public Vector3 ScaleMax = new Vector3(3, 3, 3);
        public bool Uniform = true;
        public bool RandomScaleAtStart = false;

        private Transform tf;
        private Vector3 startScale;
        private Vector3 endScale;
        private float elapsedTime = 0f;
        private float changeTime = 0f;
        private float lerpTime = 0f;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            tf = transform;

            elapsedTime = changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);

            if (RandomScaleAtStart)
            {
                if (Uniform)
                {
                    startScale.x = startScale.y = startScale.z = Random.Range(ScaleMin.x, Mathf.Abs(ScaleMax.x));
                }
                else
                {
                    startScale.x = Random.Range(ScaleMin.x, Mathf.Abs(ScaleMax.x));
                    startScale.y = Random.Range(ScaleMin.y, Mathf.Abs(ScaleMax.y));
                    startScale.z = Random.Range(ScaleMin.z, Mathf.Abs(ScaleMax.z));
                }

                tf.localScale = startScale;
            }
            else
            {
                startScale = tf.localScale;
            }
        }

        public void Update()
        {
            if (UseInterval)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime > changeTime)
                {
                    lerpTime = elapsedTime = 0f;

                    if (Uniform)
                    {
                        endScale.x = endScale.y = endScale.z = Random.Range(ScaleMin.x, Mathf.Abs(ScaleMax.x));
                    }
                    else
                    {
                        endScale.x = Random.Range(ScaleMin.x, Mathf.Abs(ScaleMax.x));
                        endScale.y = Random.Range(ScaleMin.y, Mathf.Abs(ScaleMax.y));
                        endScale.z = Random.Range(ScaleMin.z, Mathf.Abs(ScaleMax.z));
                    }

                    changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);
                }

                tf.localScale = Vector3.Lerp(startScale, endScale, lerpTime);

                if (lerpTime < 1f)
                {
                    lerpTime += Time.deltaTime / (changeTime - 0.1f);
                }
                else
                {
                    startScale = tf.localScale;
                }
            }
        }

        #endregion
    }
}
// © 2015-2018 crosstales LLC (https://www.crosstales.com)
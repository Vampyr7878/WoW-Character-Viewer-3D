using UnityEngine;

namespace Crosstales.Common.Util
{
    /// <summary>Random rotation changer.</summary>
    //[HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_util_1_1_random_rotator.html")] //TODO update URL
    public class RandomRotator : MonoBehaviour
    {
        #region Variables

        public bool UseInterval = true;
        public Vector2 ChangeInterval = new Vector2(10, 20);
        public Vector3 SpeedMin = new Vector3(5, 5, 5);
        public Vector3 SpeedMax = new Vector3(15, 15, 15);
        public bool RandomRotationAtStart = false;

        private Transform tf;
        private Vector3 speed;
        private float elapsedTime = 0f;
        private float changeTime = 0f;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            tf = transform;

            elapsedTime = changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);

            if (RandomRotationAtStart)
            {
                tf.localRotation = Random.rotation;
            }
        }

        public void Update()
        {
            if (UseInterval)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime > changeTime)
                {
                    elapsedTime = 0f;

                    speed.x = Random.Range(Mathf.Abs(SpeedMin.x), Mathf.Abs(SpeedMax.x)) * (Random.Range(0, 2) == 0 ? 1 : -1);
                    speed.y = Random.Range(Mathf.Abs(SpeedMin.y), Mathf.Abs(SpeedMax.y)) * (Random.Range(0, 2) == 0 ? 1 : -1);
                    speed.z = Random.Range(Mathf.Abs(SpeedMin.z), Mathf.Abs(SpeedMax.z)) * (Random.Range(0, 2) == 0 ? 1 : -1);
                    changeTime = Random.Range(ChangeInterval.x, ChangeInterval.y);
                }

                tf.Rotate(speed.x * Time.deltaTime, speed.y * Time.deltaTime, speed.z * Time.deltaTime);
            }
        }

        #endregion
    }
}
// © 2015-2018 crosstales LLC (https://www.crosstales.com)
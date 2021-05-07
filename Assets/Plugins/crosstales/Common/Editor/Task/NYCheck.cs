using UnityEngine;
using UnityEditor;

namespace Crosstales.Common.EditorTask
{
    /// <summary>Checks if a 'Happy new year'-message must be displayed.</summary>
    [InitializeOnLoad]
    public static class NYCheck
    {
        private const string KEY_NYCHECK_DATE = "CT_CFG_NYCHECK_DATE";

        #region Constructor

        static NYCheck()
        {
            string lastYear = EditorPrefs.GetString(KEY_NYCHECK_DATE);

            string year = System.DateTime.Now.ToString("yyyy");
            string month = System.DateTime.Now.ToString("MM");

            if (!year.Equals(lastYear) && month.Equals("01"))
            {
                Debug.LogWarning("¸.•°*”˜˜”*°•.¸ ★  crosstales LLC wishes you a happy and successful " + year + "!  ★ ¸.•*¨`*•.♫❤♫❤♫❤");

                EditorPrefs.SetString(KEY_NYCHECK_DATE, year);
            }
        }

        #endregion

    }
}
// © 2017-2018 crosstales LLC (https://www.crosstales.com)
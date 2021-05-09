using UnityEngine;

//Class to store particle color date from database
public class ParticleColor
{
    //Starting color
    public Color Start { get; set; }
    //Middle color
    public Color Mid { get; set; }
    //End color
    public Color End { get; set; }

    //Constructor
    public ParticleColor(int start, int mid, int end)
    {
        Color color;
        ColorUtility.TryParseHtmlString("#" + start.ToString("X8").Substring(2), out color);
        Start = color;
        ColorUtility.TryParseHtmlString("#" + mid.ToString("X8").Substring(2), out color);
        Mid = color;
        ColorUtility.TryParseHtmlString("#" + end.ToString("X8").Substring(2), out color);
        End = color;
    }
}

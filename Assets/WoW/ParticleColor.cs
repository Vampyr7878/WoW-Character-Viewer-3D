using UnityEngine;

// Class to store particle color date from database
public class ParticleColor
{
    // Starting color
    public Color32 Start { get; set; }
    // Middle color
    public Color32 Mid { get; set; }
    // End color
    public Color32 End { get; set; }

    public ParticleColor(int start, int mid, int end)
    {
        Color color;
        ColorUtility.TryParseHtmlString($"#{start.ToString("X8")[2..]}", out color);
        Start = color;
        ColorUtility.TryParseHtmlString($"#{mid.ToString("X8")[2..]}", out color);
        Mid = color;
        ColorUtility.TryParseHtmlString($"#{end.ToString("X8")[2..]}", out color);
        End = color;
    }
}

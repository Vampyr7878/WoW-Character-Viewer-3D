using UnityEngine;

// Option Data for custom dropdown where you can disable certain options
public class CustomOptionData : UnityEngine.UI.Dropdown.OptionData
{
    // Property to enable/disable current option
    public bool Interactable { get; set; }

    // Property to hide current option
    public int Index { get; set; }

    // DB ID of the option
    public int ID { get; set; }

    // Property to store first color
    public Color32 Color1 { get; set; }

    // Property to store second color
    public Color32 Color2 { get; set; }

    public CustomOptionData(string text, Color32 color1, Color32 color2, Sprite image, int index, int id) : base(text, image)
    {
        Interactable = true;
        Index = index;
        Color1 = color1;
        Color2 = color2;
        ID = id;
    }
}

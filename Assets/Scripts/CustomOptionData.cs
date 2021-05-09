using UnityEngine;

//Option Data for custom dropdown where you can disable certain options
public class CustomOptionData : UnityEngine.UI.Dropdown.OptionData
{
    //Property to enable/disable current option
    public bool Interactable { get; set; }

    //Constructor
    public CustomOptionData(string text, Sprite image) : base(text, image)
    {
        Interactable = true;
    }
}

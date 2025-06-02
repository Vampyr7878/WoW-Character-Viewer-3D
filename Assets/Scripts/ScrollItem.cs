using Assets.WoW;
using UnityEngine;
using UnityEngine.UI;

// Hold scroll item chidren for easy access
public class ScrollItem : MonoBehaviour
{
    // Toggle of scroll item
    public Toggle toggle;
    // Text of scroll item
    public Text label;
    // Icon of scroll item
    public Image background;
    // Checkmark of scroll item
    public Image checkmark;
    // Checkmark outline of scroll item
    public Outline outline;
    // Tooltip of scroll item
    public Image tooltip;

    // Item info of scroll item
    public ItemInstance Item { get; set; }

    // Set contents of the tooltip
    public void SetTooltip(string name, Color32 nameColor, string description, Color32 descriptionColor, string slot, string type)
    {
        var itemTooltip = GetComponent<ItemTooltip>();
        itemTooltip.name.text = name;
        itemTooltip.name.color = nameColor;
        if (string.IsNullOrEmpty(description))
        {
            itemTooltip.description.gameObject.SetActive(false);
        }
        else
        {
            itemTooltip.description.gameObject.SetActive(true);
            itemTooltip.description.text = description;
            itemTooltip.description.color = descriptionColor;
        }
        itemTooltip.slot.text = slot;
        itemTooltip.type.text = type;
    }
}

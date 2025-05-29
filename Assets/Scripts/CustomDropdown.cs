using UnityEngine;
using UnityEngine.UI;

// Custom dropdown that allows you to disable some options
public class CustomDropdown : Dropdown
{
    // First image
    public Image captionImage1;
    // Second image
    public Image captionImage2;

    // counter to track down next index value
    private int index = 0;

    // Constructor
    protected override GameObject CreateDropdownList(GameObject template)
    {
        index = 0;
        return base.CreateDropdownList(template);
    }

    // Make disabled options half transparent
    protected override DropdownItem CreateItem(DropdownItem itemTemplate)
    {
        DropdownItem item = base.CreateItem(itemTemplate);
        Image[] images = item.GetComponentsInChildren<Image>();
        item.toggle.interactable = ((CustomOptionData)options[index]).Interactable;
        Color color = item.text.color;
        color.a = ((CustomOptionData)options[index]).Interactable ? 1f : 0.5f;
        item.text.color = color;
        if (((CustomOptionData)options[index]).Color1 == Color.black)
        {
            images[1].gameObject.SetActive(false);
            images[2].gameObject.SetActive(false);
            images[3].gameObject.SetActive(false);
        }
        else if (((CustomOptionData)options[index]).Color2 == Color.black)
        {
            images[1].color = ((CustomOptionData)options[index]).Color1;
            images[2].gameObject.SetActive(false);
            images[3].gameObject.SetActive(false);
        }
        else
        {
            images[1].gameObject.SetActive(false);
            images[2].color = ((CustomOptionData)options[index]).Color1;
            images[3].color = ((CustomOptionData)options[index]).Color2;
        }
        index++;
        return item;
    }

    // Remove Number from option that contains text on Awake
    protected override void Awake()
    {
        base.Awake();
        string text = captionText.text;
        text = text[(text.IndexOf(':') + 1)..];
        captionText.text = text;
        if (((CustomOptionData)options[value]).Color1 == Color.black)
        {
            captionImage.gameObject.SetActive(false);
            captionImage1.gameObject.SetActive(false);
            captionImage2.gameObject.SetActive(false);
        }
        else if (((CustomOptionData)options[value]).Color2 == Color.black)
        {
            captionImage.color = ((CustomOptionData)options[value]).Color1;
            captionImage1.gameObject.SetActive(false);
            captionImage2.gameObject.SetActive(false);
        }
        else
        {
            captionImage.gameObject.SetActive(false);
            captionImage1.color = ((CustomOptionData)options[value]).Color1;
            captionImage2.color = ((CustomOptionData)options[value]).Color2;
        }
    }

    // Remove Number from option that contains text on Start
    protected override void Start()
    {
        base.Start();
        string text = captionText.text;
        text = text[(text.IndexOf(':') + 1)..];
        captionText.text = text;
        if (((CustomOptionData)options[value]).Color1 == Color.black)
        {
            captionImage.gameObject.SetActive(false);
            captionImage1.gameObject.SetActive(false);
            captionImage2.gameObject.SetActive(false);
        }
        else if (((CustomOptionData)options[value]).Color2 == Color.black)
        {
            captionImage.color = ((CustomOptionData)options[value]).Color1;
            captionImage1.gameObject.SetActive(false);
            captionImage2.gameObject.SetActive(false);
        }
        else
        {
            captionImage.gameObject.SetActive(false);
            captionImage1.color = ((CustomOptionData)options[value]).Color1;
            captionImage2.color = ((CustomOptionData)options[value]).Color2;
        }
    }

    // Remove Number from option that contains text on Refresh
    public new void RefreshShownValue()
    {
        base.RefreshShownValue();
        string text = captionText.text;
        text = text[(text.IndexOf(':') + 1)..];
        captionText.text = text;
        if (((CustomOptionData)options[value]).Color1 == Color.black)
        {
            captionImage.gameObject.SetActive(false);
            captionImage1.gameObject.SetActive(false);
            captionImage2.gameObject.SetActive(false);
        }
        else if (((CustomOptionData)options[value]).Color2 == Color.black)
        {
            captionImage.gameObject.SetActive(true);
            captionImage.color = ((CustomOptionData)options[value]).Color1;
            captionImage1.gameObject.SetActive(false);
            captionImage2.gameObject.SetActive(false);
        }
        else
        {
            captionImage.gameObject.SetActive(false);
            captionImage1.gameObject.SetActive(true);
            captionImage1.color = ((CustomOptionData)options[value]).Color1;
            captionImage2.gameObject.SetActive(true);
            captionImage2.color = ((CustomOptionData)options[value]).Color2;
        }
    }

    // Get current dropdown value
    public int GetValue()
    {
        return ((CustomOptionData)options[value]).ID;
    }

    // Set current dropdown value
    public void SetValue(int val)
    {
        value = options.FindIndex(x => ((CustomOptionData)x).ID == val);
    }
}

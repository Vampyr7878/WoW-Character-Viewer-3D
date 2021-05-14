using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Custom dropdown that allows you to disable some options
public class CustomDropdown : Dropdown
{
    private int index = 0;

    //Constructor
    protected override GameObject CreateDropdownList(GameObject template)
    {
        index = 0;
        return base.CreateDropdownList(template);
    }

    //Make disabled options half transparent
    protected override DropdownItem CreateItem(DropdownItem itemTemplate)
    {
        DropdownItem item = base.CreateItem(itemTemplate);
        item.toggle.interactable = ((CustomOptionData)options[index]).Interactable;
        Color color = item.text.color;
        color.a = ((CustomOptionData)options[index]).Interactable ? 1f : 0.5f;
        item.text.color = color;
        index++;
        return item;
    }

    //Remove Number from option that contains text on Awake
    protected override void Awake()
    {
        base.Awake();
        string text = captionText.text;
        text = text.Substring(text.IndexOf(':') + 1);
        captionText.text = text;
    }

    //Remove Number from option that contains text on Start
    protected override void Start()
    {
        base.Start();
        string text = captionText.text;
        text = text.Substring(text.IndexOf(':') + 1);
        captionText.text = text;
    }

    //Remove Number from option that contains text on Refresh
    public new void RefreshShownValue()
    {
        base.RefreshShownValue();
        string text = captionText.text;
        text = text.Substring(text.IndexOf(':') + 1);
        captionText.text = text;
    }

    //Remove Number from option that contains text on Validation
    protected override void OnValidate()
    {
        base.OnValidate();
        string text = captionText.text;
        text = text.Substring(text.IndexOf(':') + 1);
        captionText.text = text;
    }

    //Get current dropdown value index
    public int GetValue()
    {
        return ((CustomOptionData)options[value]).Index;
    }

    //Set current dropdown value based on index
    public void SetValue(int val)
    {
        value = options.FindIndex(x => ((CustomOptionData)x).Index == val);
    }
}

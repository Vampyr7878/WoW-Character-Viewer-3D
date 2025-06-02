using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Custom tooltip
public class ItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image tooltip;
    public new Text name;
    public Text description;
    public RectTransform c;
    public Text slot;
    public Text type;
    public ScrollItem item;

    private readonly int minWidth = 150;

    // Show tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(true);
        //tooltip.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        //tooltip.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        float width = name.preferredWidth;
        width = width < minWidth ? minWidth : width;
        description.rectTransform.sizeDelta = new Vector2(width, description.rectTransform.sizeDelta.y);
        c.sizeDelta = new Vector2(width, c.sizeDelta.y);
    }

    // Hide tooltip
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }
}

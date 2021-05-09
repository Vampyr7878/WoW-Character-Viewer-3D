using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Custom tooltip
public class ItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject item;

    //Show tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        Image tooltip = Array.Find(item.GetComponentsInChildren<Image>(true), x => x.CompareTag("tooltip"));
        tooltip.gameObject.SetActive(true);
        tooltip.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        Vector3 pos = tooltip.rectTransform.position;
        float width = tooltip.GetComponentInChildren<Text>().preferredWidth + 20;
        width += width / 4 + pos.x;
        if (width > Screen.width)
        {
            tooltip.rectTransform.position = new Vector3(tooltip.rectTransform.position.x - width + Screen.width, tooltip.rectTransform.position.y, tooltip.rectTransform.position.z);
        }
    }

    //Hide tooltip
    public void OnPointerExit(PointerEventData eventData)
    {
        Image tooltip = Array.Find(item.GetComponentsInChildren<Image>(true), x => x.CompareTag("tooltip"));
        tooltip.gameObject.SetActive(false);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Script that autoscrolls scroll rect to currently selected item on show
[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Scrolling speed
    public float scrollSpeed = 10f;

    // Check mouseover
    private bool mouseOver = false;
    // List of selectable elements
    private List<Selectable> m_Selectables = new();
    // Rectangle to be scrolled
    private ScrollRect m_ScrollRect;
    // Next scroll position
    private Vector2 m_NextScrollPosition = Vector2.up;

    // Handler for OnEnable event
    private void OnEnable()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
    }

    private void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
        ScrollToSelected(true);
    }

    private void Update()
    {
        //  Scroll via input
        InputScroll();
        if (!mouseOver)
        {
            //  Lerp scrolling code
            m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.deltaTime);
        }
        else
        {
            m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        }
    }

    // scroll via input
    private void InputScroll()
    {
        if (m_Selectables.Count > 0)
        {
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical") || Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                ScrollToSelected(false);
            }
        }
    }

    // scroll to selected element
    private void ScrollToSelected(bool quickScroll)
    {
        int selectedIndex = -1;
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement)
        {
            selectedIndex = m_Selectables.IndexOf(selectedElement);
        }
        if (selectedIndex > -1)
        {
            if (quickScroll)
            {
                m_ScrollRect.normalizedPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
            else
            {
                m_NextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)m_Selectables.Count - 1)));
            }
        }
    }

    // Set mouseover
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    // Clear mosueover
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler
{
    public Canvas canvas;

    private RectTransform rectTransform;

    void IDragHandler.OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = transform.parent.GetComponent<RectTransform>();
    }
}

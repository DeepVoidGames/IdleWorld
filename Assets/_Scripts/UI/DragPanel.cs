using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 offset;
    private RectTransform canvasRectTransform;
    private RectTransform panelRectTransform;

    void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRectTransform = canvas.transform as RectTransform;
            panelRectTransform = transform as RectTransform;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out offset);
        offset = panelRectTransform.anchoredPosition - offset;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (panelRectTransform == null)
            return;

        Vector2 pointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out pointerPosition);
        panelRectTransform.anchoredPosition = pointerPosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Optional: Implement logic to snap back or finalize the drag
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        panelRectTransform.SetAsLastSibling(); // Ensure the dragged panel is on top
    }
}
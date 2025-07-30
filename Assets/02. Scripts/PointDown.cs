using UnityEngine;
using UnityEngine.EventSystems;

public class PointDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static bool isDragging;

    public void OnPointerDown(PointerEventData eventData) => isDragging = true;
    public void OnDrag(PointerEventData eventData) => isDragging = true;

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        GameManager2.matList.Clear();
    }
}

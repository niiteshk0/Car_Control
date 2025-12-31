using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccelerateUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool isPressed = false;

    public bool IsPressed() => isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
    
}

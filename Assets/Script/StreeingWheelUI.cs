using UnityEngine;
using UnityEngine.EventSystems;

public class StreeingWheelUI : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [Header("Steering Settings")]
    public float maxStreeing = 1f;
    public float rotationLimit = 180;

    [Header("Return Settings")]
    public float wheelReturnSpeed = 5f; // Speed at which wheel returns to center
    public bool smoothReturn = true; // Enable/disable smooth return

    private float currSteer;
    private float currentAngle = 0f;
    private float lastRawAngle = 0f;
    private bool isDragging = false;


    public float GetSteer() => currSteer;

    void Update()
    {
        if(!isDragging && smoothReturn && Mathf.Abs(currentAngle) > 0.1f)
        {
            currentAngle = Mathf.Lerp(currentAngle, 0f, Time.deltaTime * wheelReturnSpeed);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            currSteer = Mathf.Clamp((currentAngle / rotationLimit), -1f, 1f);

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        // Initialize the starting angle
        Vector2 centre = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        Vector2 direction = eventData.position - centre;
        lastRawAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 centre = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        Vector2 direction = eventData.position - centre;
        float rawAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        
        
        // deltaAngle  (avoids an immediate jump to the current pointer angle).
        float delta = Mathf.DeltaAngle(lastRawAngle, rawAngle);
        currentAngle += delta;

        // Clamp to rotation limits
        currentAngle = Mathf.Clamp(currentAngle, -rotationLimit, rotationLimit);
        
        lastRawAngle = rawAngle;
        

        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        currSteer = Mathf.Clamp((currentAngle / rotationLimit), -1f, 1f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if(!smoothReturn)
        {
            transform.rotation = Quaternion.identity;
            currSteer = 0;
            currentAngle = 0f;
            lastRawAngle = 0f;
            smoothReturn = true;
        }
    }
}
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0,4,-5);
    [SerializeField] float smothSpeed = 5f;
    [SerializeField] bool lookATTarget = true;
    Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if(target == null)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smothSpeed);

        if(lookATTarget)
            transform.LookAt(target);
    }
}

using UnityEngine;
using Unity.Netcode;

public class CameraFollow : NetworkBehaviour
{
    [SerializeField] Vector3 offset = new Vector3(0,4,-5);
    [SerializeField] float smothSpeed = 5f;
    [SerializeField] bool lookATTarget = true;
    
    Transform target;
    Vector3 velocity;
    Camera cam;
    AudioListener audioListener;

    private void Awake() {
        cam = GetComponent<Camera>();   
        audioListener = GetComponent<AudioListener>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Only enable camera for the owner of this car
        if (!IsOwner)
        {
            // Disable camera for other players' cars
            if (cam != null)
                cam.enabled = false;
            
            if (audioListener != null)
                audioListener.enabled = false;
            
            enabled = false; // Disable this script
            return;
        }
        
        // Set target to the parent car (not the camera itself!)
        target = transform.parent;
    }   

    void LateUpdate()
    {
        if(!target)
            return;

        Vector3 desiredPos = target.position + target.rotation * offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smothSpeed);

        if(lookATTarget)
            transform.LookAt(target);
    }
}

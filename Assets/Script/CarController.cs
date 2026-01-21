using System.Data.Common;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;

public class CarController : NetworkBehaviour
{
    [Header("Wheels")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Car Settings")]
    public float maxMotorTorque = 1000;
    public float maxSteerAngle = 100f;
    public bool isBackward = false;

    [Header("Drift Settings")]
    public float driftSpeedThreshold = 100f;
    public float driftFriction  = 1.8f; 
    public float normalFriction = 1.2f;
    public bool isDrifting = false;


    [Header("UI References")]  
    public StreeingWheelUI steeringUI;
    public AccelerateUI accelerateUI;

    [Header("Networking Settings")]
    public NetworkVariable<float> speed =
        new NetworkVariable<float>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    float steerInput;
    Rigidbody rb;

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        steeringUI = FindObjectOfType<StreeingWheelUI>();
        accelerateUI = FindObjectOfType<AccelerateUI>();

    }

    void FixedUpdate()
    {
        if(!IsOwner) return;

        steerInput = steeringUI.GetSteer();
        float speed = rb.linearVelocity.magnitude;

        // ApplyMotor(Input.GetAxis("Vertical"));

        ApplySteering();  

        bool isHardTurn = Mathf.Abs(steerInput * maxSteerAngle) > 80f;
        bool isFast = speed > driftSpeedThreshold;

        if(isHardTurn && isFast && !isBackward)
        {
            ApplyRealDrift(true);
        }
        else
        {   
            ApplyRealDrift(false);
        }

        if(accelerateUI.IsPressed())
        {
            float direction = isBackward ? -1f : 1f;
            ApplyMotor(direction);
        }
        else
        {
            ApplyMotor(0);
        }
        
        UpdateWheelMesh();      
    }

    public void ApplyMotor(float motorInput)
    {
        
        frontLeftCollider.motorTorque = motorInput*maxMotorTorque;
        frontRightCollider.motorTorque = motorInput*maxMotorTorque;
        if(isBackward)
        {
            rearLeftCollider.motorTorque = motorInput*maxMotorTorque;
            rearRightCollider.motorTorque = motorInput*maxMotorTorque;
        }
        else
        {
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
        }
    }

    public void ApplySteering()
    {
        float turn = -steerInput * maxSteerAngle ;
        if(Mathf.Abs(turn) <= 75)
        {
            isBackward = false;
            frontLeftCollider.steerAngle = turn;
            frontRightCollider.steerAngle = turn;
        }
        else
        {
            isBackward = true;
        }
       
    }

    void ApplyRealDrift(bool drift)
    {
        WheelFrictionCurve sideWays;

        sideWays = rearLeftCollider.sidewaysFriction;
        sideWays.stiffness = drift ? driftFriction : normalFriction;
        rearLeftCollider.sidewaysFriction = sideWays;

        sideWays = rearRightCollider.sidewaysFriction;
        sideWays.stiffness = drift ? driftFriction : normalFriction;
        rearRightCollider.sidewaysFriction = sideWays;
        isDrifting = drift;
    }

    void UpdateWheelMesh()
    {   
        UpdatesingleWheel(frontLeftCollider, frontLeftMesh);
        UpdatesingleWheel(frontRightCollider, frontRightMesh);
        UpdatesingleWheel(rearLeftCollider, rearLeftMesh);
        UpdatesingleWheel(rearRightCollider, rearRightMesh);
        
    }

    void UpdatesingleWheel(WheelCollider collider, Transform transform )
    {
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        transform.position = pos;
        transform.rotation = rot;
    }

}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    [Header("Car Wheels (Wheel Collider)")] // Assign wheel Colliders through the inspector
    public WheelCollider frontLeft;

    public WheelCollider frontRight;
    public WheelCollider backLeft;
    public WheelCollider backRight;

    [Header("Car Wheels (Transform)")] // Assign wheel Transform(Mesh render) through the inspector
    public Transform wheelFL;

    public Transform wheelFR;
    public Transform wheelBL;
    public Transform wheelBR;

    [Header("Car Front (Transform)")] // Assign a Gameobject representing the front of the car
    public Transform carFront;

    [Header("General Parameters")] // Look at the documentation for a detailed explanation 
    public List<string> NavMeshLayers;

    public int MaxSteeringAngle = 45;
    public int MaxRPM = 150;

    [Header("Debug")]
    public bool ShowGizmos;

    public bool Debugger;

    [Header("Destination Parameters")] // Look at the documentation for a detailed explanation
    public bool Patrol = true;

    public Transform CustomDestination;

    [HideInInspector]
    public bool move; // Look at the documentation for a detailed explanation

    readonly float AIFOV = 60;
    readonly float MovementTorque = 1;
    bool allowMovement;
    int currentWayPoint;
    int Fails;
    float LocalMaxSpeed;
    int NavMeshLayerBite;

    Vector3 PostionToFollow = Vector3.zero;

    public List<Vector3> Waypoints { get; } = new();

    public List<Vector3> FutureWaypoints
    {
        get
        {
            var buff = Waypoints.Skip(currentWayPoint).ToList();
            buff.Insert(0, transform.position);
            return buff;
        }
    }

    void Awake()
    {
        currentWayPoint = 0;
        allowMovement = true;
        move = true;
    }

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        CalculateNavMashLayerBite();
    }

    void FixedUpdate()
    {
        UpdateWheels();
        ApplySteering();
        PathProgress();
    }

    void OnDrawGizmos() // shows a Gizmos representing the waypoints and AI FOV
    {
        if (ShowGizmos)
        {
            for (var i = 0; i < Waypoints.Count; i++)
            {
                if (i == currentWayPoint)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    if (i > currentWayPoint)
                        Gizmos.color = Color.red;
                    else
                        Gizmos.color = Color.green;
                }
                Gizmos.DrawWireSphere(Waypoints[i], 2f);
            }
            CalculateFOV();
        }

        void CalculateFOV()
        {
            Gizmos.color = Color.white;
            var totalFOV = AIFOV * 2;
            var rayRange = 10.0f;
            var halfFOV = totalFOV / 2.0f;
            var leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            var rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
            var leftRayDirection = leftRayRotation * transform.forward;
            var rightRayDirection = rightRayRotation * transform.forward;
            Gizmos.DrawRay(carFront.position, leftRayDirection * rayRange);
            Gizmos.DrawRay(carFront.position, rightRayDirection * rayRange);
        }
    }

    void CalculateNavMashLayerBite()
    {
        if (NavMeshLayers == null || NavMeshLayers[0] == "AllAreas")
        {
            NavMeshLayerBite = NavMesh.AllAreas;
        }
        else if (NavMeshLayers.Count == 1)
        {
            NavMeshLayerBite += 1 << NavMesh.GetAreaFromName(NavMeshLayers[0]);
        }
        else
        {
            foreach (var Layer in NavMeshLayers)
            {
                var I = 1 << NavMesh.GetAreaFromName(Layer);
                NavMeshLayerBite += I;
            }
        }
    }

    //Checks if the agent has reached the currentWayPoint or not. If yes, it will assign the next waypoint as the currentWayPoint depending on the input
    void PathProgress()
    {
        wayPointManager();
        Movement();
        ListOptimizer();

        void wayPointManager()
        {
            if (currentWayPoint >= Waypoints.Count)
            {
                allowMovement = false;
            }
            else
            {
                PostionToFollow = Waypoints[currentWayPoint];
                allowMovement = true;
                if (Vector3.Distance(carFront.position, PostionToFollow) < 2)
                    currentWayPoint++;
            }

            if (currentWayPoint >= Waypoints.Count - 3)
                CreatePath();
        }

        void CreatePath()
        {
            if (CustomDestination == null)
            {
                if (Patrol)
                {
                    RandomPath();
                }
                else
                {
                    debug("No custom destination assigned and Patrol is set to false", false);
                    allowMovement = false;
                }
            }
            else
            {
                CustomPath(CustomDestination);
            }

        }

        void ListOptimizer()
        {
            if (currentWayPoint > 1 && Waypoints.Count > 30)
            {
                Waypoints.RemoveAt(0);
                currentWayPoint--;
            }
        }
    }

    public void RandomPath() // Creates a path to a random destination
    {
        var path = new NavMeshPath();
        Vector3 sourcePostion;

        if (Waypoints.Count == 0)
        {
            var randomDirection = Random.insideUnitSphere * 100;
            randomDirection += transform.position;
            sourcePostion = carFront.position;
            Calculate(randomDirection, sourcePostion, carFront.forward, NavMeshLayerBite);
        }
        else
        {
            sourcePostion = Waypoints[Waypoints.Count - 1];
            var randomPostion = Random.insideUnitSphere * 100;
            randomPostion += sourcePostion;
            var direction = (Waypoints[Waypoints.Count - 1] - Waypoints[Waypoints.Count - 2]).normalized;
            Calculate(randomPostion, sourcePostion, direction, NavMeshLayerBite);
        }

        void Calculate(Vector3 destination, Vector3 sourcePostion, Vector3 direction, int NavMeshAreaByte)
        {
            if (NavMesh.SamplePosition(destination, out var hit, 150, 1 << NavMesh.GetAreaFromName(NavMeshLayers[0])) &&
                NavMesh.CalculatePath(sourcePostion, hit.position, NavMeshAreaByte, path) && path.corners.Length > 2)
            {
                if (CheckForAngle(path.corners[1], sourcePostion, direction))
                {
                    Waypoints.AddRange(path.corners.ToList());
                    debug("Random Path generated successfully", false);
                }
                else
                {
                    if (CheckForAngle(path.corners[2], sourcePostion, direction))
                    {
                        Waypoints.AddRange(path.corners.ToList());
                        debug("Random Path generated successfully", false);
                    }
                    else
                    {
                        debug("Failed to generate a random path. Waypoints are outside the AIFOV. Generating a new one",
                        false);
                        Fails++;
                    }
                }
            }
            else
            {
                debug("Failed to generate a random path. Invalid Path. Generating a new one", false);
                Fails++;
            }
        }
    }

    public void CustomPath(Transform destination) //Creates a path to the Custom destination
    {
        var path = new NavMeshPath();
        Vector3 sourcePostion;

        if (Waypoints.Count == 0)
        {
            sourcePostion = carFront.position;
            Calculate(destination.position, sourcePostion, carFront.forward, NavMeshLayerBite);
        }
        else
        {
            if (Fails > 0 && Waypoints.Count > 2)
            {
                Waypoints.RemoveAt(Waypoints.Count - 1);
                Fails = 0; //crappy way to do it but only for testing
            }
            sourcePostion = Waypoints[^1]; // last waypoint in list
            var direction = (Waypoints[^1] - Waypoints[^2]).normalized; // direction of last segment
            Calculate(destination.position, sourcePostion, direction, NavMeshLayerBite);
        }
        return;

        void Calculate(Vector3 destination, Vector3 sourcePostion, Vector3 direction, int NavMeshAreaBite)
        {
            if (NavMesh.SamplePosition(destination, out var hit, 150, NavMeshAreaBite) &&
                NavMesh.CalculatePath(sourcePostion, hit.position, NavMeshAreaBite, path))
            {
                if (path.corners.ToList().Count() > 1 &&
                    CheckForAngle(path.corners[1], sourcePostion, direction))
                {
                    Waypoints.AddRange(path.corners.ToList());
                    debug("Custom Path generated successfully", false);
                }
                else
                {
                    if (path.corners.Length > 2 &&
                        CheckForAngle(path.corners[2], sourcePostion, direction))
                    {
                        Waypoints.AddRange(path.corners.ToList());
                        debug("Custom Path generated successfully", false);
                    }
                    else
                    {
                        debug("Failed to generate a Custom path. Waypoints are outside the AIFOV. Generating a new one",
                        false);
                        Fails++;
                    }
                }
            }
            else
            {
                debug("Failed to generate a Custom path. Invalid Path. Generating a new one", false);
                Fails++;
            }
        }
    }

    bool CheckForAngle(Vector3 pos, Vector3 source,
        Vector3 direction) //calculates the angle between the car and the waypoint 
    {
        var distance = (pos - source).normalized;
        var CosAngle = Vector3.Dot(distance, direction);
        var Angle = Mathf.Acos(CosAngle) * Mathf.Rad2Deg;

        if (Angle < AIFOV)
            return true;
        return false;
    }

    void ApplyBrakes() // Apply brake torque 
    {
        frontLeft.brakeTorque = 5000;
        frontRight.brakeTorque = 5000;
        backLeft.brakeTorque = 5000;
        backRight.brakeTorque = 5000;
    }


    void UpdateWheels() // Updates the wheel's postion and rotation
    {
        ApplyRotationAndPostion(frontLeft, wheelFL);
        ApplyRotationAndPostion(frontRight, wheelFR);
        ApplyRotationAndPostion(backLeft, wheelBL);
        ApplyRotationAndPostion(backRight, wheelBR);
    }

    void ApplyRotationAndPostion(WheelCollider targetWheel, Transform wheel) // Updates the wheel's postion and rotation
    {
        targetWheel.ConfigureVehicleSubsteps(5, 12, 15);

        Vector3 pos;
        Quaternion rot;
        targetWheel.GetWorldPose(out pos, out rot);
        wheel.position = pos;
        wheel.rotation = rot;
    }

    void ApplySteering() // Applies steering to the Current waypoint
    {
        var relativeVector = transform.InverseTransformPoint(PostionToFollow);
        var SteeringAngle = relativeVector.x / relativeVector.magnitude * MaxSteeringAngle;
        if (SteeringAngle > 15) LocalMaxSpeed = 100;
        else LocalMaxSpeed = MaxRPM;

        frontLeft.steerAngle = SteeringAngle;
        frontRight.steerAngle = SteeringAngle;
    }

    void Movement() // moves the car forward and backward depending on the input
    {
        if (move && allowMovement)
            allowMovement = true;
        else
            allowMovement = false;

        if (allowMovement)
        {
            frontLeft.brakeTorque = 0;
            frontRight.brakeTorque = 0;
            backLeft.brakeTorque = 0;
            backRight.brakeTorque = 0;

            var SpeedOfWheels = (int)((frontLeft.rpm + frontRight.rpm + backLeft.rpm + backRight.rpm) / 4);

            if (SpeedOfWheels < LocalMaxSpeed)
            {
                backRight.motorTorque = 400 * MovementTorque;
                backLeft.motorTorque = 400 * MovementTorque;
                frontRight.motorTorque = 400 * MovementTorque;
                frontLeft.motorTorque = 400 * MovementTorque;
            }
            else if (SpeedOfWheels < LocalMaxSpeed + LocalMaxSpeed * 1 / 4)
            {
                backRight.motorTorque = 0;
                backLeft.motorTorque = 0;
                frontRight.motorTorque = 0;
                frontLeft.motorTorque = 0;
            }
            else
            {
                ApplyBrakes();
            }

        }
        else
        {
            ApplyBrakes();
        }
    }

    void debug(string text, bool IsCritical)
    {
        if (Debugger)
        {
            if (IsCritical)
                Debug.LogError(text);
            else
                Debug.Log(text);
        }
    }
}

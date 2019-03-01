using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianMovement : MonoBehaviour {
    public float sensorLength = 5.0f;

    public float sightDistance = 5.0f;
    public float fovAngle = 90;

    public float speed = 10.0f;
    public float sideRaycastDistance = 2.0f;
    public float travelDistanceFromStart = 110.0f;
    public float secondsBetweenSidesteps = 1.0f;
    public enum sideStepDirection {Always_Left, Always_Right, Random, Repeat_Last, Repeat_First, None};
    public sideStepDirection presetSideStepDirection;
    public Vector3 target;
    public int pedestrianGroup = 0;
    public int pedestrianID = 0;
    public int numCollisions = 0;

    public GameObject[] otherPedestrians;

    public bool finishedTrial = false;

    Collider myCollider;

    private bool inCollision = false;

    private sideStepDirection pedestrianSideStepDirection;

    private Vector3 initialTransform;
    private Vector3 sideStepTransform;

    private float secondsSinceSidestepping = 0;

    // Use this for initialization
    void Start() {
        initialTransform = transform.position;
        target = initialTransform + (transform.forward * travelDistanceFromStart);
        myCollider = transform.GetComponent<Collider>();

        //otherPedestrians = GameObject.FindGameObjectsWithTag("Obstacle");

        InitializeSidestep();
    }

    void InitializeSidestep() {
        // Roll random sidestep
        if (presetSideStepDirection == sideStepDirection.Random) {
            int rand = Random.Range(0,2);
            pedestrianSideStepDirection = (sideStepDirection)rand;
        } else {
            pedestrianSideStepDirection = presetSideStepDirection;
        }
        secondsSinceSidestepping = 0;
    }

    // Update is called once per frame
    void Update() {
        if (finishedTrial) {
            Debug.Log("FINISHED TRIAL");
            // Stop movement if trial has finished
            return;
        }

        if (transform.position.z < -58 || transform.position.z > 58) {
            // Remove pedestrian if out-of-bounds
            DestroyGameObject();
        }

        //RaycastHit hit;
        //float step = speed * Time.deltaTime;
        float step = speed;
        if (TrialManager.useDeltaTimeSetting) {
            step *= Time.deltaTime;
        }
        target = initialTransform + (transform.forward * travelDistanceFromStart);

        inCollision = false;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

/*
        if (Physics.Raycast(transform.position, transform.forward, out hit, (sensorLength + transform.localScale.z))) {
            sidestep(hit, step);
        } else if (Physics.Raycast(transform.position + new Vector3(-sideRaycastDistance,0,0), transform.forward, out hit, (sensorLength + transform.localScale.z))) {
            sidestep(hit, step);
        } else if (Physics.Raycast(transform.position + new Vector3(sideRaycastDistance,0,0), transform.forward, out hit, (sensorLength + transform.localScale.z))) {
            sidestep(hit, step);
        } else {
            inCollision = false;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
*/
        //foreach (GameObject ped in otherPedestrians) {
        //    Debug.Log(ped);
        //}

        /*
        // #TODO: add testing mode
        if (inCollision) {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        } else {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
        */
    }

    public void ReactToObstacle(Collider c) {
        //Debug.Log("Reacting to " + c + "    " + c.tag);

        // Don't react to non-pedestrians and itself
        if (c.tag != "Obstacle" || c == myCollider)
            return;

        // React to pedestrians within group
            //TODO

        // React to opposing pedestrians
        if (c.gameObject.GetComponent<PedestrianMovement>() && c.gameObject.GetComponent<PedestrianMovement>().pedestrianGroup != pedestrianGroup) {
            //Debug.Log("Reacting to opposing pedestrian");
            sidestep(c.transform.position, speed);
        }

    }

    void sidestep(Vector3 obstaclePosition, float step) {
        if (TrialManager.useDeltaTimeSetting) {
            step *= Time.deltaTime;
        }
        sideStepTransform = new Vector3(1,0,0);
        secondsSinceSidestepping += Time.deltaTime;
        //float angle = Vector3.SignedAngle(obstaclePosition, transform.position, transform.forward);

        float angle = Mathf.Atan2(obstaclePosition.z - transform.position.z, obstaclePosition.x - transform.position.x) * 180 / Mathf.PI;

        if (pedestrianGroup == 0)
            angle *= -1;


        //Debug.Log("Signed angle: " + angle + "   " + pedestrianGroup);

        float fovAngleHalf = fovAngle/2.0f;

        float centerAngle = 10;


        if (angle > 90 + fovAngleHalf || angle < 90 - fovAngleHalf)
            return;


        if (angle > 90 + centerAngle) {
            //Sidestep Right
            sideStepTransform = new Vector3(1,0,0);
            secondsSinceSidestepping += Time.deltaTime;
        } else if (angle < 90 - centerAngle) {
            // Sidestep Left
            sideStepTransform = new Vector3(-1,0,0);
            secondsSinceSidestepping += Time.deltaTime;
        } else {
            // Center collision - Figure out a sidestep
            switch (pedestrianSideStepDirection) {
                case sideStepDirection.Always_Left:
                // Always sidestep to the left
                    sideStepTransform = new Vector3(1,0,0);
                    secondsSinceSidestepping += Time.deltaTime;
                break;
                case sideStepDirection.Always_Right:
                //Always sidestep to the right
                    sideStepTransform = new Vector3(-1,0,0);
                    secondsSinceSidestepping += Time.deltaTime;
                break;
                case sideStepDirection.None:
                 // No sidestep
                    secondsSinceSidestepping += Time.deltaTime;
                break;
                case sideStepDirection.Random:
                     //TODO
                break;
                default:
                    sideStepTransform = new Vector3(0,0,0);
                break;
            }
        }

        // Movement
        transform.position += sideStepTransform * (step);
    }
/*
    void sidestep(RaycastHit hit, float step) {


        // Destroy pedestrian on collision with wall
        // #TODO: Replace with regrouping
        if (hit.collider.tag == "Wall") {
            DestroyGameObject();
        }

        // Prevent own collider from triggering sidestep
        if (hit.collider.tag != "Obstacle" || hit.collider == myCollider)
            return;

        if (hit.collider.GetComponent<PedestrianMovement>().pedestrianGroup == pedestrianGroup) {
            inCollision = false;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            return;
        }

        if (!inCollision) {
            inCollision = true;
            numCollisions += 1;
        }

        // Determine side step movement
        switch (pedestrianSideStepDirection) {
            case sideStepDirection.Always_Left:
            // Always sidestep to the left
                sideStepTransform = new Vector3(1,0,0);
                secondsSinceSidestepping += Time.deltaTime;
            break;
            case sideStepDirection.Always_Right:
            //Always sidestep to the right
                sideStepTransform = new Vector3(-1,0,0);
                secondsSinceSidestepping += Time.deltaTime;
            break;
            case sideStepDirection.None:
             // No sidestep
                secondsSinceSidestepping += Time.deltaTime;
            break;
            default:
                sideStepTransform = new Vector3(0,0,0);
            break;
        }

        // Change sidestep
        if (secondsSinceSidestepping > secondsBetweenSidesteps) {
           sideStepTransform = new Vector3(0,0,0);
           InitializeSidestep();
        }

        // Movement
        transform.position += sideStepTransform * (step);
    }
*/
    void DestroyGameObject() {
        finishedTrial = true;
        //Destroy(gameObject);
    }

    void OnDrawGizmos() {

        // Draw debug ray to show direction of movement
        Gizmos.DrawRay(transform.position, transform.forward * (sensorLength + transform.localScale.z));


        //Gizmos.DrawRay(transform.position, transform.forward * (sensorLength + transform.localScale.z));
        //Gizmos.DrawRay(transform.position + new Vector3(-sideRaycastDistance,0,0), transform.forward * (sensorLength + transform.localScale.z));
        //Gizmos.DrawRay(transform.position + new Vector3(sideRaycastDistance,0,0), transform.forward * (sensorLength + transform.localScale.z));
    }
}
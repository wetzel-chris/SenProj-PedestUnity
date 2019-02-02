using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianMovement : MonoBehaviour {
    public float sensorLength = 5.0f;
    public float speed = 10.0f;
    public float sideRaycastDistance = 2.0f;
    public float travelDistanceFromStart = 110.0f;
    public float secondsBetweenSidesteps = 1.0f;
    public enum sideStepDirection {Left, Right, None, Random};
    public sideStepDirection presetSideStepDirection;
    public Vector3 target;
    public int pedestrianGroup = 0;
    public int pedestrianID = 0;
    public int numCollisions = 0;

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

        InitializeSidestep();
    }

    void InitializeSidestep() {
        //Roll random sidestep
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
            return;
        }
        if (transform.position.z < -58 || transform.position.z > 58) {
            DestroyGameObject();
        }

        RaycastHit hit;
        //float step = speed * Time.deltaTime;
        float step = speed;
        if (TrialManager.useDeltaTimeSetting) {
            step *= Time.deltaTime;
        }
        target = initialTransform + (transform.forward * travelDistanceFromStart);

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

        /*
        if (inCollision) {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        } else {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
        }
        */
    }

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
            case sideStepDirection.Left:
                sideStepTransform = new Vector3(1,0,0);
                secondsSinceSidestepping += Time.deltaTime;
            break;
            case sideStepDirection.Right:
                sideStepTransform = new Vector3(-1,0,0);
                secondsSinceSidestepping += Time.deltaTime;
            break;
            case sideStepDirection.None:
                // No movement
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

    void DestroyGameObject() {
        finishedTrial = true;
        //Destroy(gameObject);
    }

    void OnDrawGizmos() {
        Gizmos.DrawRay(transform.position, transform.forward * (sensorLength + transform.localScale.z));
        Gizmos.DrawRay(transform.position + new Vector3(-sideRaycastDistance,0,0), transform.forward * (sensorLength + transform.localScale.z));
        Gizmos.DrawRay(transform.position + new Vector3(sideRaycastDistance,0,0), transform.forward * (sensorLength + transform.localScale.z));
    }
}
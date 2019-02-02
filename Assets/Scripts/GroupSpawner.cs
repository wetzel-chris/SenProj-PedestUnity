using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSpawner : MonoBehaviour {
    public groupFormation spawnFormation;
    public Rigidbody pedestrian;
    public float spacing_x = 1f;
    public float spacing_z = 1f;

	// Use this for initialization
	void Start () {
	    if (spawnFormation == groupFormation.Random) {
	        spawnFormation = (groupFormation)Random.Range(1,3);
	    }

        int pedestrianInitialID = 0;

        // Quick fix for spawner facing other way
	    if (transform.rotation.y == 1) {
	        pedestrianInitialID += 3;
	        spacing_z = -spacing_z;
	    }

	    if (spawnFormation == groupFormation.Trial) {
	        spawnFormation = TrialManager.groupFormationToTestSetting;
	    }

	    switch (spawnFormation) {
	        case groupFormation.Concave_V:
                Rigidbody concavepedestrianClone  = (Rigidbody) Instantiate(pedestrian, new Vector3(transform.position.x, transform.position.y, transform.position.z - spacing_z), transform.rotation);
                concavepedestrianClone.gameObject.GetComponent<PedestrianMovement>().pedestrianID = pedestrianInitialID;
                Rigidbody concavepedestrianClone2 = (Rigidbody) Instantiate(pedestrian, new Vector3(transform.position.x + spacing_x, transform.position.y, transform.position.z), transform.rotation);
                concavepedestrianClone2.gameObject.GetComponent<PedestrianMovement>().pedestrianID = pedestrianInitialID + 1;
                Rigidbody concavepedestrianClone3 = (Rigidbody) Instantiate(pedestrian, new Vector3(transform.position.x - spacing_x, transform.position.y, transform.position.z), transform.rotation);
                concavepedestrianClone3.gameObject.GetComponent<PedestrianMovement>().pedestrianID = pedestrianInitialID + 2;
	        break;
	        case groupFormation.Convex_V:
                Rigidbody convexPedestrianClone  = (Rigidbody) Instantiate(pedestrian, transform.position, transform.rotation);
                convexPedestrianClone.gameObject.GetComponent<PedestrianMovement>().pedestrianID = pedestrianInitialID;
                Rigidbody convexPedestrianClone2 = (Rigidbody) Instantiate(pedestrian, new Vector3(transform.position.x + spacing_x, transform.position.y, transform.position.z - spacing_z), transform.rotation);
                convexPedestrianClone2.gameObject.GetComponent<PedestrianMovement>().pedestrianID = pedestrianInitialID + 1;
                Rigidbody convexPedestrianClone3 = (Rigidbody) Instantiate(pedestrian, new Vector3(transform.position.x - spacing_x, transform.position.y, transform.position.z - spacing_z), transform.rotation);
                convexPedestrianClone3.gameObject.GetComponent<PedestrianMovement>().pedestrianID = pedestrianInitialID + 2;
            break;
            case groupFormation.Side_By_Side:
                Rigidbody sidePedestrianClone  = (Rigidbody) Instantiate(pedestrian, transform.position, transform.rotation);
                sidePedestrianClone.gameObject.GetComponent<PedestrianMovement>().pedestrianID = pedestrianInitialID;
                Rigidbody sidePedestrianClone2 = (Rigidbody) Instantiate(pedestrian, new Vector3(transform.position.x + spacing_x, transform.position.y, transform.position.z), transform.rotation);
                sidePedestrianClone2.gameObject.GetComponent<PedestrianMovement>().pedestrianID = pedestrianInitialID + 1;
                Rigidbody sidePedestrianClone3 = (Rigidbody) Instantiate(pedestrian, new Vector3(transform.position.x - spacing_x, transform.position.y, transform.position.z), transform.rotation);
                sidePedestrianClone3.gameObject.GetComponent<PedestrianMovement>().pedestrianID = pedestrianInitialID + 2;
            break;

	    }
	    Destroy(gameObject);

	}
}

public enum groupFormation {Random, Concave_V, Convex_V, Side_By_Side, Trial};
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrialManager : MonoBehaviour {

    public static bool useDeltaTimeSetting = false;
    public bool useDeltaTime = false;
    public static groupFormation groupFormationToTestSetting;
    public groupFormation groupFormationToTest;
    public int trialsToRun = 10;

    private int trialID = 1;
    private float timeSinceStart = 0.0f;

	// Use this for initialization
	void Awake () {
	    // Destroy if trial manager already exists
	    if (GameObject.FindGameObjectsWithTag("TrialManager").Length > 1) {
	        Destroy(gameObject);
	    }

	    DontDestroyOnLoad(this.gameObject);
	    groupFormationToTestSetting = groupFormationToTest;
		useDeltaTimeSetting = useDeltaTime;
	}
	
	// Update is called once per frame
	void Update () {
	    timeSinceStart += Time.deltaTime;
	    int pedestriansLeft = 6;
	    foreach (GameObject ge in GameObject.FindGameObjectsWithTag("Obstacle")) {
	        PedestrianMovement pm = ge.GetComponent<PedestrianMovement>();
            if (pm.finishedTrial) {
                pedestriansLeft -= 1;
            }
	    }
	    if (GameObject.FindGameObjectsWithTag("Spawner").Length <= 0 && pedestriansLeft <= 0) {
	        EndTrialRun();
	    }
	    //if(GameObject.FindGameObjectsWithTag("Obstacle").Length <= 0 && GameObject.FindGameObjectsWithTag("Spawner").Length <= 0) {
        //    EndTrialRun();
        //}
	}

	void EndTrialRun() {
	    WriteToCsv();
        timeSinceStart = 0;
        Debug.Log(trialID);
        if (trialID >= trialsToRun) {
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
        trialID += 1;
        SceneManager.LoadScene("Pedestrian Simulation", LoadSceneMode.Single);
	}

	void WriteToCsv() {
        string filePath = Application.dataPath + "/csv/trial_data-" + groupFormationToTest + ".csv";
        bool fileExists = File.Exists(filePath);
        StreamWriter writer = new StreamWriter(filePath, append: true);
        if (!fileExists) {
            //writer = new StreamWriter(filePath);
            // Header
            writer.WriteLine("id,formation,time,col0,col1,col2,col3,col4,col5");
        }

        // Get Pedestrian collision data
        int[] collisions = new int[] {0,0,0,0,0,0};
        foreach (GameObject ge in GameObject.FindGameObjectsWithTag("Obstacle")) {
            PedestrianMovement pm = ge.GetComponent<PedestrianMovement>();
            collisions[pm.pedestrianID] = pm.numCollisions;
            Debug.Log(pm.pedestrianID + "   " + pm.numCollisions);
            Debug.Log(collisions[pm.pedestrianID]);
        }

        Debug.Log("----");
        Debug.Log(collisions[0]);
        Debug.Log(collisions[1]);
        Debug.Log(collisions[2]);

        // Output data
        writer.WriteLine(trialID +
            "," + groupFormationToTest +
            "," + timeSinceStart +
            "," + collisions[0] +
            "," + collisions[1] +
            "," + collisions[2] +
            "," + collisions[3] +
            "," + collisions[4] +
            "," + collisions[5]
            );

        writer.Flush();
        writer.Close();
	}
}

using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using Unity.Mathematics;

public class follow_dirs : Agent
{
    // Start is called before the first frame update
    Rigidbody rBody;
    private List<(double, double)> dirs = FileReader.read();
    private float timer = 0.0f;
    private int index;
    public Transform Target;
    public bool useVectorObs;
    private void Start()
    {
        rBody = gameObject.GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        // index = 0;
        rBody.velocity = Vector3.zero;
        // Move the target to a new spot
        // Target.localPosition = new Vector3(UnityEngine.Random.value * 8 - 4,
        //                                    0.5f,
        //                                    UnityEngine.Random.value * 8 - 4);
        // Debug.Log(dirs.Count);
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(transform.InverseTransformDirection(rBody.velocity));
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        timer += Time.deltaTime;
        index = (int)(timer / 0.04195);
        // Debug.Log(index);
        var dist = vectorAction[0];
        Debug.Log(dist);
        var dir = Quaternion.Euler(0,vectorAction[1], 0);
        Debug.Log(dir);
        transform.localRotation = dir;
        rBody.velocity = transform.forward * dist;
    }

    public override void Heuristic(float[] actionsOut)
    {
        try {
        actionsOut[0] = (float) dirs[index].Item1;
        actionsOut[1] = (float) dirs[index].Item2;
        } catch (ArgumentOutOfRangeException) {
            rBody.transform.position = new Vector3(789, 2, -535);
            AddReward(1f);
            timer = 0.0f;
            index = 0;
            EndEpisode();
        }  
    }

    private void onTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "End") {
            AddReward(1f);
            rBody.transform.position = new Vector3(789, 2, -535);
            EndEpisode();
            Debug.Log("SUCCESS");

        }
    }

    // void OnCollisionEnter(Collision collision) 
    // {
    //     if (collision.gameObject.tag == "End") {
    //         SetReward(1);
    //         rBody.transform.position = new Vector3(789, 2, -535);
    //         EndEpisode();
    //         Debug.Log("SUCCESS");
    //     }
    // }
}

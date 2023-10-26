using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;

    private Transform lastCheckpoint;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Platform")
        lastCheckpoint.position = playerRef.transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Spikes")
        {
            playerRef.transform.position = lastCheckpoint.position;
            //Deal damage
        }
    }
}

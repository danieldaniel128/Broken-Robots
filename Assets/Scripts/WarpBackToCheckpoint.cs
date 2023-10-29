using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpBackToCheckpoint : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;

    public Vector3 lastCheckpoint;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Spikes")
        {
            playerRef.transform.position = lastCheckpoint;
        }
    }
}

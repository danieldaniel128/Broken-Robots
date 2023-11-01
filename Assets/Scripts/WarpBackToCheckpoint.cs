using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpBackToCheckpoint : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;

    public Transform lastCheckpoint;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Spikes"))
            playerRef.transform.position = lastCheckpoint.position;
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag.Equals("Checkpoint"))
        {
            CheckpointLocation checkpointHolder = collider.GetComponent<CheckpointLocation>();
            lastCheckpoint = checkpointHolder.checkpointLocation;
            Debug.Log(checkpointHolder.checkpointLocation.position);
        }
    }
}

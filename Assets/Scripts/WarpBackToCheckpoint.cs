using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpBackToCheckpoint : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;
    [SerializeField] private Health playerHP;

    public Transform lastCheckpoint;

    private void OnCollisionEnter(Collision collision) //On contact with impassables
    {
        if (collision.gameObject.tag.Equals("Spikes"))
            playerRef.transform.position = lastCheckpoint.position;
    }

    private void Update() //On death
    {
        if (playerHP.CurrentHealth <= 0)
        {
            playerRef.transform.position = lastCheckpoint.position;
            playerHP.CurrentHealth = 9;
        }
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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [SerializeField] private WarpBackToCheckpoint playerRef;

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Platform")
        {
            playerRef.lastCheckpoint = playerRef.transform.position;
        }
    }
}

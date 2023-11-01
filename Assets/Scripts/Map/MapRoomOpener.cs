using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoomOpener : MonoBehaviour
{
    [SerializeField] MapManager mapManager;
    [SerializeField] string mapRoomToOpen;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag.Equals("Player"))
            mapManager.OpenRoomID(mapRoomToOpen);
    }
}

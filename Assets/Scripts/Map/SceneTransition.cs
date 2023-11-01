using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string stageToTransitionTo;
    [SerializeField] private Vector3 nextStageSpawnpoint;

    [SerializeField] private GameObject playerRef;

    [SerializeField] private GameObject[] Stages;
    [SerializeField] private GameObject currentStage;

    [SerializeField] private MapManager mapManager;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            //MapJSON reader = new MapJSON();
            //List<MapRoom> roomList = reader.ReadJSON();
            //Add update to the correct room here
            //reader.outputUpdatedJSON(roomList);
            foreach (var stage in Stages)
            {
                if (stage.name == stageToTransitionTo)
                {
                    stage.SetActive(true);
                    mapManager.OpenRoomID(stageToTransitionTo);
                    playerRef.transform.position = nextStageSpawnpoint;
                    currentStage.SetActive(false);
                    break;
                }
            }
        }
    }
}

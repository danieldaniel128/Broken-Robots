using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    public string sceneToTransitionTo;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            string dataPath = Application.dataPath + "/RoomState.json";
            string[] dataLines = File.ReadAllLines(dataPath);
            List<MapRoom> reader = new List<MapRoom>();
            foreach (string line in dataLines)
            {
                reader.Add(MapRoom.ReadFromJSON(line));
            }
            //Add Json update

            SceneManager.LoadScene(sceneToTransitionTo, LoadSceneMode.Single);
        }
    }
}

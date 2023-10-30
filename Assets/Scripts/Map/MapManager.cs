using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private RoomScript[] roomArray;
    [SerializeField] private GameObject MapCanvasGO;
    [SerializeField] private string OpenMapKey;

    private void Awake()
    {
        //Read from the json and open all the rooms that signal true
    }

    private void Update()
    {
        if (Input.GetKeyDown(OpenMapKey))
        {
            Debug.Log("Map was opened using " + OpenMapKey);
            MapCanvasGO.SetActive(true);
        }
        if (Input.GetKeyUp(OpenMapKey))
        {
            Debug.Log("Map was closed using " + OpenMapKey);
            MapCanvasGO.SetActive(false);
        }
    }

    //This method needs a trigger to tell it when the player entered a room and what room it was
    public void OpenRoomID(string roomID) //roomID is the name of the gameObject parenting the image
    {
        foreach (var room in roomArray)
        {
            if (roomID == room.name)
            {
                room.OpenRoom();
            }
        }
    }

    //This method needs a trigger to tell it when a collectible was obtained and which room it was in
    public void GetRoomCollectibleID(string roomID) //roomID is the name of the gameObject parenting the image
    {
        foreach (var room in roomArray)
        {
            if (roomID == room.name)
            {
                room.GetCollectible();
            }
        }
    }
}

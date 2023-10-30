using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public class MapRoom
{
    public string roomID;
    public bool openState;
    public static MapRoom ReadFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<MapRoom>(jsonString);
    }
}

public class MapJSON : MonoBehaviour
{
    //Move me to the start game button, use outputNewJson upon starting a new game
    //Update each room's state when you enter a new room/get a collectible
    //When you load a new scene, construct the map at MapManager using these values to determine which rooms to add to the map

    public List<MapRoom> rooms = new List<MapRoom>();

    public void outputNewJSON()
    {
        rooms.Add(new MapRoom() { roomID = "Tutorial1", openState = true });
        rooms.Add(new MapRoom() { roomID = "Tutorial2", openState = false });
        rooms.Add(new MapRoom() { roomID = "A1", openState = false });
        rooms.Add(new MapRoom() { roomID = "A2", openState = false });
        rooms.Add(new MapRoom() { roomID = "A2_2", openState = false });
        rooms.Add(new MapRoom() { roomID = "A2_2_Collectible", openState = false });
        rooms.Add(new MapRoom() { roomID = "A3", openState = false });
        rooms.Add(new MapRoom() { roomID = "A_RestArea", openState = false });
        rooms.Add(new MapRoom() { roomID = "A_RestArea_Collectible", openState = false });
        rooms.Add(new MapRoom() { roomID = "A4", openState = false });
        rooms.Add(new MapRoom() { roomID = "BOSS", openState = false });

        string stringOutput = "";

        foreach (var room in rooms)
        {
            stringOutput += JsonUtility.ToJson(room) + "\n";
        }

        Debug.Log(stringOutput);

        File.WriteAllText(Application.dataPath + "/RoomState.json", stringOutput);
    }

    public void outputUpdatedJSON(List<JSONReader> reader)
    {

    }
}

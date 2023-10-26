using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    private bool isRoomOpen = false;
    private bool roomCollectibleGot = false;
    [SerializeField] private GameObject roomImage;
    [SerializeField] private GameObject noCollectibleRoomImage; //Optional, only for rooms with collectibles

    public void OpenRoom()
    {
        isRoomOpen = true;
        roomImage.SetActive(true);
    }

    public void GetCollectible()
    {
        roomCollectibleGot = true;
        noCollectibleRoomImage.SetActive(true);
    }
}

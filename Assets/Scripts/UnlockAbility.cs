using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockAbility : MonoBehaviour
{
    [SerializeField] GameObject collectibleRef;
    [SerializeField] MapManager mapManager;
    [SerializeField] string abilityToUnlock;

    private void OnTriggerEnter(Collider collider)
    {
        if (abilityToUnlock.Equals("Dash"))
        {//Fix map design for that to work
            //mapManager.GetRoomCollectibleID("A4");
            collectibleRef.SetActive(false);
            Debug.Log("Unlock Dash Here");
        }
        else if (abilityToUnlock.Equals("EMP"))
        {
            mapManager.GetRoomCollectibleID("A2_2");
            collectibleRef.SetActive(false);
            Debug.Log("Unlock EMP Show Here");
        }
    }
}

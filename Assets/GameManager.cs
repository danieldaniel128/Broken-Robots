using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool IsInBossRoom;
    public bool IsBossDead;
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    void WinGame()
    {
        SceneManager.LoadScene(0);
    }
}

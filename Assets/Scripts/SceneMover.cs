using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("A4 1", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

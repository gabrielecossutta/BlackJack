using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//script used to load levels (Used on GUI) 
public class LoadLevels : MonoBehaviour
{
    //Load the Game Level
    private void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    //Load the menu level
    private void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
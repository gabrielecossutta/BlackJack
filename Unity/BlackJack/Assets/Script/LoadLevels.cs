using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//script used to load levels (Used on GUI) 
public class LoadLevels : MonoBehaviour
{
    //Load the Game Level
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    //Load the menu level
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    //close the Game
    public void Quit()
    {
        Application.Quit();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] public GameObject mainMenu, howToPlayMenu;


    private void Awake()
    {
        howToPlayMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void goToLoad()
    {
        PlayerPrefs.SetInt("load", 1);
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void goToNewGame()
    {
        PlayerPrefs.SetInt("load", 0);
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void goToHowToPlay()
    {
        mainMenu.SetActive(false);
        howToPlayMenu.SetActive(true);
    }

    public void goToMenu()
    {
        mainMenu.SetActive(true);
        howToPlayMenu.SetActive(false);
    }

    public void exitGame() { 
        Application.Quit();
    }
}

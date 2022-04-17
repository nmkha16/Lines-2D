using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IngameMenu : MonoBehaviour
{
    public static IngameMenu Instance { get; private set;}

    [SerializeField] GameObject pauseMenu, endMenu, saveAlert;

    
    private bool isPaused;
    private void Start()
    {
        Instance = this;
        isPaused = false;
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                goToResume();
            }
            else
            {
                goToPause();
            }
        }
    }

    public void goToPause()
    {
        isPaused=true;
        GridManager.Instance.disableAllCellsCollider(true);
        GridManager.Instance.disableAllBallsCollider(true);
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void goToResume()
    {
        isPaused = false;
        GridManager.Instance.disableAllCellsCollider(false);
        GridManager.Instance.disableAllBallsCollider(false);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void goToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void goToEndMenu()
    {
        // set a new high score record
        if (IngameHUD.Instance.getTotalScore() > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore",IngameHUD.Instance.getTotalScore());
        }

        Time.timeScale = 0f;
        endMenu.SetActive(true);
    }

    public void gotoSaveAlert()
    {
        saveAlert.SetActive(true);
        Invoke("disableSaveAlert", 1f);
    }


    private void disableSaveAlert()
    {
        saveAlert.SetActive(false);
    }
}

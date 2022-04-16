using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu, saveAlert;

    private bool isPaused;
    private void Start()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
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

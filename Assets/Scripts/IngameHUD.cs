using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IngameHUD : MonoBehaviour
{
    public static IngameHUD Instance { get; private set; }

    [SerializeField] public TMP_Text score, timer;

    private float timerToUpdate = 1f;
    private int totalTime = 0;
    private int totalScore = 0;

    private void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // should update timer every 1s
        timerToUpdate -= Time.deltaTime;
        if (timerToUpdate <= 0f)
        {
            totalTime += 1;
            timer.text = totalTime.ToString();

            timerToUpdate = 1f;
        }
    }


    // method to call to update score from other script
    public void updateScore(int newScore)
    {
        totalScore+= newScore;
        score.text = newScore.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
public class IngameHUD : MonoBehaviour
{
    public static IngameHUD Instance { get; private set; }

    [SerializeField] public TMP_Text score, timer, saveMessage;

    private float timerToUpdate = 1f;
    private int totalTime = 0;
    private int totalScore = 0;

    private string applicationPath;

    private void Start()
    {
        Instance = this;
        applicationPath = Application.persistentDataPath + "/Save";

    }

    // Update is called once per frame
    void Update()
    {
        // should update timer every 1s
        timerToUpdate -= Time.deltaTime;
        if (timerToUpdate <= 0f)
        {
            updateTimer(1);

            timerToUpdate = 1f;
        }
    }


    //method to call to update timer from other script
    public void updateTimer(int newTimer)
    {
        totalTime += newTimer;
        timer.text = totalTime.ToString();
    }

    // method to call to update score from other script
    public void updateScore(int newScore)
    {
        totalScore+= newScore;
        score.text = newScore.ToString();
    }

    // method to call save function from gridmanager, will return a save state class
    public void saveGame()
    {
        Savestate saveFile = GridManager.Instance.save();

        string serializeSaveFile = JsonUtility.ToJson(saveFile);

        // do the store save
        try
        {
            Debug.Log(applicationPath);
            if (!Directory.Exists(applicationPath))
            {
                Directory.CreateDirectory(applicationPath);
            }
            writeToFile(applicationPath+"/save.sav0", serializeSaveFile);
            saveMessage.text = "SAVE COMPLETED!";
        }
        catch
        {
            saveMessage.text = "SAVE FAILED!!!";
        }

    }

    // this method return saveState
    public Savestate loadGame()
    {
        string serializedSaveFile = readFromFile(applicationPath + "/save.sav0");
        if (serializedSaveFile == "") return null;
        return JsonUtility.FromJson<Savestate>(serializedSaveFile);
    }
    

    public int getTotalTimer()
    {
        return totalTime;
    }

    public int getTotalScore()
    {
        return totalScore;
    }

    private void writeToFile(string path,string content)
    {
        File.WriteAllText(path, content);
    }

    private string readFromFile(string path)
    {
        if (!File.Exists(path)) return "";
        return File.ReadAllText(path);
    }
}

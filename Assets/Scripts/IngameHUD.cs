using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using Newtonsoft.Json;
public class IngameHUD : MonoBehaviour
{
    public static IngameHUD Instance { get; private set; }

    [SerializeField] public TMP_Text score, timer, saveMessage, highScore;
    private float timerToUpdate = 1f;
    private int totalTime = 0;
    private int totalScore = 0;

    private string applicationPath;

    JsonSerializerSettings setting = new JsonSerializerSettings();
    

    private void Awake()
    {
        Instance = this;
        // need more insight on this code
        // apparently persistentDataPath doesn't work on some machines including mine
        //applicationPath = Application.persistentDataPath + "/Save";

        // the real problem was project setting .net was net 2.0 standard, changing to .net 4.x fixed the problem
        // the real suspect is JSON.NET is incompatible at NET2.0
        applicationPath = Path.Combine(Application.persistentDataPath, "/Save/");

        // this return path C:\Users\desol\AppData\Roaming\Lines98\Save\
       // applicationPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "/Lines98/Save/";
       // Debug.Log(applicationPath);
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        highScore.text = PlayerPrefs.GetInt("highscore").ToString();
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

        string serializeSaveFile = JsonConvert.SerializeObject(saveFile,setting);

        // do the store save
        try
        {
            if (!Directory.Exists(applicationPath))
            {
                Directory.CreateDirectory(applicationPath);
            }
            writeToFile(applicationPath+"save.sav0", serializeSaveFile);
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
        Debug.Log(applicationPath);
        string serializedSaveFile = readFromFile(applicationPath + "save.sav0");
        if (serializedSaveFile== "") return null;

        SavestateFormatter savestateFormatter = JsonConvert.DeserializeObject<SavestateFormatter>(serializedSaveFile, setting);

        //Debug.Log(savestateFormatter.ToString());

        return new Savestate(savestateFormatter);
        //return JsonConvert.DeserializeObject<Savestate>(serializedSaveFile,setting);
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

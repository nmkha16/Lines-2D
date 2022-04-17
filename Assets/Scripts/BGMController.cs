using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    private static BGMController instance = null;
    public static BGMController Instance
    {
        get { return instance; }
    }

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
    public AudioSource _boomSE, _moveSE;
    public static SoundEffectController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}

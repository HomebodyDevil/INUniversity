using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static Action<float> OnPlayerGetEXP;
    public static Action OnPlayerLevelUp;

    static PlayerManager instance;

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public static PlayerManager Instance()
    {
        if (instance == null)
            return null;
        return instance;
    }

    private void Awake()
    {
        Initialize();
    }
}

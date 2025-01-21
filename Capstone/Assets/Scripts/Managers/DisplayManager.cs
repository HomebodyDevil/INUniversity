using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    private static DisplayManager instance;
    [Range(30, 120)] [SerializeField] private int frameRate;

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

    public static DisplayManager Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    private void Awake()
    {
        Initialize();        
    }

    private void Start()
    {
        Application.targetFrameRate = frameRate;
    }
}

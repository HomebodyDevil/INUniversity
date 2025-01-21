using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    //private MapUIManager mapUIManager;
    //private BattleManager battleManager;

    //private List<IUIManager> uiManagers;

    private A_UIManager currentUIManager;

    private void Initialize()
    {
        if (instance == null)
        {            
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public static UIManager Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    private void Awake()
    {
        Initialize();
    }

    //private void Start()
    //{
    //    uiManagers = new List<IUIManager>(SceneManagerEX.SCENE_COUNT);
    //    Debug.Log(uiManagers.Count + " : SASDF");
    //}

    public void UpdateCurrentManager(A_UIManager uiManager)
    {
        //SceneManagerEX.Scenes currentScene = SceneManagerEX.CurrentScene();

        //Debug.Log(string.Format("uiManagers Capacity : {0}\ncurrentScene : {1}", uiManagers.Count, (int)currentScene));

        //uiManagers[(int)currentScene] = uiManager;     

        currentUIManager = uiManager;
    }

    public A_UIManager CurrentUIManager()
    {
        //SceneManagerEX.Scenes currentScene = SceneManagerEX.CurrentScene();

        //return uiManagers[(int)currentScene];

        return currentUIManager;
    }
}

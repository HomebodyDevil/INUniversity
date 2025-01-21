using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX : MonoBehaviour
{
    public const int SCENE_COUNT = 3;

    public static Action OnSwitchSceneToIntro;
    public static Action OnSwitchSceneToMap;
    public static Action OnSwitchSceneToBattle;

    public enum Scenes
    {
        IntroScene = 0,
        MapScene = 1,
        BattleScene = 2,
    }

    private static SceneManagerEX instance;

    [SerializeField] private int sceneCount;
    
    public Transform prevPlayerTransform;
    public Transform prevEnemyTransform;

    private GameObject enemyInstance;

    private static Scenes currentScene;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {      
        currentScene = Scenes.MapScene;
    }

    private void Update()
    {
        // SwitchToBattleScene();
        // SwitchToMapScene();
    }

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

    public static SceneManagerEX Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    public static Scenes CurrentScene()
    {
        return currentScene;
    }

    public void SwitchToMapScene()
    {
        if (currentScene != Scenes.MapScene)
        {
            currentScene = Scenes.MapScene;

            ChangePlayerTransform(false, currentScene);

            //if (enemyInstance)          // 다시 Map으로 돌아갈 때 enemy 객체가 있다면, 제거해준다.
            //    Destroy(enemyInstance);

            SceneManager.LoadScene("MapScene");

            CameraManager.Instance().SetCamera(CameraManager.CameraKind.MapCamera);

            OnSwitchSceneToMap.Invoke();
        }

        BattleManager.Instance().RemoveEnemy();
        BattleManager.Instance().HealToPlayer(1.0f);

        UIManager.Instance().CurrentUIManager().OnCanclePanel();

        PlayerMovement.OnMakePlayerCanMove.Invoke();
    }

    public void SwitchToBattleScene()
    {
        if (currentScene != Scenes.BattleScene)
        {
            currentScene = Scenes.BattleScene;

            ChangePlayerTransform(true, currentScene);

            SceneManager.LoadScene("BattleScene");

            CameraManager.Instance().SetCamera(CameraManager.CameraKind.BattleCamera);

            BattleManager.Instance().UpdateDropLists();
            BattleManager.Instance().SetEnemy();

            OnSwitchSceneToBattle.Invoke();
        }

        UIManager.Instance().CurrentUIManager().OnCanclePanel();    // Panel들 다 없애는 용도?

        PlayerMovement.OnMakePlayerCantMove.Invoke();

        //if (Input.GetKeyDown(KeyCode.F2) && currentScene != "BattleScene")
        //{
        //    ChangePlayerTransform(true);
        //    TransferEnemyToBattleScene();

        //    SceneManager.LoadSceneAsync("BattleScene");
        //    UIManager.Instance().SwitchUITo(Scenes.BattleScene);

        //    CameraManager.Instance().SetCamera(CameraManager.CameraKind.BattleCamera);

        //    //SceneManager.UnloadSceneAsync(currentScene);
        //    currentScene = "BattleScene";
        //}
    }

    public void ActivateMiniGamesPanel()
    {
        Debug.Log("ACTIVATED");
    }

    //private void TransferEnemyToBattleScene()
    //{
    //    if (BattleManager.Instance().currentEnemyData != null)
    //    {
    //        // Debug.Log(BattleManager.Instance().currentEnemyData.prefabPath);

    //        GameObject enemy = Resources.Load<GameObject>(BattleManager.Instance().currentEnemyData.battleEnemyPrefabPath);

    //        enemyInstance = Instantiate(enemy);
    //        enemyInstance.transform.parent = gameObject.transform;
    //        enemyInstance.transform.position = CameraManager.Instance().enemyTransformInBattle.position;
    //        enemyInstance.transform.rotation = CameraManager.Instance().enemyTransformInBattle.rotation;
    //    }
    //    else
    //        Debug.Log("Enemy Is Null");
    //}

    private void ChangePlayerTransform(bool rememberPlayerPrevTransform, Scenes sceneTo)
    {
        if (rememberPlayerPrevTransform)
        {
            prevPlayerTransform.position = Player.Instance().gameObject.transform.position;
            prevPlayerTransform.rotation = Player.Instance().gameObject.transform.rotation;
        }

        //switch(sceneTo)
        //{
        //    case Scenes.IntroScene:
        //        break;
        //    case Scenes.MapScene:
        //        Player.Instance().gameObject.transform.position =
        //                    prevPlayerTransform.position;
        //        Player.Instance().gameObject.transform.rotation =
        //                    prevPlayerTransform.rotation;
        //        break;
        //    case Scenes.BattleScene:
        //        //Player.Instance().gameObject.transform.position =
        //        //            CameraManager.Instance().playerTransformInBattle.position;
        //        //Player.Instance().gameObject.transform.rotation =
        //        //            CameraManager.Instance().playerTransformInBattle.rotation;
        //        break;
        //}
    }
}

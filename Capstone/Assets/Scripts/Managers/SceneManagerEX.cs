using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerEX : MonoBehaviour
{
    [SerializeField] Image loadingPanel;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] float fadeTime;

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

        loadingPanel.gameObject.SetActive(false);
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

            // 흐으음
            //SceneManager.LoadScene("MapScene");           
            //CameraManager.Instance().SetCamera(CameraManager.CameraKind.MapCamera);
            //OnSwitchSceneToMap.Invoke();

            StartCoroutine("WaitForLoadScene", "MapScene");
        }

        BattleManager.Instance().RemoveEnemy();
        BattleManager.Instance().HealToPlayer(1.0f);

        //UIManager.Instance().CurrentUIManager().OnCanclePanel();

        PlayerMovement.OnMakePlayerCanMove.Invoke();
    }

    public void SwitchToBattleScene()
    {
        if (currentScene != Scenes.BattleScene)
        {
            currentScene = Scenes.BattleScene;

            ChangePlayerTransform(true, currentScene);

            // 흐으음.
            //SceneManager.LoadScene("BattleScene");
            //CameraManager.Instance().SetCamera(CameraManager.CameraKind.BattleCamera);

            //BattleManager.Instance().UpdateDropLists();
            //BattleManager.Instance().SetEnemy();

            //OnSwitchSceneToBattle.Invoke();
            //loadingPanel.SetActive(false);

            StartCoroutine("WaitForLoadScene", "BattleScene");
            //Debug.Log("TESTING");
        }

        //UIManager.Instance().CurrentUIManager().OnCanclePanel();    // Panel들 다 없애는 용도?

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

    IEnumerator WaitForLoadScene(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        loadingPanel.color = new Color(loadingPanel.color.r, loadingPanel.color.g, loadingPanel.color.b, 0.0f);
        loadingPanel.gameObject.SetActive(true);
        yield return StartCoroutine("Fade", true);

        UIManager.Instance().CurrentUIManager().OnCanclePanel();    // Panel들 다 없애는 용도?

        int count = 0;
        while(!op.isDone)
        {
            count++;
            Debug.Log(count);
            yield return null;

            if (op.progress < 0.9f)
            {
                Debug.Log("Scene Loading");                
            }
            else
            {
                op.allowSceneActivation = true;

                if (sceneName == "BattleScene")
                {
                    CameraManager.Instance().SetCamera(CameraManager.CameraKind.BattleCamera);

                    BattleManager.Instance().UpdateDropLists();
                    BattleManager.Instance().SetEnemy();

                    OnSwitchSceneToBattle.Invoke();
                }
                else if (sceneName == "MapScene")
                {
                    CameraManager.Instance().SetCamera(CameraManager.CameraKind.MapCamera);
                    OnSwitchSceneToMap.Invoke();
                }
                else
                {
                    Debug.Log("WTF?");
                }

                break;
            }
        }

        Debug.Log("Finished");
        yield return new WaitForSecondsRealtime(0.05f);

        yield return StartCoroutine("Fade", false);
        loadingPanel.gameObject.SetActive(false);
    }

    IEnumerator Fade(bool isIn)
    {
        float tick = 0;
        while(tick < fadeTime)
        {
            //Debug.Log(tick);

            yield return null;
            tick += Time.deltaTime;
            
            float alpha = isIn ? Mathf.Lerp(0.0f, 1.0f, (tick / fadeTime)) : Mathf.Lerp(1.0f, 0.0f, (tick / fadeTime));
            if (alpha >= 0.99)
                alpha = 1.0f;
            else if (alpha <= 0.01)
                alpha = 0.0f;

            Color newColor = new Color(loadingPanel.color.r, loadingPanel.color.g, loadingPanel.color.b, alpha);
            loadingPanel.color = newColor;
            loadingText.color = newColor;
        }
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

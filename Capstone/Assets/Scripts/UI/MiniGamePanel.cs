using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class MiniGamePanel : MonoBehaviour, IPanel
{
    public static Action EnableMiniGamesPanel;
    public static Action DisableMiniGamesPanel;

    private MapUIManager uiManager;
    private GridLayoutGroup gridGroup;

    private IPanel panelInterface;

    [SerializeField] private float paddingX = 20.0f;

    private void Awake()
    {
        EnableMiniGamesPanel -= EnablePanel;
        EnableMiniGamesPanel += EnablePanel;

        DisableMiniGamesPanel -= DisablePanel;
        DisableMiniGamesPanel += DisablePanel;
    }

    private void OnEnable()
    {
        //EnableMiniGamesPanel -= EnablePanel;
        //EnableMiniGamesPanel += EnablePanel;

        //DisableMiniGamesPanel -= DisablePanel;
        //DisableMiniGamesPanel += DisablePanel;
    }

    public void OnClicked()
    {
        //Debug.Log("Clicked Items");

        bool isActive = gameObject.activeSelf;
        

        if (isActive)
        {
            //gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            UIManager.Instance().CurrentUIManager().ActivateCanclePanel();
            UIManager.Instance().CurrentUIManager().activeObject.Push(gameObject);

            // Time.timeScale = 0;

            //Player player = Player.Instance();
            //PlayerCameraController cameraController = player.gameObject.GetComponent<PlayerCameraController>();
            //cameraController.InActivateAllCamera();

            CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            cinemachineBrain.enabled = false;
        }
    }

    private void Start()
    {
        //gameObject.SetActive(false);

        panelInterface = GetComponent<IPanel>();
        gridGroup = GetComponent<GridLayoutGroup>();

        panelInterface.Initialize();
    }

    public void EnablePanel()
    {
        gameObject.SetActive(true);
    }

    public void DisablePanel()
    {
        gameObject.SetActive(false);
    }

    void IPanel.Initialize()
    {
        RectTransform rect = GetComponent<RectTransform>();

        float width = rect.rect.width;
        float height = rect.rect.height;

        float widthPercent = width * 0.8f;
        float heightPercent = height * 0.9f;

        float cell = Math.Min(widthPercent, heightPercent) / 3;

        gridGroup.cellSize = new Vector2(heightPercent / 3, heightPercent / 3);
    }

    public void GetUIManager()
    {
        StartCoroutine(GetUIManagerCoroutine());
    }

    public IEnumerator GetUIManagerCoroutine()
    {
        while (true)
        {
            uiManager = MapUIManager.Instance();
            if (uiManager != null)
                yield break;
            else
                yield return null;
        }
    }
}

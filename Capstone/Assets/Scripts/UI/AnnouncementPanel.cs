using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncementPanel : MonoBehaviour
{
    //public static bool isInitialized = false;

    //public static Action<string, string> Act_SetAnnoucementPanelText;
    //public static Action Act_EnableAnnouncementPanel;
    //public static Action Act_DisableAnnouncementPanel;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    private void Awake()
    {
        //isInitialized = false;

        //Act_SetAnnoucementPanelText -= SetAnnouncementText;
        //Act_SetAnnoucementPanelText += SetAnnouncementText;

        //Act_EnableAnnouncementPanel -= EnablePanel;
        //Act_EnableAnnouncementPanel += EnablePanel;

        //Act_DisableAnnouncementPanel -= DisablePanel;
        //Act_DisableAnnouncementPanel += DisablePanel;
    }

    private void Start()
    {
        //isInitialized = true;
    }

    private void OnDestroy()
    {
        //Act_EnableAnnouncementPanel -= EnablePanel;
        //Act_DisableAnnouncementPanel -= DisablePanel;
    }

    public void OnOKButtonClicked()
    {
        DisablePanel();
    }

    public void SetAnnouncementText(string title, string content)
    {
        titleText.text = title;
        contentText.text = content;
    }

    public void EnablePanel()
    {
        gameObject.SetActive(true);
    }

    public void DisablePanel()
    {
        gameObject.SetActive(false);
    }
}

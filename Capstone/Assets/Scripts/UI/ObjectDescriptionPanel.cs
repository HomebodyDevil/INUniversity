using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDescriptionPanel : MonoBehaviour
{
    //public enum OBJECT_TYPE
    //{
    //    Item = 0,
    //    Equipment = 1,
    //    Card = 2,
    //}

    public static Action Act_EnableObjectDescriptionPanel;
    public static Action Act_DisableObjectDescriptionPanel;
    public static Action<string, string, string> Act_UpdateObjectDescription;

    [SerializeField] private Image objectImage;
    [SerializeField] private TextMeshProUGUI objectText;
    [SerializeField] private TextMeshProUGUI objectDescriptionText;
    [SerializeField] private GameObject canclePanel;
    [SerializeField] private RectTransform descriptionPanel;
    [SerializeField] private RectTransform objectPanel;
    [SerializeField] private RectTransform objectDescriptionPanel;

    private void Awake()
    {
        Act_EnableObjectDescriptionPanel -= EnableDescriptionPanel;
        Act_EnableObjectDescriptionPanel += EnableDescriptionPanel;

        Act_DisableObjectDescriptionPanel -= DisableDescriptionPanel;
        Act_DisableObjectDescriptionPanel += DisableDescriptionPanel;

        Act_UpdateObjectDescription -= UpdateObjectDescription;
        Act_UpdateObjectDescription += UpdateObjectDescription;
    }

    //private void Update()
    //{
    //    Initialize();
    //}

    //private void Start()
    //{
    //    Initialize();
    //}

    private void OnDestroy()
    {
        Act_EnableObjectDescriptionPanel -= EnableDescriptionPanel;
        Act_DisableObjectDescriptionPanel -= DisableDescriptionPanel;
        Act_UpdateObjectDescription -= UpdateObjectDescription;
    }

    private void Initialize()
    {
        float width = objectDescriptionPanel.rect.width;
        float height = (descriptionPanel.rect.height - objectPanel.rect.height) * 0.9f;

        objectDescriptionPanel.sizeDelta = new Vector2(width, height);
    }

    public void UpdateObjectDescription(string imagePath, string name, string description)
    {
        // enum���� �߾ �ƴµ�
        // enum���� ���� �Ŵ���? Ŭ������ �����ϴ� �� �������ٵ�, �׷��� �ٲٱ� �����Ƽ� �׳� int�� ������ ����.

        // type : 0 -> Item
        // type : 1 -> Equipment
        // type : 2 -> Card

        objectImage.sprite = Resources.Load<Sprite>(imagePath);
        objectText.text = name;
        objectDescriptionText.text = description;
    }

    private void EnableDescriptionPanel()
    {
        gameObject.SetActive(true);
    }

    private void DisableDescriptionPanel()
    {
        gameObject.SetActive(false);
    }
}

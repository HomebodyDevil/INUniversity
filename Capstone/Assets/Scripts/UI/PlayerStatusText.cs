using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatusText : MonoBehaviour
{
    public static Action Act_UpdatePlayerStatusValueText;

    [SerializeField] private TextMeshProUGUI statusValueText;

    private void Awake()
    {
        Act_UpdatePlayerStatusValueText -= UpdatePlayerStatusValueText;
        Act_UpdatePlayerStatusValueText += UpdatePlayerStatusValueText;
    }

    private void OnEnable()
    {
        if (statusValueText == null)
            return;

        UpdatePlayerStatusValueText();
    }

    private void OnDestroy()
    {
        Act_UpdatePlayerStatusValueText -= UpdatePlayerStatusValueText;
    }

    private void Start()
    {
        //UpdatePlayerStatusValueText();
        StartCoroutine(UpdateText());
    }

    private void OnDisable()
    {
        StopCoroutine("UpdateText");
    }

    private void UpdatePlayerStatusValueText()
    {
        //Debug.Log("AAA");

        PlayerSpecManager playerSpecManager = PlayerSpecManager.Instance();

        int attackPoint = (int)playerSpecManager.currentPlayerAttackPoint;
        int maxHP = (int)playerSpecManager.maxPlayerHP;
        int currHP = (int)playerSpecManager.currentPlayerHP;
        int maxCost = (int)playerSpecManager.maxPlayerCost;

        string content = string.Format("{0, 5}\n{1, 5}\n{2, 5}\n{3, 5}", attackPoint, maxHP, currHP, maxCost);
        statusValueText.text = content;
    }

    IEnumerator UpdateText()
    {
        while(true)
        {
            UpdatePlayerStatusValueText();
            yield return new WaitForSeconds(0.5f);
        }
    }
}

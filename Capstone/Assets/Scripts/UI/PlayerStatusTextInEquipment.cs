using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatusTextInEquipment : MonoBehaviour
{
    public static Action Act_UpdatePlayerStatusTextInEquipment;

    private TextMeshProUGUI text;

    private void Awake()
    {
        Act_UpdatePlayerStatusTextInEquipment -= UpdatePlayerStatusText;
        Act_UpdatePlayerStatusTextInEquipment += UpdatePlayerStatusText;
    }

    private void OnEnable()
    {
        if (text == null)
        {
            return;
        }

        UpdatePlayerStatusText();
    }

    private void OnDestroy()
    {
        Act_UpdatePlayerStatusTextInEquipment -= UpdatePlayerStatusText;
    }

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        //UpdatePlayerStatusText();

        StartCoroutine(UpdateText());
    }

    private void OnDisable()
    {
        StopCoroutine("UpdateText");
    }

    private void UpdatePlayerStatusText()
    {
        PlayerSpecManager playerSpecManager = PlayerSpecManager.Instance();

        int attack = (int)playerSpecManager.currentPlayerAttackPoint;
        int maxHP = (int)playerSpecManager.maxPlayerHP;
        int maxCost = (int)playerSpecManager.maxPlayerCost;

        string content = string.Format("Attack\t{0, 5}\nMaxHP\t{1, 5}\nMaxCost\t{2, 5}", attack, maxHP, maxCost);
        text.text = content;
    }

    IEnumerator UpdateText()
    {
        while(true)
        {
            UpdatePlayerStatusText();
            yield return new WaitForSeconds(0.5f);
        }
    }
}

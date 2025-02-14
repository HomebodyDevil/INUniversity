using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHPInBattle : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        StartCoroutine("UpdateText");
    }

    private void OnDisable()
    {
        StopCoroutine("UpdateText");
    }

    IEnumerator UpdateText()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.1f);

            text.text = string.Format("HP : ( {0} / {1} )",
                                        (int)PlayerSpecManager.Instance().currentPlayerHP,
                                        (int)PlayerSpecManager.Instance().maxPlayerHP);
        }
    }
}

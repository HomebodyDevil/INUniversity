using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EXPTextInMap : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        StartCoroutine("UpdateEXPText");
    }

    private void OnDisable()
    {
        StopCoroutine("UpdateEXPText");
    }

    IEnumerator UpdateEXPText()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1.5f);

            text.text = string.Format("EXP : ( {0:0.0} / {1:0.0} )",
                                        PlayerSpecManager.Instance().currentPlayerEXP,
                                        PlayerSpecManager.Instance().maxPlayerEXP);
        }   
    }
}

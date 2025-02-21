using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCostInBattle : MonoBehaviour
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

            text.text = string.Format("Cost : ( {0:0.0} / {1:0.0} )",
                                        (float)PlayerSpecManager.Instance().currentPlayerCost,
                                        (float)PlayerSpecManager.Instance().maxPlayerCost);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCostInBattle : MonoBehaviour
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

            text.text = string.Format("EnemyCost : ( {0:0.0} / {1:0.0} )",
                                        (float)BattleManager.Instance().currentEnemyCost,
                                        (float)BattleManager.Instance().currentEnemyMaxCost);
        }
    }
}

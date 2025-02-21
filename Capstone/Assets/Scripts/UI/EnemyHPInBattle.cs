using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyHPInBattle : MonoBehaviour
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

            text.text = string.Format("EnemyHP : ( {0:0.0} / {1:0.0} )",
                                        (float)BattleManager.Instance().currentEnemyHP,
                                        (float)BattleManager.Instance().currentEnemyMaxHP);
        }
    }
}

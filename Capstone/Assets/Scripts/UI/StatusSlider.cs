using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusSlider : MonoBehaviour
{
    public enum SliderType
    {
        HP,
        Cost,
        EXP,
        EnemyHP,
        EnemyCost,
    }

    [SerializeField] private SliderType sliderType;
    [SerializeField] private GameObject fillArea;

    private Slider statusSlider;

    private void Start()
    {
        statusSlider = GetComponent<Slider>();

        StartUpdateSliderValue();
    }

    private void StartUpdateSliderValue()
    {
        switch (sliderType)
        {
            case SliderType.HP:
                StartCoroutine("UpdateHPValueCoroutine");
                break;
            case SliderType.Cost:
                StartCoroutine("UpdateCostValueCoroutine");
                break;
            case SliderType.EXP:
                StartCoroutine("UpdateEXPValueCoroutine");
                break;
            case SliderType.EnemyHP:
                StartCoroutine("UpdateEnemyHPValueCoroutine");
                break;
            case SliderType.EnemyCost:
                StartCoroutine("UpdateEnemyCostValueCoroutine");
                break;
        }
    }

    IEnumerator UpdateHPValueCoroutine()
    {
        while(true)
        {
            statusSlider.value = PlayerSpecManager.Instance().currentPlayerHP /
                        PlayerSpecManager.Instance().maxPlayerHP;

            CheckValue();

            yield return null;
        }
    }

    IEnumerator UpdateCostValueCoroutine()
    {
        while(true)
        {
            statusSlider.value = PlayerSpecManager.Instance().currentPlayerCost /
                        PlayerSpecManager.Instance().maxPlayerCost;

            CheckValue();

            yield return null;

        }
    }

    IEnumerator UpdateEXPValueCoroutine()
    {
        while(true)
        {
            statusSlider.value = PlayerSpecManager.Instance().currentPlayerEXP /
                        PlayerSpecManager.Instance().maxPlayerEXP;

            CheckValue();

            yield return null;
        }
    }

    IEnumerator UpdateEnemyHPValueCoroutine()
    {
        while (true)
        {
            statusSlider.value = BattleManager.Instance().currentEnemyHP /
                        BattleManager.Instance().currentEnemyMaxHP;

            CheckValue();

            yield return null;
        }
    }

    IEnumerator UpdateEnemyCostValueCoroutine()
    {
        while (true)
        {
            statusSlider.value = BattleManager.Instance().currentEnemyCost /
                        BattleManager.Instance().currentEnemyMaxCost;

            CheckValue();

            yield return null;
        }
    }

    private void CheckValue()
    {
        if (statusSlider.value <= 0)
            fillArea.SetActive(false);
        else
            fillArea.SetActive(true);
    }
}

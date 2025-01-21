using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    // TextMeshPro를 사용하기 위한 자료형은 TextMeshProUGUI임.

    public enum StatusPanelMode
    {
        Map = 0,
        Battle = 1,
    }

    [SerializeField] private StatusPanelMode mode;
    [SerializeField] private Slider firstSlider;
    [SerializeField] private TextMeshProUGUI firstText;
    [SerializeField] private Slider secondSlider;
    [SerializeField] private TextMeshProUGUI secondText;

    //private void Start()
    //{
    //    //switch(mode)
    //    //{
    //    //    case StatusPanelMode.Map:
    //    //        OnMapMode();
    //    //        break;
    //    //    case StatusPanelMode.Battle:
    //    //        OnBattleMode();
    //    //        break;
    //    //}
    //}

    //private void OnMapMode()
    //{

    //}

    //private void OnBattleMode()
    //{

    //}
}

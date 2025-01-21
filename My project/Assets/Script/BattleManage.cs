using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManage : MonoBehaviour
{
    public Slider Hp;
    public Slider Cost;

    private void Start()
    {
        Hp = GameObject.Find("Canvas").transform.Find("HPBar").GetComponent<Slider>();
        Cost = GameObject.Find("Canvas").transform.Find("CostBar").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        Cost.value = Cost.value + 7.5f * Time.deltaTime;
    }
}

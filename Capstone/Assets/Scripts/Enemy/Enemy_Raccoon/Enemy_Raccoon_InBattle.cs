using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class Enemy_Raccoon_InBattle : MonoBehaviour
{
    private bool canAct;

    [SerializeField] private Animator animator;

    [Space(10.0f), Header("ActCost")]
    [SerializeField] private float attackCost;
    [SerializeField] private float healCost;

    [Space(10.0f), Header("ActWeight")]
    [SerializeField] private int attackChance;
    [SerializeField] private int healChance;
    private List<int> actChances;
    private int totalChances;

    [Space(10.0f), Header("ActAmount")]
    [SerializeField] private float attackAmount;
    [SerializeField, Range(0.0f, 1.0f)] private float healRatio;

    [Space(10.0f), Header("ActCool")]
    [SerializeField] private float healCool;

    private bool canHeal;

    public void Start()
    {
        //SoundManager.Instance().ChangeToBattleBackgroundAudio();
        SoundManager.Instance().SetChangeBackgroundAudio(false);

        canAct = false;

        canHeal = true;
        actChances = new List<int>();
        InitChances();

        BattleManager.OnBattleWin -= MakeCantAct;
        BattleManager.OnBattleWin += MakeCantAct;

        BattleManager.OnBattleLose -= MakeCantAct;
        BattleManager.OnBattleLose += MakeCantAct;

        BattleManager.OnPauseBattle -= MakeCantAct;
        BattleManager.OnPauseBattle += MakeCantAct;

        BattleManager.OnStartBattle -= OnStartBattle;
        BattleManager.OnStartBattle += OnStartBattle;

        UnityEngine.Random.InitState((int)(DateTime.Now.Ticks * 1000));

        StartCoroutine("Act", 1.0f);
    }

    private void OnDestroy()
    {
        StopCoroutine("Act");

        BattleManager.OnBattleWin -= MakeCantAct;
        BattleManager.OnBattleLose -= MakeCantAct;
        BattleManager.OnPauseBattle -= MakeCantAct;
        BattleManager.OnStartBattle -= OnStartBattle;
    }

    private void InitChances()
    {
        actChances.Add(attackChance);
        actChances.Add(healChance);
        foreach (int chance in actChances)
        {
            totalChances += chance;
        }
    }

    private void MakeCanAct()
    {
        canAct = true;
    }

    private void MakeCantAct()
    {
        canAct = false;
    }

    private void OnStartBattle(bool isFirst)
    {
        MakeCanAct();
    }

    //private void Update()
    //{

    //}

    IEnumerator Act(float coolSeconds = 1.0f)
    {
        float coolSec = coolSeconds;

        while (true)
        {
            yield return new WaitForSeconds(coolSec);

            if (!canAct)
                continue;

            //Debug.Log("Do Something");
            //animator.SetBool("isAct", true);

            SelectAct();

            coolSec = UnityEngine.Random.Range(1.0f, 1.5f) * coolSeconds;
        }
    }

    private void SelectAct()
    {
        int randVal = (int)UnityEngine.Random.Range(0, totalChances);
        //Debug.Log(string.Format("randVal : {0}", randVal));

        int selectIndex = 0;
        for (; selectIndex < actChances.Count; selectIndex++)
        {
            if (randVal >= actChances[selectIndex])
            {
                randVal -= actChances[selectIndex];
            }
            else
                break;
        }

        switch (selectIndex)
        {
            case 0:
                AttackToPlayer();
                break;
            case 1:
                HealSelf();
                break;
        }
    }

    private void AttackToPlayer()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < attackCost)
            return;

        animator.SetBool("isAct", true);

        SoundManager.PlayEffectAudio.Invoke(SoundManager.AudioType.hit, false);

        //Debug.Log("Attack");
        BattleManager.Instance().ReduceEnemyCost(attackCost);
        BattleManager.Instance().DamageToPlayer(attackAmount);
    }

    private void HealSelf()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (!canHeal || currentEnemyCost < healCost)
            return;

        float currHP = BattleManager.Instance().currentEnemyHP;
        float maxHP = BattleManager.Instance().currentEnemyMaxHP;
        float healHP = maxHP * healRatio;

        if (currHP >= maxHP)
        {
            AttackToPlayer();
            return;
        }

        animator.SetBool("isAct", true);

        SoundManager.PlayEffectAudio.Invoke(SoundManager.AudioType.heal, false);

        //Debug.Log("Heal");

        BattleManager.Instance().HealToEnemy(healHP);
        BattleManager.Instance().ReduceEnemyCost(healCost);

        StartCoroutine("HealCool");
    }

    IEnumerator HealCool()
    {
        //int rep = 0;
        //while(true)
        //{
        //    if (rep++ > 10000)
        //    {
        //        Debug.Log("Many Loop");
        //        break;
        //    }
        //}

        canHeal = false;

        yield return new WaitForSeconds(healCool);

        canHeal = true;
    }
}

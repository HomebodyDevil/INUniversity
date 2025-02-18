using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Park_InBattle : MonoBehaviour
{
    private bool canAct;

    [Space(10.0f), Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Enemy_Park_InBattle_Bahavior_Control behaviorControl;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator effectAnimator;

    [Space(10.0f), Header("ActCost")]
    [SerializeField] private float attackHPCost;
    [SerializeField] private float attackMPCost;
    [SerializeField] private float healCost;

    [Space(10.0f), Header("ActWeight")]
    [SerializeField] private int attackHPChance;
    [SerializeField] private int attackMPChance;
    [SerializeField] private int healChance;
    private List<int> actChances;
    private int totalChances;

    [Space(10.0f), Header("ActAmount")]
    [SerializeField, Range(0.0f, 1.0f)] private float attackHPRatio;
    [SerializeField, Range(0.0f, 1.0f)] private float attackHPMinRatio;
    [SerializeField, Range(0.0f, 1.0f)] private float attackMPRatio;
    [SerializeField, Range(0.0f, 1.0f)] private float healRatio;

    [Space(10.0f), Header("ActCool")]
    [SerializeField] private float normalCool;
    [SerializeField] private float healCool;
    [SerializeField] private float attackHPCool;
    [SerializeField] private float attackMPCool;

    private bool canHeal;
    private bool canAttackHP;
    private bool canAttackMP;

    public void Start()
    {
        canAct = false;
        canHeal = true;
        canAttackHP = true;
        canAttackMP = true;


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

        BattleManager.OnEnemyHPisZero -= Dead;
        BattleManager.OnEnemyHPisZero += Dead;

        BattleManager.checkDeathImediate = false;

        UnityEngine.Random.InitState((int)(DateTime.Now.Ticks * 1000));

        StartCoroutine("Act", normalCool);
    }

    private void OnDestroy()
    {
        BattleManager.checkDeathImediate = true;

        //StopCoroutines();

        BattleManager.OnBattleWin -= MakeCantAct;
        BattleManager.OnBattleLose -= MakeCantAct;
        BattleManager.OnPauseBattle -= MakeCantAct;
        BattleManager.OnStartBattle -= OnStartBattle;
        BattleManager.OnEnemyHPisZero -= Dead;
    }

    private void StopCoroutines()
    {
        StopCoroutine("Act");
        StopCoroutine("HealCool");
    }

    private void Dead()
    {
        canAct = false;
        StopCoroutines();

        animator.SetBool("isDead", true);
    }

    private void InitChances()
    {
        actChances.Add(attackHPChance);
        actChances.Add(attackMPChance);
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
        Debug.Log("MakeCantAct");

        canAct = false;
    }

    private void OnStartBattle(bool isFirst)
    {
        MakeCanAct();
    }

    IEnumerator Act(float coolSeconds = 1.0f)
    {
        float coolSec = coolSeconds;

        int loopCount = 0;
        while (true)
        {
            yield return new WaitForSecondsRealtime(coolSec);

            if (loopCount++ > 9999)
            {
                Debug.Log("Many Loop");
                break;
            }

            if (!canAct)
                continue;

            SelectAct();

            coolSec = UnityEngine.Random.Range(0.8f, 1.5f) * coolSeconds;
        }
    }

    private void SelectAct()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        float currentEnemyMaxCost = BattleManager.Instance().currentEnemyMaxCost;

        int randVal = (int)UnityEngine.Random.Range(0, totalChances);

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

        if (BattleManager.Instance().currentEnemyHP / BattleManager.Instance().currentEnemyMaxHP < 0.5f &&
                canHeal)
            selectIndex = 2;

        switch (selectIndex)
        {
            case 0:
                AttackPlayerHP();
                break;
            case 1:
                AttackPlayerMP();
                break;
            case 2:
                HealSelf();
                break;
        }
    }

    public void AttackPlayerHP()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < attackHPCost || !canAttackHP)
            return;

        float currentPlayerHP = PlayerSpecManager.Instance().currentPlayerHP;
        float maxPlayerHP = PlayerSpecManager.Instance().maxPlayerHP;

        float randVal = UnityEngine.Random.Range(-0.2f, 0.2f);
        randVal = Mathf.Clamp(attackHPRatio + randVal, 0.0f, 0.8f);

        float attackAmount = currentPlayerHP * (randVal);
        if (currentPlayerHP / maxPlayerHP < attackHPMinRatio)
        {
            attackAmount = maxPlayerHP;
        }

        behaviorControl.SetAttackAmount(attackAmount);

        behaviorControl.EndAttackBool();
        animator.SetBool("Attack1", true);
        behaviorControl.SetBehaviorIndex(0);

        BattleManager.Instance().ReduceEnemyCost(attackHPCost);

        StartCoroutine("AttackHPCool");
    }

    public void AttackPlayerMP()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < attackMPCost || !canAttackMP)
            return;

        float currentPlayerCost = PlayerSpecManager.Instance().currentPlayerCost;

        float randVal = UnityEngine.Random.Range(-0.2f, 0.2f);
        randVal = Mathf.Clamp(attackMPRatio + randVal, 0.0f, 0.9f);

        float attackAmount = currentPlayerCost * (randVal);
        behaviorControl.SetAttackAmount(attackAmount);

        behaviorControl.EndAttackBool();
        animator.SetBool("Attack2", true);
        behaviorControl.SetBehaviorIndex(1);

        BattleManager.Instance().ReduceEnemyCost(attackMPCost);

        StartCoroutine("AttackMPCool");
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
            return;
        }

        BattleManager.Instance().HealToEnemy(healHP);
        BattleManager.Instance().ReduceEnemyCost(healCost);

        StartCoroutine("HealCool");
    }

    IEnumerator HealCool()
    {
        canHeal = false;

        yield return new WaitForSeconds(healCool);

        canHeal = true;
    }

    //public float GetAttackAmount()
    //{
    //    return attackAmount;
    //}

    IEnumerator AttackHPCool()
    {
        canAttackHP = false;

        yield return new WaitForSeconds(attackHPCool);

        canAttackHP = true;
    }


    IEnumerator AttackMPCool()
    {
        canAttackMP = false;

        yield return new WaitForSeconds(attackMPCool);

        canAttackMP = true;
    }
}

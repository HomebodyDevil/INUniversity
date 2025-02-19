using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lee_InBattle : MonoBehaviour
{
    private bool canAct;

    [Space(10.0f), Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Enemy_Lee_InBattle_Behavior_Control behaviorControl;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator effectAnimator;

    [Space(10.0f), Header("ActCost")]
    [SerializeField] private float healCost;
    [SerializeField] private float attackCost;
    [SerializeField] private float changeTimeCost;

    [Space(10.0f), Header("ActWeight")]
    [SerializeField] private int healChance;
    [SerializeField] private int attackChance;
    [SerializeField] private int changeTimeChance;
    private List<int> actChances;
    private int totalChances;

    [Space(10.0f), Header("ActAmount")]
    [SerializeField, Range(0.0f, 1.0f)] private float healRatio;
    [SerializeField] private float attackAmount;
    [SerializeField] private float nightStillAmount;
    [SerializeField] private float additionalAttackAmount;

    //[SerializeField] private float lunchTickAmount;       // hp 회복량(Tick)
    //[SerializeField] private float nightAmountAmount;     // cost 회복량(Tick)

    [Space(10.0f), Header("ActCool")]
    [SerializeField] private float normalCool;
    [SerializeField] private float healCool;
    [SerializeField] private float attackCool;
    [SerializeField] private float changeTimeCool;    

    private bool canHeal;
    private bool canAttack;
    private bool canChangeTime;

    private bool isLunch;    

    public void Start()
    {
        SoundManager.Instance().ChangeToBattleBackgroundAudio(SoundManager.AudioType.Lee);    
        SoundManager.Instance().SetChangeBackgroundAudio(true);

        canAct = false;
        canHeal = true;
        canAttack = true;
        canChangeTime = true;

        isLunch = true;

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

        behaviorControl.SetCostStillAmount(nightStillAmount);

        UnityEngine.Random.InitState((int)(DateTime.Now.Ticks * 1000));

        StartCoroutine("ChangeTimeCool");

        StartCoroutine("Act", normalCool);
    }

    private void OnDestroy()
    {
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
        StopCoroutine("AttackCool");
        StopCoroutine("ChangeTimeCool");
    }

    IEnumerator HealCool()
    {
        canHeal = false;

        yield return new WaitForSeconds(healCool);

        canHeal = true;
    }

    IEnumerator AttackCool()
    {
        canAttack = false;

        yield return new WaitForSeconds(attackCool);

        canAttack = true;
    }

    IEnumerator ChangeTimeCool()
    {
        canChangeTime = false;

        yield return new WaitForSeconds(changeTimeCool);

        canChangeTime = true;
    }

    private void InitChances()
    {
        actChances.Add(healChance);
        actChances.Add(attackChance);
        actChances.Add(changeTimeChance);
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
            selectIndex = 0;

        Debug.Log(selectIndex);

        switch (selectIndex)
        {
            case 0:
                HealSelf();
                break;
            case 1:
                Attack();
                break;
            case 2:
                ChangeTime();
                break;
        }
    }

    private void Dead()
    {
        canAct = false;
        StopCoroutines();

        animator.SetBool("Dead", true);
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

        SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.heal, false);

        StartCoroutine("HealCool");
    }

    private void Attack()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < attackCost || !canAttack)
        {
            return;
        }

        attackAmount += additionalAttackAmount;
        nightStillAmount += additionalAttackAmount;
        nightStillAmount = Mathf.Min(nightStillAmount, 3.0f);

        behaviorControl.SetCostStillAmount(nightStillAmount);

        behaviorControl.SetAttackAmount(attackAmount);
        behaviorControl.EndAttackBool();

        animator.SetBool("Attack_Red", true);
        behaviorControl.SetBehaviorIndex(0);

        BattleManager.Instance().ReduceEnemyCost(attackCost);
        
        StartCoroutine("AttackCool");
    }

    private void ChangeTime()
    {
        float currentEnemycost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemycost < changeTimeCost || !canChangeTime)
        {
            Attack();
            return;
        }
                
        behaviorControl.SetCostStillAmount(nightStillAmount);
        behaviorControl.EndAttackBool();

        isLunch = isLunch ? false : true;

        behaviorControl.SetIsLunch(isLunch);

        if (isLunch)    // 바꾼 후의 시점
        {
            behaviorControl.SetBehaviorIndex(1);
            animator.SetBool("Attack_Yellow", true);
        }
        else
        {
            behaviorControl.SetBehaviorIndex(2);
            animator.SetBool("Attack_Black", true);
        }

        BattleManager.Instance().ReduceEnemyCost(changeTimeCost);

        StartCoroutine("ChangeTimeCool");
    }
}

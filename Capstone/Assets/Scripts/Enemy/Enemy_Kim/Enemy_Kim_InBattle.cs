using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Kim_InBattle : MonoBehaviour
{
    private bool canAct;

    [Space(10.0f), Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Enemy_Kim_InBattle_Behavior_Control behaviorControl;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator effectAnimator;

    [Space(10.0f), Header("ActCost")]
    [SerializeField] private float healCost;
    [SerializeField] private float normalAttackCost;
    [SerializeField] private float slowCostCost;

    [Space(10.0f), Header("ActWeight")]
    [SerializeField] private int healChance;
    [SerializeField] private int normalAttackChance;
    [SerializeField] private int slowCostChance;
    private List<int> actChances;
    private int totalChances;

    [Space(10.0f), Header("ActAmount")]
    [SerializeField, Range(0.0f, 1.0f)] private float healRatio;
    [SerializeField] private float normalAttackAmount;
    [SerializeField] private float poisonAmount;
    [SerializeField] private float poisonTick;
    [SerializeField] private float slowCostRatio;

    [Space(10.0f), Header("ActCool")]
    [SerializeField] private float normalCool;
    [SerializeField] private float healCool;
    [SerializeField] private float normalAttackCool;
    //[SerializeField] private float poisionCool;
    [SerializeField] private float slowCool;

    private bool canHeal;
    private bool canNormalAttack;
    private bool canSlowCost;
    private float currentPoisonDamage;
    private float originalIncreaseCost;

    private bool isTicking;

    public void Start()
    {
        canAct = false;
        canHeal = true;
        canNormalAttack = true;
        canSlowCost = true;
        isTicking = false;

        originalIncreaseCost = PlayerSpecManager.Instance().currentCostIncreaseAmount;
        currentPoisonDamage = 0.0f;

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

        BattleManager.OnBattleWin -= StopCoroutines;
        BattleManager.OnBattleWin += StopCoroutines;

        BattleManager.OnBattleLose -= StopCoroutines;
        BattleManager.OnBattleLose += StopCoroutines;

        BattleManager.OnEnemyHPisZero -= Dead;
        BattleManager.OnEnemyHPisZero += Dead;

        BattleManager.checkDeathImediate = false;

        BattleManager.OnBattleLose -= ResetPlayerCostIncrease;
        BattleManager.OnBattleLose += ResetPlayerCostIncrease;

        BattleManager.OnBattleWin -= ResetPlayerCostIncrease;
        BattleManager.OnBattleWin += ResetPlayerCostIncrease;

        StartCoroutine("Act", normalCool);
        StartCoroutine("TickDamageToPlayer");
    }

    private void OnDestroy()
    {
        BattleManager.checkDeathImediate = true;

        //StopCoroutines();

        BattleManager.OnBattleWin -= MakeCantAct;
        BattleManager.OnBattleLose -= MakeCantAct;
        BattleManager.OnPauseBattle -= MakeCantAct;
        BattleManager.OnStartBattle -= OnStartBattle;
        BattleManager.OnBattleWin -= StopCoroutines;
        BattleManager.OnBattleLose -= StopCoroutines;
        BattleManager.OnEnemyHPisZero -= Dead;
        BattleManager.OnBattleLose -= ResetPlayerCostIncrease;

        BattleManager.OnBattleLose += ResetPlayerCostIncrease;
        BattleManager.OnBattleWin -= ResetPlayerCostIncrease;
    }

    IEnumerator HealCool()
    {
        canHeal = false;

        yield return new WaitForSeconds(healCool);

        canHeal = true;
    }

    private void InitChances()
    {
        actChances.Add(healChance);
        actChances.Add(normalAttackChance);
        actChances.Add(slowCostChance);
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

            coolSec = Random.Range(0.8f, 1.5f) * coolSeconds;
        }
    }

    private void SelectAct()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        float currentEnemyMaxCost = BattleManager.Instance().currentEnemyMaxCost;

        int randVal = (int)Random.Range(0, totalChances);

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

        //Debug.Log(selectIndex);
        switch (selectIndex)
        {
            case 0:
                HealSelf();
                break;
            case 1:
                NormalAttack();
                break;
            case 2:
                SlowCost();
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

        StartCoroutine("HealCool");
    }

    private void NormalAttack()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < normalAttackCost || !canNormalAttack)
            return;

        if (!isTicking)
        {
            PlayerSprite.ChangePlayerColor(Color.green);
            isTicking = true;
        }

        currentPoisonDamage += poisonAmount;

        behaviorControl.EndAttackBool();
        behaviorControl.SetAttackAmount(normalAttackAmount);
        animator.SetBool("Attack1", true);
        behaviorControl.SetBehaviorIndex(0);

        BattleManager.Instance().ReduceEnemyCost(normalAttackCost);

        StartCoroutine("NormalAttackCool");
    }

    private void SlowCost()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < slowCostCost || !canSlowCost)
        {
            //Debug.Log(string.Format("{0}, {1}, {2}", slowCostCost, currentEnemyCost, canSlowCost));
            return;
        }

        float slowAmount = PlayerSpecManager.Instance().currentCostIncreaseAmount;
        slowAmount = slowAmount * slowCostRatio;

        behaviorControl.EndAttackBool();
        behaviorControl.SetAttackAmount(slowAmount);
        animator.SetBool("Attack2", true);
        behaviorControl.SetBehaviorIndex(1);

        BattleManager.Instance().ReduceEnemyCost(slowCostCost);

        StartCoroutine("SlowCostCool");
    }

    private void StopCoroutines()
    {
        StopCoroutine("Act");
        StopCoroutine("HealCool");
        StopCoroutine("TickDamageToPlayer");
    }

    private void ResetPlayerCostIncrease()
    {
        PlayerSpecManager.Instance().currentCostIncreaseAmount = originalIncreaseCost;
    }

    IEnumerator NormalAttackCool()
    {
        canNormalAttack = false;

        yield return new WaitForSeconds(normalAttackCool);

        canNormalAttack = true;
    }

    IEnumerator SlowCostCool()
    {
        canSlowCost = false;

        yield return new WaitForSeconds(slowCool);

        canSlowCost = true;
    }

    IEnumerator TickDamageToPlayer()
    {
        while(true)
        {
            yield return new WaitForSeconds(poisonTick);

            BattleManager.Instance().DamageToPlayer(currentPoisonDamage);
        }
    }
}
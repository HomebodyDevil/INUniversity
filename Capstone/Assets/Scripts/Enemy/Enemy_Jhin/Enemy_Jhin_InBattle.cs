using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class Enemy_Jhin_InBattle : MonoBehaviour
{
    private bool canAct;

    [Space(10.0f), Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Enemy_Jhin_InBattle_Behavior_Control behaviorControl;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator effectAnimator;

    [Space(10.0f), Header("ActCost")]
    [SerializeField] private float attackCost;
    [SerializeField] private float healCost;
    [SerializeField] private float increaseMaxHPCost;
    [SerializeField] private float increaseMaxCostCost;
    [SerializeField] private float increaseAttackAmountCost;

    [Space(10.0f), Header("ActWeight")]
    [SerializeField] private int attackChance;
    [SerializeField] private int healChance;
    [SerializeField] private int increaseMaxHPChance;
    [SerializeField] private int increaseMaxCostChance;
    [SerializeField] private int increaseAttackAmountChance;
    private List<int> actChances;
    private int totalChances;

    [Space(10.0f), Header("ActAmount")]
    [SerializeField] private float attackAmount;
    [SerializeField] private float healRatio;
    [SerializeField] private int amountOfIncreaseMaxHP;
    [SerializeField] private int amountOfIncreaseMaxCost;
    [SerializeField, Range(0.0f, 2.0f)] private float amountOfIncreaseIncreasingMaxCost;
    [SerializeField] private int amountOfIncreaseAttackAmount;

    [Space(10.0f), Header("ActCool")]
    [SerializeField] private float normalCool;
    [SerializeField] private float healCool;
    [SerializeField] private float increaseMaxHPCool;
    [SerializeField] private float increaseMaxCostCool;
    [SerializeField] private float increaseAttackAmountCool;

    private bool canHeal;
    private bool canIncreaseMaxHP;
    private bool canIncreaseMaxCost;
    private bool canIncreaseAttackAmount;

    private bool doingIncreaseMaxHPCoroutine;
    private bool doingIncreaseMaxCostCoroutine;
    private bool doingIncreaseAttackAmountCoroutine;

    public void Start()
    {
        SoundManager.Instance().ChangeToBattleBackgroundAudio(SoundManager.AudioType.Jhin);
        SoundManager.Instance().SetChangeBackgroundAudio(true);

        canAct = false;

        canHeal = true;
        canIncreaseMaxHP = true;
        canIncreaseMaxCost = true;
        canIncreaseAttackAmount = true;

        doingIncreaseMaxHPCoroutine = false;
        doingIncreaseMaxCostCoroutine = false;
        doingIncreaseAttackAmountCoroutine = false;

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

        StartCoroutine("HealCool");
        StartCoroutine("IncreaseMaxHPCool");
        StartCoroutine("IncreaseMaxCostCool");
        StartCoroutine("IncreaseAttackAmountCool");

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
        actChances.Add(attackChance);
        actChances.Add(healChance);
        actChances.Add(increaseMaxHPChance);
        actChances.Add(increaseMaxCostChance);
        actChances.Add(increaseAttackAmountChance);
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

    //private void Update()
    //{

    //}

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

            //Debug.Log("Do Something");
            //animator.SetBool("isAct", true);

            SelectAct();

            coolSec = UnityEngine.Random.Range(0.8f, 1.5f) * coolSeconds;
        }
    }

    private void SelectAct()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        float currentEnemyMaxCost = BattleManager.Instance().currentEnemyMaxCost;
        if (currentEnemyCost >= currentEnemyMaxCost)
        {
            StrongAttack();
            return;
        }

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
            case 2:
                IncreaseMaxHP();
                break;
            case 3:
                IncreaseMaxCost();
                break;
            case 4:
                IncreaseAttackAmount();
                break;
        }
    }

    private void StrongAttack()
    {
        float currentEnemyMaxCost = BattleManager.Instance().currentEnemyMaxCost;

        animator.SetBool("isAttack", true);
        behaviorControl.SetBehaviorIndex(1);

        //Debug.Log("Attack");
        BattleManager.Instance().ReduceEnemyCost(currentEnemyMaxCost);
    }

    public void SetCanAct(bool can)
    {
        canAct = can;
    }

    public void AttackToPlayer()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < attackCost)
            return;

        animator.SetBool("isAttack", true);
        behaviorControl.SetBehaviorIndex(0);

        //Debug.Log("Attack");
        BattleManager.Instance().ReduceEnemyCost(attackCost);
    }

    //public void DamageToPlayer()
    //{
    //    BattleManager.Instance().DamageToEnemy(attackCost);
    //}

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

        //animator.SetBool("isAttack", true);
        //Debug.Log("Heal");

        BattleManager.Instance().HealToEnemy(healHP);
        BattleManager.Instance().ReduceEnemyCost(healCost);

        StartCoroutine("HealCool");
    }

    private void IncreaseMaxHP()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < increaseMaxHPCost)
            return;

        if (!canIncreaseMaxHP)
        {
            AttackToPlayer();
            return;
        }

        //sprite.SetColor(Color.red);
        //sprite.color = new Color(1.0f, 0.0f, 0.0f);

        //sprite.color = Color.red;
        //sprite.gameObject.GetComponent<EnemyEffectTransform>().SetDefaultHealEffectScale();
        //effectAnimator.SetBool("Healed", true);

        BattleManager.Instance().ReduceEnemyCost(increaseMaxHPCost);

        BattleManager.Instance().currentEnemyMaxHP += amountOfIncreaseMaxHP;
        BattleManager.Instance().HealToEnemy(0.3f * BattleManager.Instance().currentEnemyMaxHP);

        //EnemyEffectTransform.EnableEnemyHealedEffect.Invoke(Color.red);
        EnemyHealedEffectTransform.PlayEnemyHealedEffect.Invoke(Color.red);

        SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.heal, false);

        StartCool();
    }

    private void IncreaseMaxCost()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < increaseMaxCostCost)
        {
            return;
        }

        if (!canIncreaseMaxCost)
        {
            return;
        }

        //sprite.SetColor(Color.blue);

        //sprite.color = Color.blue;
        //sprite.gameObject.GetComponent<EnemyEffectTransform>().SetDefaultHealEffectScale();
        //effectAnimator.SetBool("Healed", true);
        //EnemyEffectTransform.EnableEnemyHealedEffect.Invoke(Color.blue);

        BattleManager.Instance().ReduceEnemyCost(increaseMaxCostCool);

        EnemyHealedEffectTransform.PlayEnemyHealedEffect.Invoke(Color.blue);

        SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.heal, false);

        BattleManager.Instance().currentEnemyMaxCost += amountOfIncreaseMaxCost;
        BattleManager.Instance().currentEnemyCostIncreaseAmount += amountOfIncreaseIncreasingMaxCost;

        StartCool();
    }

    private void IncreaseAttackAmount()
    {
        float currentEnemyCost = BattleManager.Instance().currentEnemyCost;
        if (currentEnemyCost < increaseAttackAmountCost)
        {
            return;
        }

        if (!canIncreaseAttackAmount)
        {
            return;
        }

        //sprite.SetColor(Color.magenta);

        //sprite.color = Color.magenta;
        //sprite.gameObject.GetComponent<EnemyEffectTransform>().SetDefaultHealEffectScale();
        //effectAnimator.SetBool("Healed", true);
        //EnemyEffectTransform.EnableEnemyHealedEffect.Invoke(Color.magenta);

        BattleManager.Instance().ReduceEnemyCost(increaseAttackAmountCost);

        EnemyHealedEffectTransform.PlayEnemyHealedEffect.Invoke(Color.magenta);

        SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.heal, false);

        attackAmount += amountOfIncreaseAttackAmount;
        behaviorControl.SetAttackAmount(attackAmount);

        StartCool();
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

    private void StartCool()
    {
        if (!doingIncreaseMaxHPCoroutine)
        {
            StartCoroutine("IncreaseMaxHPCool");
        }

        if (!doingIncreaseMaxHPCoroutine)
        {
            StartCoroutine("IncreaseMaxHPCool");
        }

        if (!doingIncreaseMaxCostCoroutine)
        {
            StartCoroutine("IncreaseMaxCostCool");
        }

        if (!doingIncreaseAttackAmountCoroutine)
        {
            StartCoroutine("IncreaseAttackAmountCool");
        }
    }

    IEnumerator IncreaseMaxHPCool()
    {
        doingIncreaseMaxHPCoroutine = true;
        canIncreaseMaxHP = false;

        yield return new WaitForSeconds(increaseMaxHPCool);

        doingIncreaseMaxHPCoroutine = false;
        canIncreaseMaxHP = true;
    }

    IEnumerator IncreaseMaxCostCool()
    {
        doingIncreaseMaxCostCoroutine = true;
        canIncreaseMaxCost = false;

        yield return new WaitForSeconds(increaseMaxCostCool);

        doingIncreaseMaxCostCoroutine = false;
        canIncreaseMaxCost = true;
    }

    IEnumerator IncreaseAttackAmountCool()
    {
        doingIncreaseAttackAmountCoroutine = true;
        canIncreaseAttackAmount = false;

        yield return new WaitForSeconds(increaseAttackAmountCool);

        doingIncreaseAttackAmountCoroutine = false;
        canIncreaseAttackAmount = true;
    }

    public float GetAttackAmount()
    {
        return attackAmount;
    }
}

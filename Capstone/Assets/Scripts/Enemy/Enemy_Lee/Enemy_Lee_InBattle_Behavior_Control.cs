using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lee_InBattle_Behavior_Control : MonoBehaviour
{
    [SerializeField] private float nightDealTick;
    [SerializeField] private float nightDealAmount;

    private Animator animator;

    private float attackAmount;
    private int behaviorIndex;

    private bool isLunch;
    private float costStillAmount;

    private void Start()
    {
        BattleManager.OnEnemyHPisZero -= StopCoroutines;
        BattleManager.OnEnemyHPisZero += StopCoroutines;

        BattleManager.OnBattleLose -= StopCoroutines;
        BattleManager.OnBattleLose += StopCoroutines;

        isLunch = true;

        animator = GetComponent<Animator>();
        //attackAmount = parkScript.GetAttackAmount();

        behaviorIndex = -1;
    }

    private void StopCoroutines()
    {
        PlayerSprite.ChangePlayerColor(Color.white);
        StopCoroutine("TickAttack");
    }

    private void OnDestroy()
    {
        StopCoroutines();

        BattleManager.OnEnemyHPisZero -= StopCoroutines;
        BattleManager.OnBattleLose -= StopCoroutines;
    }

    public void SetBehaviorIndex(int i)
    {
        behaviorIndex = i;
    }

    public void SetAttackAmount(float amount)
    {
        attackAmount = amount;
    }

    public void SetIsLunch(bool now)
    {
        isLunch = now;
    }

    public void DoAct()
    {
        if (behaviorIndex == -1)
        {
            Debug.Log("behaviorIndex is -1");
            return;
        }
        else if (behaviorIndex == 0)
        {
            SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.leeHit, false);

            Attack();
            behaviorIndex = -1;
        }
        else if (behaviorIndex == 1)
        {
            ChangeTime(true);
            PlayerSprite.ChangePlayerColor(Color.white);
            StopCoroutine("TickAttack");
            behaviorIndex = -1;
        }
        else if (behaviorIndex == 2)
        {
            ChangeTime(false);
            StartCoroutine("TickAttack");
            behaviorIndex = -1;
        }
    }

    private void ChangeTime(bool isToLunch)
    {
        SoundManager.PlayEffectAudio.Invoke(SoundManager.AudioType.leeTimeChange, false);

        if (isToLunch)
        {
            BattleSceneLights.ChangeTimeToLunch.Invoke();
        }
        else
        {
            BattleSceneLights.ChangeTimeToNight.Invoke();
        }
    }

    private void Attack()
    {
        BattleManager.Instance().DamageToPlayer(attackAmount);

        if (isLunch)
        {
            BattleManager.Instance().HealToEnemy(attackAmount * 0.7f);
        }
        else
        {
            BattleManager.Instance().ReducePlayerCost(costStillAmount);
        }
    }

    public void SetCostStillAmount(float amount)
    {
        costStillAmount = amount;
    }

    public void EndAttackBool()
    {
        animator.SetBool("Attack_Red", false);
        animator.SetBool("Attack_Black", false);
        animator.SetBool("Attack_Yellow", false);
    }

    IEnumerator TickAttack()
    {
        PlayerSprite.ChangePlayerColor(Color.green);

        while(true)
        {
            yield return new WaitForSeconds(nightDealTick);

            SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.poisonHit, false);

            BattleManager.Instance().DamageToPlayer(nightDealAmount);
        }        
    }
}

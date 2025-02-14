using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Kim_InBattle_Behavior_Control : MonoBehaviour
{
    private Animator animator;

    private float attackAmount;
    private int behaviorIndex;

    private void Start()
    {
        animator = GetComponent<Animator>();
        //attackAmount = parkScript.GetAttackAmount();

        behaviorIndex = -1;
    }

    public void SetBehaviorIndex(int i)
    {
        behaviorIndex = i;
    }

    public void SetAttackAmount(float amount)
    {
        attackAmount = amount;
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
            NormalAttack();
            behaviorIndex = -1;
        }
        else if (behaviorIndex == 1)
        {
            SlowCost();
            behaviorIndex = -1;
        }
    }

    private void NormalAttack()
    {
        BattleManager.Instance().DamageToPlayer(attackAmount);
    }

    private void SlowCost()
    {
        float currentIncreaseAmount = PlayerSpecManager.Instance().currentCostIncreaseAmount;
        currentIncreaseAmount -= attackAmount;
        currentIncreaseAmount = Mathf.Max(currentIncreaseAmount, 0.1f);

        PlayerSpecManager.Instance().currentCostIncreaseAmount = currentIncreaseAmount;
    }

    public void EndAttackBool()
    {
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
    }
}

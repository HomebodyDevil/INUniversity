using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Park_InBattle_Bahavior_Control : MonoBehaviour
{
    Enemy_Park_InBattle parkScript;

    private Animator animator;

    private float attackAmount;
    private int behaviorIndex;

    private void Start()
    {
        parkScript = transform.parent.GetComponent<Enemy_Park_InBattle>();
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

            SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.parkHPHit, false);

            BattleManager.Instance().DamageToPlayer(attackAmount);
            behaviorIndex = -1;
        }
        else if (behaviorIndex == 1)
        {
            SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.parkCostHit, false);

            BattleManager.Instance().ReducePlayerCost(attackAmount);
            behaviorIndex = -1;
        }
    }

    public void EndAttackBool()
    {
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
    }
}

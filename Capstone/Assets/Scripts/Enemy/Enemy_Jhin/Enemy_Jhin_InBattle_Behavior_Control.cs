using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jhin_InBattle_Behavior_Control : MonoBehaviour
{
    /* Behavior Index
     *  0 : attack
     */

    Enemy_Jhin_InBattle jhinScript;

    private float attackAmount;
    private int behaviorIndex;

    private void Start()
    {
        jhinScript = transform.parent.GetComponent<Enemy_Jhin_InBattle>();
        attackAmount = jhinScript.GetAttackAmount();

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
            SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.hit, false);

            BattleManager.Instance().DamageToPlayer(attackAmount);
            behaviorIndex = -1;
        }
        else if (behaviorIndex == 1)
        {
            SoundManager.PlayHitAudio.Invoke(SoundManager.AudioType.jhinStrongHit, false);

            BattleManager.Instance().DamageToPlayer(attackAmount * 1.5f);
            behaviorIndex = -1;
        }
    }

    //private void EndDeadAnimation()
    //{
    //    //BattleManager.Instance().EnemyDead();

    //    if (BattleManager.OnEnemyDead != null)
    //        BattleManager.OnEnemyDead.Invoke();
    //    Debug.Log("Ended");
    //}

    //private void DamageToPlayer()
    //{
    //    BattleManager.Instance().DamageToPlayer(attackAmount);
    //}
}

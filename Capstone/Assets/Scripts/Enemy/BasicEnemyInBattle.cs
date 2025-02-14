using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyInBattle : MonoBehaviour
{
    private bool canAct;

    [Space(10.0f), Header("Components")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Enemy_Park_InBattle_Bahavior_Control behaviorControl;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator effectAnimator;

    [Space(10.0f), Header("ActCost")]
    [SerializeField] private float healCost;

    [Space(10.0f), Header("ActWeight")]
    [SerializeField] private int healChance;
    private List<int> actChances;
    private int totalChances;

    [Space(10.0f), Header("ActAmount")]
    [SerializeField, Range(0.0f, 1.0f)] private float healRatio;

    [Space(10.0f), Header("ActCool")]
    [SerializeField] private float normalCool;
    [SerializeField] private float healCool;

    private bool canHeal;

    public void Start()
    {
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

        BattleManager.OnEnemyHPisZero -= Dead;
        BattleManager.OnEnemyHPisZero += Dead;

        BattleManager.checkDeathImediate = false;

        StartCoroutine("Act", normalCool);
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
            selectIndex = 2;

        switch (selectIndex)
        {
            case 0:
                HealSelf();
                break;
        }
    }

    private void Dead()
    {
        canAct = false;
        StopCoroutines();

        animator.SetBool("isDead", true);
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

    private void StopCoroutines()
    {
        StopCoroutine("Act");
        StopCoroutine("HealCool");
    }
}

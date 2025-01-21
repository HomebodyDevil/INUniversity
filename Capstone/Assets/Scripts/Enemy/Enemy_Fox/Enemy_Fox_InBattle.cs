using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Fox_InBattle : MonoBehaviour
{
    private bool canAct;

    [SerializeField] private Animator animator;

    public void Start()
    {
        //Debug.Log("Fox Here~");

        canAct = false;

        BattleManager.OnBattleWin -= MakeCantAct;
        BattleManager.OnBattleWin += MakeCantAct;

        BattleManager.OnBattleLose -= MakeCantAct;
        BattleManager.OnBattleLose += MakeCantAct;

        BattleManager.OnPauseBattle -= MakeCantAct;
        BattleManager.OnPauseBattle += MakeCantAct;

        BattleManager.OnStartBattle -= OnStartBattle;
        BattleManager.OnStartBattle += OnStartBattle;

        StartCoroutine("Act", 2.0f);
    }

    private void OnDestroy()
    {
        StopCoroutine("Act");

        BattleManager.OnBattleWin -= MakeCantAct;
        BattleManager.OnBattleLose -= MakeCantAct;
        BattleManager.OnPauseBattle -= MakeCantAct;
        BattleManager.OnStartBattle -= OnStartBattle;
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
        while (true)
        {
            yield return new WaitForSeconds(coolSeconds);

            if (!canAct)
                continue;

            //Debug.Log("Do Something");

            animator.SetBool("isAct", true);
            BattleManager.Instance().DamageToPlayer(10.0f);
        }
    }
}

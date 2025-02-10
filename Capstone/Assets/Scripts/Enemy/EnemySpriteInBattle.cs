using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteInBattle : MonoBehaviour
{
    private Transform battleCamera;
    private Animator animator;

    void Start()
    {
        battleCamera = CameraManager.Instance().GetBattleCamera();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        LookCamera();
    }

    private void LookCamera()
    {
        transform.LookAt(battleCamera);
    }

    private void EndAttack()
    {
        animator.SetBool("isAttack", false);
    }

    private void EndAct()
    {
        animator.SetBool("isAct", false);
    }
}

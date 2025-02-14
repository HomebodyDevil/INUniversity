using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    //public static Action Act_LookBattleCamera;
    //public static Action Act_LookFreeLookCamera;

    public static Action<UnityEngine.Color> ChangePlayerColor;

    [SerializeField] private Transform freeLookCamera;
    [SerializeField] private Transform battleVirtualCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;

    List<Transform> cameras = new List<Transform>();
    private int cameraIndex = 0;

    Vector3 camToPlayerVec;
    float angle;

    private void Awake()
    {
        //Act_LookBattleCamera -= LookBattleCamera;
        //Act_LookBattleCamera += LookBattleCamera;

        //Act_LookFreeLookCamera -= LookFreeLookCamera;
        //Act_LookFreeLookCamera += LookFreeLookCamera;

        SceneManagerEX.OnSwitchSceneToBattle -= LookBattleCamera;
        SceneManagerEX.OnSwitchSceneToBattle += LookBattleCamera;

        SceneManagerEX.OnSwitchSceneToBattle -= AnimateBattleSprite;
        SceneManagerEX.OnSwitchSceneToBattle += AnimateBattleSprite;

        SceneManagerEX.OnSwitchSceneToMap -= LookFreeLookCamera;
        SceneManagerEX.OnSwitchSceneToMap += LookFreeLookCamera;

        SceneManagerEX.OnSwitchSceneToMap -= AnimateMapSprite;
        SceneManagerEX.OnSwitchSceneToMap += AnimateMapSprite;

        PlayerCurrentCardHolder.Act_ValidDragEnd -= DoAct;
        PlayerCurrentCardHolder.Act_ValidDragEnd += DoAct;

        ChangePlayerColor -= SetPlayerColor;
        ChangePlayerColor += SetPlayerColor;

        BattleManager.OnBattleLose -= ResetPlayerColor;
        BattleManager.OnBattleLose += ResetPlayerColor;

        BattleManager.OnBattleWin -= ResetPlayerColor;
        BattleManager.OnBattleWin += ResetPlayerColor;
    }

    private void OnDestroy()
    {
        //Act_LookBattleCamera -= LookBattleCamera;
        //Act_LookFreeLookCamera -= LookFreeLookCamera;

        SceneManagerEX.OnSwitchSceneToBattle -= LookBattleCamera;
        SceneManagerEX.OnSwitchSceneToMap -= LookFreeLookCamera;

        SceneManagerEX.OnSwitchSceneToBattle -= AnimateBattleSprite;
        SceneManagerEX.OnSwitchSceneToMap -= AnimateMapSprite;

        PlayerCurrentCardHolder.Act_ValidDragEnd -= DoAct;

        ChangePlayerColor -= SetPlayerColor;

        BattleManager.OnBattleLose -= ResetPlayerColor;
        BattleManager.OnBattleWin -= ResetPlayerColor;
    }

    private void Start()
    {
        cameras.Add(freeLookCamera);
        cameras.Add(battleVirtualCamera);
    }

    private void Update()
    {
        CalcAngleWithCamera();
        LookCamera();
    }

    private float CalcAngleWithCamera()
    {
        camToPlayerVec = transform.parent.position - cameras[cameraIndex].position;
        Vector3 movingVec = PlayerMovement.Instance().moveDirection;
        Vector2 moving = new Vector2(movingVec.x, movingVec.z);
        //Debug.Log(moving);

        //Vector2 toTarget = new Vector2((float)player.transform.position.x - (float)player.xCor, 
        //                                (float)player.gameObject.transform.position.z - (float)player.zCor);
        //animator.SetBool("isMoving", (int)toTarget.magnitude > 0);

        animator.SetBool("isMoving", PlayerMovement.Instance().isMoving);

        //animator.SetBool("isMoving", (int)moving.magnitude > 0);
        //Debug.Log(camToPlayerVec);
        //Debug.Log(transform.forward);

        //if (movingVec != Vector3.zero)
        //{
        //    Vector2 camToPlayer = new Vector2(camToPlayerVec.x, camToPlayerVec.z);
        //    camToPlayer = camToPlayer.normalized;

        //    //angle = Vector2.SignedAngle(camToPlayer, moving);
        //    angle = 360 - Quaternion.FromToRotation(camToPlayer, moving.normalized).eulerAngles.z;   // 두 벡터 사이의 각을 0~360도 범위로 나타냄.
        //    //Debug.Log(angle);            

        //    animator.SetFloat("angle", angle / 360);
        //}

        Vector2 camToPlayer = new Vector2(camToPlayerVec.x, camToPlayerVec.z);
        camToPlayer = camToPlayer.normalized;

        //angle = Vector2.SignedAngle(camToPlayer, moving);
        angle = 360 - Quaternion.FromToRotation(camToPlayer, moving.normalized).eulerAngles.z;   // 두 벡터 사이의 각을 0~360도 범위로 나타냄.
                                                                                                 //Debug.Log(angle);            

        animator.SetFloat("angle", angle / 360);

        return 0;
    }

    private void LookCamera()
    {
        transform.forward = camToPlayerVec;
        //transform.LookAt(cameras[cameraIndex]);
    }

    private void LookBattleCamera()
    {
        cameraIndex = 1;
    }

    private void LookFreeLookCamera()
    {
        cameraIndex = 0;
    }

    private void AnimateBattleSprite()
    {
        animator.SetBool("isBattle", true);
    }

    private void AnimateMapSprite()
    {
        animator.SetBool("isBattle", false);
    }

    private void DoAct()
    {
        animator.SetBool("isAct", true);
    }

    private void ResetAct()
    {
        animator.SetBool("isAct", false);
    }

    private void SetPlayerColor(UnityEngine.Color color)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = color;
    }

    private void ResetPlayerColor()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = UnityEngine.Color.white;
    }
}

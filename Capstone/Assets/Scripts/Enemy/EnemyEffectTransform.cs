using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectTransform : MonoBehaviour
{
    public static Action EnableEnemyHittedEffect;
    public static Action EnableEnemyHealedEffect;

    //[SerializeField] private Transform freeLookCamera;
    //[SerializeField] private Transform battleVirtualCamera;

    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Animator animator;
    [SerializeField] private float effectScale;
    [SerializeField] private float effectPosScale;

    //List<Transform> cameras = new List<Transform>();
    private int cameraIndex = 0;

    private Transform cameraTransform;
    private Vector3 initialPosition;

    //private void Awake()
    //{

    //}

    private void Start()
    {
        EnableEnemyHittedEffect -= OnEnableEnemyHittedEffect;
        EnableEnemyHittedEffect += OnEnableEnemyHittedEffect;

        EnableEnemyHealedEffect -= OnEnableEnemyHealedEffect;
        EnableEnemyHealedEffect += OnEnableEnemyHealedEffect;

        //SceneManagerEX.OnSwitchSceneToBattle -= LookBattleCamera;
        //SceneManagerEX.OnSwitchSceneToBattle += LookBattleCamera;

        //SceneManagerEX.OnSwitchSceneToMap -= LookFreeLookCamera;
        //SceneManagerEX.OnSwitchSceneToMap += LookFreeLookCamera;

        cameraTransform = CameraManager.Instance().GetBattleCamera();
        initialPosition = transform.localPosition;

        //cameras.Add(freeLookCamera);
        //cameras.Add(battleVirtualCamera);
    }

    private void Update()
    {
        LookCamera();
    }

    private void OnDestroy()
    {
        //Act_LookBattleCamera -= LookBattleCamera;
        //Act_LookFreeLookCamera -= LookFreeLookCamera;

        EnableEnemyHittedEffect -= OnEnableEnemyHittedEffect;
        EnableEnemyHealedEffect -= OnEnableEnemyHealedEffect;

        //SceneManagerEX.OnSwitchSceneToBattle -= LookBattleCamera;
        //SceneManagerEX.OnSwitchSceneToMap -= LookFreeLookCamera;
    }

    private void LookCamera()
    {
        transform.LookAt(cameraTransform.position);
    }

    //private void LookBattleCamera()
    //{
    //    cameraIndex = 1;
    //}

    //private void LookFreeLookCamera()
    //{
    //    cameraIndex = 0;
    //}

    public void RandomTransform()
    {
        float randX = UnityEngine.Random.Range(-0.2f, 0.2f) * effectPosScale;
        float randY = UnityEngine.Random.Range(-0.2f, 0.2f) * effectPosScale;

        float randScale = UnityEngine.Random.Range(0.7f, 1.1f) * effectScale;

        transform.localPosition = new Vector3(initialPosition.x + randX, initialPosition.y + randY, 0);
        transform.localScale = new Vector3(randScale, randScale, randScale);

        //Debug.Log(randColor);
    }

    private void RandomColor()
    {
        float randColRatio = UnityEngine.Random.Range(0.8f, 1.1f);
        Color randColor = new Color(randColRatio, randColRatio, randColRatio);
        renderer.color = randColor;
    }

    private void SetColor(float r, float g, float b, float a)
    {
        //renderer.color = new Color(r, g, b, a);
        renderer.color = Color.yellow;
    }

    public void ResetTransform()
    {
        transform.localPosition = initialPosition;
    }

    public void ResetHitted()
    {
        animator.SetBool("Hitted", false);
    }

    private void OnEnableEnemyHittedEffect()
    {
        RandomColor();
        animator.SetBool("Hitted", true);
    }

    public void ResetHealed()
    {
        animator.SetBool("Healed", false);
    }

    private void OnEnableEnemyHealedEffect()
    {
        ResetTransform();

        SetColor(0.0f, 240 / 255, 110 / 255, 1.0f);
        animator.SetBool("Healed", true);
    }
}

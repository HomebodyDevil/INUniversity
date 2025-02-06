using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerEffectTransform : MonoBehaviour
{
    public static Action EnablePlayerHittedEffect;
    public static Action EnablePlayerHealedEffect;

    [SerializeField] private Transform freeLookCamera;
    [SerializeField] private Transform battleVirtualCamera;

    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Animator animator;

    List<Transform> cameras = new List<Transform>();
    private int cameraIndex = 0;

    //private void Awake()
    //{

    //}

    private void Start()
    {
        EnablePlayerHittedEffect -= OnEnablePlayerHittedEffect;
        EnablePlayerHittedEffect += OnEnablePlayerHittedEffect;

        EnablePlayerHealedEffect -= OnEnablePlayerHealedEffect;
        EnablePlayerHealedEffect += OnEnablePlayerHealedEffect;

        SceneManagerEX.OnSwitchSceneToBattle -= LookBattleCamera;
        SceneManagerEX.OnSwitchSceneToBattle += LookBattleCamera;

        SceneManagerEX.OnSwitchSceneToMap -= LookFreeLookCamera;
        SceneManagerEX.OnSwitchSceneToMap += LookFreeLookCamera;

        cameras.Add(freeLookCamera);
        cameras.Add(battleVirtualCamera);
    }

    private void Update()
    {
        LookCamera();
    }

    private void OnDestroy()
    {
        //Act_LookBattleCamera -= LookBattleCamera;
        //Act_LookFreeLookCamera -= LookFreeLookCamera;

        EnablePlayerHittedEffect -= OnEnablePlayerHittedEffect;
        EnablePlayerHealedEffect -= OnEnablePlayerHealedEffect;

        SceneManagerEX.OnSwitchSceneToBattle -= LookBattleCamera;
        SceneManagerEX.OnSwitchSceneToMap -= LookFreeLookCamera;
    }

    private void LookCamera()
    {
        transform.LookAt(cameras[cameraIndex]);
    }

    private void LookBattleCamera()
    {
        cameraIndex = 1;
    }

    private void LookFreeLookCamera()
    {
        cameraIndex = 0;
    }

    public void RandomTransform()
    {
        float randX = UnityEngine.Random.Range(-0.2f, 0.2f);
        float randY = UnityEngine.Random.Range(-0.2f, 0.2f);

        float randScale = UnityEngine.Random.Range(0.7f, 1.1f);

        transform.localPosition = new Vector3(randX, randY, 0);
        transform.localScale = new Vector3(randScale, randScale, randScale);

        //Debug.Log(randColor);
    }

    private void RandomColor()
    {
        float randColRatio = UnityEngine.Random.Range(0.0f, 1.1f);
        Color randColor = new Color(randColRatio, randColRatio, randColRatio);
        renderer.color = randColor;
    }

    private void SetColor(float r, float g, float b, float a)
    {
        renderer.color = new Color(r, g, b, a);
    }

    public void ResetTransform()
    {
        transform.position = Vector3.zero;
    }

    public void ResetHitted()
    {
        animator.SetBool("Hitted", false);
    }

    private void OnEnablePlayerHittedEffect()
    {
        RandomColor();
        animator.SetBool("Hitted", true);
    }

    public void ResetHealed()
    {
        animator.SetBool("Healed", false);
    }

    private void OnEnablePlayerHealedEffect()
    {
        SetColor(1.0f, 240 / 255, 110 / 255, 1.0f);
        animator.SetBool("Healed", true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealedEffectTransform : MonoBehaviour
{
    public static Action<Color> PlayEnemyHealedEffect;

    [Space(10), Header("Scales")]
    [SerializeField] private float effectScale;
    [SerializeField] private float effectPosScale;

    [SerializeField] private Animator animator;
    private Transform cameraTransform;
    private Vector3 initialPosition;
    private SpriteRenderer renderer;

    private void Start()
    {
        //animator = GetComponent<Animator>();
        cameraTransform = CameraManager.Instance().GetBattleCamera();
        initialPosition = transform.localPosition;
        renderer = GetComponent<SpriteRenderer>();

        transform.localScale = new Vector3(effectScale, effectScale, effectScale);

        PlayEnemyHealedEffect -= AnimateHealedEffect;
        PlayEnemyHealedEffect += AnimateHealedEffect;
    }

    private void OnDestroy()
    {
        PlayEnemyHealedEffect -= AnimateHealedEffect;
    }

    private void Update()
    {
        LookCamera();
    }

    private void LookCamera()
    {
        transform.LookAt(cameraTransform.position);
    }

    private void AnimateHealedEffect(Color color)
    {
        SetRendererColor(color);
        animator.SetBool("Healed", true);
    }

    private void SetRendererColor(Color color)
    {
        renderer.color = color;
    }

    private void ResetEffectVar()
    {
        animator.SetBool("Healed", false);
    }
}

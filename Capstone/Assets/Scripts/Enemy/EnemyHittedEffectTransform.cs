using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHittedEffectTransform : MonoBehaviour
{
    public static Action<Color> PlayEnemyHittedEffect;

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

        PlayEnemyHittedEffect -= AnimateHealedEffect;
        PlayEnemyHittedEffect += AnimateHealedEffect;
    }

    private void OnDestroy()
    {
        PlayEnemyHittedEffect -= AnimateHealedEffect;
    }

    private void Update()
    {
        //LookCamera();
    }

    private void LookCamera()
    {
        transform.LookAt(cameraTransform.position);
    }

    private void AnimateHealedEffect(Color color)
    {
        SetRendererColor(color);
        animator.SetBool("Hitted", true);
    }

    public void RandomTransform()
    {
        float randX = UnityEngine.Random.Range(-0.2f, 0.2f) * effectPosScale;
        float randY = UnityEngine.Random.Range(-0.2f, 0.2f) * effectPosScale;

        float randScale = UnityEngine.Random.Range(0.9f, 1.2f) * effectScale;

        transform.localPosition = new Vector3(initialPosition.x + randX, initialPosition.y + randY, 0);
        transform.localScale = new Vector3(randScale, randScale, randScale);

        //Debug.Log(randColor);
    }

    private void SetRendererColor(Color color)
    {
        renderer.color = color;
    }

    private void ResetEffectVar()
    {
        animator.SetBool("Hitted", false);
    }
}

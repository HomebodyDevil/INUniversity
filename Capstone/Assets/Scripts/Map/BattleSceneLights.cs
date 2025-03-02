using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;

public class BattleSceneLights : MonoBehaviour
{
    [SerializeField] Light battleLight;
    [SerializeField] float changeTime;

    public static Action ChangeTimeToLunch;
    public static Action ChangeTimeToNight;

    private Animator animator;

    private bool canChange;
    private bool isLunch;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        ChangeTimeToLunch.Invoke();
    //    }
    //    else if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        ChangeTimeToNight.Invoke();
    //    }
    //}

    private void Start()
    {
        canChange = true;
        isLunch = true;

        animator = GetComponent<Animator>();

        ChangeTimeToLunch -= ChangeToLunch;
        ChangeTimeToLunch += ChangeToLunch;

        ChangeTimeToNight -= ChangeToNight;
        ChangeTimeToNight += ChangeToNight;
    }

    private void OnDestroy()
    {
        StopCoroutine("ChangeIntensity");

        ChangeTimeToLunch -= ChangeToLunch;
        ChangeTimeToNight -= ChangeToNight;
    }

    private void ChangeToLunch()
    {
        if (!canChange || isLunch)
            return;

        canChange = false;

        ResetConditions();
        animator.SetBool("Lunch", true);
        StartCoroutine("ChangeIntensity", false);
    }

    private void ChangeToNight()
    {
        if (!canChange || !isLunch)
            return;

        canChange = false;

        ResetConditions();
        animator.SetBool("Night", true);
        StartCoroutine("ChangeIntensity", true);
    }

    private void EndLunchAnimation()
    {
        //transform.rotation = Quaternion.Euler(-30, -190, 0);
        transform.rotation = Quaternion.Euler(15, -190, 0);
        ResetConditions();
    }

    private void EndNightAnimation()
    {
        transform.rotation = Quaternion.Euler(-160, -190, 0);
        ResetConditions();
    }

    private void ResetConditions()
    {
        animator.SetBool("Night", false);
        animator.SetBool("Lunch", false);
    }

    IEnumerator ChangeIntensity(bool isToNight)
    {
        float time = 0;

        int loop = 0;
        while(time < changeTime)
        {
            if (loop++ > 100000)
            {
                Debug.Log("Many Loop");
                break;
            }

            yield return null;

            time += Time.deltaTime;

            float ratio = time / changeTime;
            ratio = Mathf.Clamp(ratio, 0.0f, 1.0f);

            battleLight.intensity = isToNight ? 1 - ratio : ratio;
        }

        canChange = true;
        isLunch = isToNight ? false : true;
    }
}

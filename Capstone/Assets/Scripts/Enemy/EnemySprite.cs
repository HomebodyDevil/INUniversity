using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    private Transform freeLookCamera;

    //private Transform battleCamera;
    //private List<Transform> cams;

    //private int camIndex;

    private void Start()
    {
        //cams = new List<Transform>();
        //camIndex = 0;

        freeLookCamera = CameraManager.Instance().GetFreeLookCamera();
        //battleCamera = CameraManager.Instance().GetBattleCamera();

        //cams.Add(freeLookCamera);
        //cams.Add(battleCamera);
    }

    //private void OnDestroy()
    //{
    //    SceneManagerEX.OnSwitchSceneToBattle -= OnSwitchToBattleScene;
    //    SceneManagerEX.OnSwitchSceneToMap -= OnSwitchToMapScene;
    //}

    void Update()
    {
        LookCamera();
    }

    private void LookCamera()
    {
        transform.LookAt(freeLookCamera);
    }

    //public void SetCamIndex(int i)
    //{
    //    camIndex = i;
    //}
}

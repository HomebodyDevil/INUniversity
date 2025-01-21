using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Cams")]
    public static CinemachineFreeLook _freeLoockCamera;
    public static CinemachineVirtualCamera _enemyVirtualCamera;
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private CinemachineVirtualCamera enemyLookCamera;
    private Transform lookAt;

    // 카메라를 적을 바라보도록 하는 Transform.
    //[SerializeField] GameObject enemyCamera;
    //[SerializeField] Transform testPosition;

    private List<GameObject> cameraList;

    void Start()
    {
        _freeLoockCamera = freeLookCamera;
        cameraList = new List<GameObject>();

        lookAt = GameObject.FindWithTag("LookAt").transform;

        // 적을 바라보도록 만드는 시험 작업.
        //CinemachineVirtualCamera enemyCam = enemyCamera.GetComponent<CinemachineVirtualCamera>();
        //enemyCam.transform.position = testPosition.position;        
        //enemyCam.enabled = true;
        //freeLoockCamera.enabled = false;

        cameraList.Add(_freeLoockCamera.gameObject);
        cameraList.Add(enemyLookCamera.gameObject);
    }

    public void ActivateCamera()
    {
        _freeLoockCamera.gameObject.SetActive(true);
    }

    public void InActivateAllCamera()
    {
        foreach (GameObject cam in cameraList)
            cam.SetActive(false);
    }
}

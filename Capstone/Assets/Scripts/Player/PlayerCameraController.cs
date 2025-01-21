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

    // ī�޶� ���� �ٶ󺸵��� �ϴ� Transform.
    //[SerializeField] GameObject enemyCamera;
    //[SerializeField] Transform testPosition;

    private List<GameObject> cameraList;

    void Start()
    {
        _freeLoockCamera = freeLookCamera;
        cameraList = new List<GameObject>();

        lookAt = GameObject.FindWithTag("LookAt").transform;

        // ���� �ٶ󺸵��� ����� ���� �۾�.
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

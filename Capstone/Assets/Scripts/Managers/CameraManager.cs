using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    public enum CameraKind
    {
        MapCamera = 0,
        EnemyCamera = 1,
        BattleCamera = 2,
    }

    public float clickDistance;

    [Header("Camera Objects")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject enemyCamera;
    [SerializeField] private GameObject battleCamera;

    [Space(10), Header("Transform Settings")]
    public Transform playerTransformInBattle;
    public Transform enemyTransformInBattle;
    [SerializeField] private Transform battleCameraTransform;

    private List<GameObject> cameraList;
    private CinemachineVirtualCameraBase currentCamera;

    public static CameraManager Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    private void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    private void Awake()
    {
        Initialize();
    }

    void Start()
    {
        // 이건 나중에 수정해줘야 할듯.
        currentCamera = playerCamera.GetComponent<CinemachineVirtualCameraBase>();

        cameraList = new List<GameObject>();

        cameraList.Add(playerCamera);
        cameraList.Add(enemyCamera);
        cameraList.Add(battleCamera);
    }

    public void SetCamera(CameraKind cameraKind)
    {
        if (currentCamera != null)
            currentCamera.Priority = 10;

        SetCInemachineBrain(true);

        currentCamera = cameraList[(int)cameraKind].GetComponent<CinemachineVirtualCameraBase>();
        currentCamera.Priority = 20;

        switch (cameraKind)
        {
            case CameraKind.MapCamera:
                break;
            case CameraKind.BattleCamera:
                SetBattleVirtualCameraTransform();
                break;
        }

        // 첫 번째로 사용해본 방법.
        //switch(cameraKind)
        //{
        //    case CameraKind.PlayerCamera:
        //        playerCamera.GetComponent<CinemachineFreeLook>().MoveToTopOfPrioritySubqueue();
        //        break;
        //    case CameraKind.EnemyCamera:
        //        enemyCamera.GetComponent<CinemachineVirtualCamera>().MoveToTopOfPrioritySubqueue();
        //        break;
        //    case CameraKind.BattleCamera:
        //        battleCamera.GetComponent<CinemachineVirtualCamera>().MoveToTopOfPrioritySubqueue();
        //        break;
        //}

        // 2번째로 사용한 방법.
        //cameraList[(int)cameraKind].SetActive(true);

        //for (int i = (int)CameraKind.PlayerCamera; i < cameraList.Count; i++)
        //    if (i != (int)cameraKind)
        //        cameraList[i].SetActive(false);
    }

    private void SetBattleVirtualCameraTransform()
    {
        battleCamera.transform.position = battleCameraTransform.position;
        battleCamera.transform.rotation = battleCameraTransform.rotation;
    }

    public void SetCInemachineBrain(bool set)
    {
        Camera.main.GetComponent<CinemachineBrain>().enabled = set;
    }

    public Transform GetFreeLookCamera()
    {
        return playerCamera.transform;
    }

    public Transform GetBattleCamera()
    {
        return battleCamera.transform;
    }
}

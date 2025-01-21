using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera_Action : MonoBehaviour
{
    public GameObject Target;               // 카메라가 따라다닐 타겟

    public float CameraSpeed = 10.0f;       // 카메라의 속도
    public float orbitSpeed = 0.5f;         // 회전 속도
    private Vector3 targetOffset;           // 타겟과 카메라 간의 거리

    private bool isInitialized = false;     //초기화

    private Vector3 prePos;
    private bool isDragging = false;

    public UnitManage unitManager;
    public GPSsystem GPSsystem;

    void Awake()
    {
        // GPSsystem 및 UnitManage 초기화
        unitManager = GameObject.Find("UnitManager").GetComponent<UnitManage>();
        GPSsystem = GameObject.Find("Capsule").GetComponent<GPSsystem>();
    }

    void Start()
    {
        // 카메라와 타겟 간 초기 오프셋 계산
        targetOffset = transform.position - Target.transform.position;
    }


    void FixedUpdate()
    {

        // 드래그 중이 아닐 때만 카메라가 현재 오프셋을 유지하면서 타겟을 따라가도록 업데이트
        if (!isDragging && !(unitManager.isPanelActive))
        {
            Vector3 targetPosition = Target.transform.position + targetOffset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * CameraSpeed);
        }

        // 항상 타겟을 바라보도록 설정
        transform.LookAt(Target.transform.position);
    }

    private void Update()
    {
        // GPSsystem의 isCamera가 true일 때만 Start() 초기화 수행
        if (!isInitialized && GPSsystem.isCamera)
        {
            InitializeCamera();
            isInitialized = true;
        }
        // 드래그 처리
        if (isInitialized)
        {
            HandleDragging();
        }
    }

    private void InitializeCamera()
    {

        // 카메라와 타겟 간 초기 오프셋 재 계산
        targetOffset = transform.position - Target.transform.position;
        Debug.Log("MainCamera_Action Initialized");
    }

    private void HandleDragging()
    {
        if (!(unitManager.isPanelActive))
        {
            if (Input.GetMouseButtonDown(0))
            {
                prePos = Input.mousePosition;
                isDragging = true;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 direction = Input.mousePosition - prePos;

                float rotationX = direction.x * orbitSpeed;
                float rotationY = direction.y * orbitSpeed;

                // Y축 기준 회전 (수평 회전)
                transform.RotateAround(Target.transform.position, Vector3.up, rotationX);

                // X축 기준 회전 (수직 회전)
                transform.RotateAround(Target.transform.position, transform.right, -rotationY);

                // 현재 위치에서 타겟과의 오프셋 갱신
                targetOffset = transform.position - Target.transform.position;

                prePos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // 드래그 종료
                isDragging = false;
            }
        }
    }
}

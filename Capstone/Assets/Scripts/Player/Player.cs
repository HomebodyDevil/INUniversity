using System.Collections;
using System.Collections.Generic;
// using UnityEditor.VersionControl;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player instance;

    // 테스트용으로 집 주의를 목표로 해본다.
    [Header("For Testing")]
    public bool isForTesting;
    public bool isForPCTest;
    [SerializeField] public double testLat;         // 집 주위 영역의 중심 위도 / 경도
    [SerializeField] public double testLong;
    [SerializeField] public double testRad;         // 영역에 대한 반지름
    [SerializeField] public double latForTestZCor;      // 지도로 맨 왼쪽 아래의 위도 / 경도(집 테스트 기준)
    [SerializeField] public double longForTestXCor;
    [SerializeField] public double latForSchoolTestZCor;  // 지도로 맨 왼쪽 아래의 위도 / 경도(학교 테스트 기준)
    [SerializeField] public double longForSchoolTestXCor;

    [Space(10), Header("OtherSettings")]
    [SerializeField] public DefaultPlayerData playerData;

    [Space(10f), Header("Destination Info")]
    [SerializeField] public double destLat;
    [SerializeField] public double destLong;
    [SerializeField] public double destRad;

    // 지도상 맨 왼쪽 아래의 위도 / 경도
    [SerializeField] public double latForZCor;
    [SerializeField] public double longForXCor;

    [Space(10f), Header("Player Current State")]
    public bool isInDest;   // 유효한 맵? 위치?에 있는지에 대한 부울.
    public double xCor;
    public double zCor;
    public bool canMove;

    private GPSManager gps;
    private PlayerCameraController cameraController;

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
    
    public static Player Instance()
    {
        if (instance == null) return null;
        return instance;
    }

    private void Awake()
    {
        Initialize();
        cameraController = GetComponent<PlayerCameraController>();
    }

    void Start()
    {
        //Debug.Log(Vector3.Distance(transform.position, CameraManager.Instance().GetFreeLookCamera().position));

        PlayerMovement.OnMakePlayerCanMove -= MakeCanMoveTrue;
        PlayerMovement.OnMakePlayerCanMove += MakeCanMoveTrue;

        PlayerMovement.OnMakePlayerCantMove -= MakeCanMoveFalse;
        PlayerMovement.OnMakePlayerCantMove += MakeCanMoveFalse;

        gps = GPSManager.Instance();

        isInDest = false;
        canMove = true;
        xCor = zCor = 0;

        if (isForTesting)
        {
            destLat = testLat;
            destLong = testLong;
            destRad = testRad;
            latForZCor = latForTestZCor;
            longForXCor = longForTestXCor;
        }

        StartCoroutine(WaitTime(0.3f));
    }

    public DefaultPlayerData GetPlayerData()
    {
        if (playerData == null) return null;
        return playerData;
    }

    // 처음에 시작하면 순간이동하길래 약간의 딜레이를 줬음.
    // 원인을 해결하기 귀찮기 때문.
    IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public void MakeCanMoveFalse()
    {
        canMove = false;
    }

    public void MakeCanMoveTrue()
    {
        canMove = true;
    }
}

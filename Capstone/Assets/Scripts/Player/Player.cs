using System.Collections;
using System.Collections.Generic;
// using UnityEditor.VersionControl;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player instance;

    // �׽�Ʈ������ �� ���Ǹ� ��ǥ�� �غ���.
    [Header("For Testing")]
    public bool isForTesting;
    public bool isForPCTest;
    [SerializeField] public double testLat;         // �� ���� ������ �߽� ���� / �浵
    [SerializeField] public double testLong;
    [SerializeField] public double testRad;         // ������ ���� ������
    [SerializeField] public double latForTestZCor;      // ������ �� ���� �Ʒ��� ���� / �浵(�� �׽�Ʈ ����)
    [SerializeField] public double longForTestXCor;
    [SerializeField] public double latForSchoolTestZCor;  // ������ �� ���� �Ʒ��� ���� / �浵(�б� �׽�Ʈ ����)
    [SerializeField] public double longForSchoolTestXCor;

    [Space(10), Header("OtherSettings")]
    [SerializeField] public DefaultPlayerData playerData;

    [Space(10f), Header("Destination Info")]
    [SerializeField] public double destLat;
    [SerializeField] public double destLong;
    [SerializeField] public double destRad;

    // ������ �� ���� �Ʒ��� ���� / �浵
    [SerializeField] public double latForZCor;
    [SerializeField] public double longForXCor;

    [Space(10f), Header("Player Current State")]
    public bool isInDest;   // ��ȿ�� ��? ��ġ?�� �ִ����� ���� �ο�.
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

    // ó���� �����ϸ� �����̵��ϱ淡 �ణ�� �����̸� ����.
    // ������ �ذ��ϱ� ������ ����.
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

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class GPSManager : MonoBehaviour
{
    static GPSManager instance;

    [Header("Test Cordinates")]
    [SerializeField] private float homeTestLat;
    [SerializeField] private float homeTestLon;
    [SerializeField] private TextMeshProUGUI longText;
    [SerializeField] private TextMeshProUGUI latText;
    [SerializeField] private GameObject LocationPanel;
    [SerializeField] private GameObject TestTextPanel;
    [SerializeField] private TextMeshProUGUI TestText;

    [Space(10), Header("Objects")]
    [SerializeField] private GameObject compusObject;
    [SerializeField] private GameObject textObject;

    [Space(20f)]
    [Range(10, 150)] public int fontSize = 10;
    public Color color = new Color(.0f, .0f, .0f, 1.0f);
    public float width, height;
    public float latitude, longitude, altitude;
    public float exLatitude, exLongitude, exAltitude;
    string message = "-";
    private IEnumerator currentCoroutine;

    private PlayerMovement playerMovement;
    private Player player;

    private string checkText;

    #region Singleton
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

    public static GPSManager Instance()
    {
        if (instance == null) return null;
        else return instance;
    }
    #endregion

    private void Awake()
    {
        SceneManagerEX.OnSwitchSceneToBattle -= DisableTestTextPanel;
        SceneManagerEX.OnSwitchSceneToBattle += DisableTestTextPanel;

        SceneManagerEX.OnSwitchSceneToMap -= EnableTestTextPanel;
        SceneManagerEX.OnSwitchSceneToMap += EnableTestTextPanel;

        SceneManagerEX.OnSwitchSceneToBattle -= WhenSwitchSceneToBattle;
        SceneManagerEX.OnSwitchSceneToBattle += WhenSwitchSceneToBattle;

        SceneManagerEX.OnSwitchSceneToMap -= WhenSwitchSceneToMap;
        SceneManagerEX.OnSwitchSceneToMap += WhenSwitchSceneToMap;

        Initialize();
    }

    private void Start()
    {
        checkText = "Check Text";

        width = 10;
        height = 10;
        exLatitude = latitude = 0f;
        exLongitude = longitude = 0f;
        exAltitude = altitude = 0f;
        currentCoroutine = getGPSCoroutine();
        playerMovement = Player.Instance().gameObject.GetComponent<PlayerMovement>();
        player = Player.Instance().gameObject.GetComponent<Player>();

        getGPSInfo();
    }

    private void Update()
    {
        //������ ���� ���� ���Դ��� üũ.
        Player.Instance().isInDest = (GetDistance(latitude, longitude, player.destLat, player.destLong) <= player.destRad);

        if (player.isForPCTest)
        {
            player.xCor = GetDistance(0, longitude, 0, player.longForXCor);
            player.zCor = GetDistance(latitude, 0, player.latForZCor, 0);
            latitude = homeTestLat;
            longitude = homeTestLon;
        }

        longText.text = "Long\t: " + longitude;
        latText.text = "Lat\t: " + latitude;

        // Ȯ�ο� Panel ���.
        // ���� ��������.
        TestText.text = String.Format("{2}\nxCor : {0}\nzCor : {1}", player.xCor, player.zCor, checkText);

        //if (Input.location.status == LocationServiceStatus.Running)
        //{
        //    double myLat = Input.location.lastData.latitude;
        //    double myLong = Input.location.lastData.longitude;
        //    double remainDistance = GetDistance(myLat, myLong, destLat, destLong);

        //    if (remainDistance < destRad)
        //    {

        //    }
        //}
    }

    private void OnDisable()
    {
        SceneManagerEX.OnSwitchSceneToBattle -= DisableTestTextPanel;
        SceneManagerEX.OnSwitchSceneToMap -= EnableTestTextPanel;

        SceneManagerEX.OnSwitchSceneToBattle -= WhenSwitchSceneToBattle;
        SceneManagerEX.OnSwitchSceneToMap -= WhenSwitchSceneToMap;
    }

    //void OnGUI()
    //{
    //    Rect position = new Rect(width, height, Screen.width, Screen.height);

    //    string text = string.Format("latitude: {0:N5} \n longitude: {1:N5} \n altitude: {2:N5}\n {3}\n
    //    : {4}\n" +
    //                                " IsInDest: {5}",
    //                                    latitude, longitude, altitude, message, playerMovement.moveDirection, player.isInDest);

    //    GUIStyle style = new GUIStyle();

    //    style.fontSize = fontSize;
    //    style.normal.textColor = color;

    //    GUI.Label(position, text, style);
    //}

    private void DisableTestTextPanel()
    {
        textObject.SetActive(false);
        TestTextPanel.SetActive(false);
    }

    private void EnableTestTextPanel()
    {
        textObject.SetActive(true);
        TestTextPanel.SetActive(true);
    }

    private void WhenSwitchSceneToMap()
    {
        compusObject.SetActive(true);
        LocationPanel.SetActive(true);
    }

    private void WhenSwitchSceneToBattle()
    {
        compusObject.SetActive(false);
        LocationPanel.SetActive(false);
    }

    public void getGPSInfo()
    {
        StartCoroutine(getGPSCoroutine());
    }

    IEnumerator getGPSCoroutine()
    {
        // ������ ����ش޶�� ��.
        // Location ������ �㰡�� �޾ƿ����� ��.
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {            
            yield return null;
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // GPS�� ����� �� �ִ��� Ȯ��.
        if (!Input.location.isEnabledByUser)
        {
            message = "GPS is not enabled";
            yield break;
        }

        // ��ġ ���񽺸� ������.
        // ù ° ���� : ��Ȯ��(���ʹ� ��Ȯ��) - ��Ȯ���� ����
        // desiredAccuracyInMeters : ���� ��ġ�� ���� �ִ� �����Ÿ�(����)
        // �� ��° ���� : ������ �Ÿ�(?)
        // updateDistanceInMeters : ������ ���� �̵� �Ÿ�.
        Input.location.Start(1f, .7f);

        // ��ġ ������ �ʱ�ȭ ����.
        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            message = maxWait.ToString() + " wait...";
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // 10�� �ȿ� �ʱ�ȭ�� ���� ������, ��ġ ������ �̿��� �����.
        if (maxWait < 1)
        {
            message = "Timed out";
            print("Timed out");
            yield break;
        }

        // ��ġ ������ connection�� �����ϸ� �����.
        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            message = "Unabled to determine device location";
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // �� ���� ������� �ʰ� ��� �����ϵ��� while���� ���.
            // yield�� ����Ͽ� �����Ӹ��� ���� �ʵ��� ��.
            while(true)
            {
                yield return null;
                // connection�� �����ϸ�, ���� location�� ǥ���� �� �ֵ���
                // ���� ����� �޾ƿ�.
                // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.

                // message = "";

                if (latitude != Input.location.lastData.latitude ||
                    longitude != Input.location.lastData.longitude ||
                    altitude != Input.location.lastData.altitude)
                {
                    checkText = "Different Pos";

                    if (Player.Instance().canMove)
                    {
                        checkText = "Now CanMove";

                        exLatitude = latitude;
                        exLongitude = longitude;
                        exAltitude = altitude;

                        latitude = Input.location.lastData.latitude;
                        longitude = Input.location.lastData.longitude;
                        altitude = Input.location.lastData.altitude;

                        //if (player.isInDest)
                        //{
                        //    player.xCor = GetDistance(0, longitude, 0, player.longForXCor);
                        //    player.zCor = GetDistance(latitude, 0, player.latForZCor, 0);
                        //}

                        player.xCor = GetDistance(0, longitude, 0, player.longForXCor);
                        player.zCor = GetDistance(latitude, 0, player.latForZCor, 0);

                        if (longitude - exLongitude != 0 || latitude - exLatitude != 0)
                            playerMovement.moveDirection = new Vector3(longitude - exLongitude, 0, latitude - exLatitude);

                        print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
                    }
                }
            }
        }

        // Stops the location service if there is no need to query location updates continuously.
        // ��� �ѳ��� �ϱ� ������ �ּ�ó��.
        // Input.location.Stop();
    }    

    public void EndGPS()
    {
        Input.location.Stop();
    }

    // �Ϲ����� ������ �̿�.(m������ ��ȯ��)
    // ��ǥ�� �Ÿ� ��� �����̶�� ��.
    // ���� ���� / �浵�� �������� ���� / �浵�� ����.
    private double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double theta = lon1 - lon2;
        double dist = Math.Sin(lat1 * Mathf.Deg2Rad) * Math.Sin(lat2 * Mathf.Deg2Rad)
                        + Math.Cos(lat1 * Mathf.Deg2Rad) * Math.Cos(lat2 * Mathf.Deg2Rad) * Math.Cos(theta * Mathf.Deg2Rad);
        
        dist = Math.Acos(dist);
        dist = dist * Mathf.Rad2Deg;
        dist = dist * 60 * 1.1515;
        dist = dist * 1609.344;     // ���� ��ȯ.

        return dist;
    }
}
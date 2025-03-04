using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GPSManager : MonoBehaviour
{
    static GPSManager instance;
    public static bool isIn = false;
    private static bool canUpdatePanel = true;

    [Header("Test Cordinates")]
    [SerializeField] private double homeTestLat;
    [SerializeField] private double homeTestLon;
    [SerializeField] private TextMeshProUGUI longText;
    [SerializeField] private TextMeshProUGUI latText;
    [SerializeField] private GameObject LocationPanel;
    [SerializeField] private GameObject TestTextPanel;
    [SerializeField] private TextMeshProUGUI TestText;

    [Space(10), Header("Objects")]
    [SerializeField] private GameObject compusObject;
    [SerializeField] private GameObject textObject;
    [SerializeField] private Image notInRangePanel;
    [SerializeField] private float fadeTime;

    [Space(20f)]
    [Range(10, 150)] public int fontSize = 10;
    public Color color = new Color(.0f, .0f, .0f, 1.0f);
    public float width, height;
    public double latitude, longitude, altitude;
    public double exLatitude, exLongitude, exAltitude;
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

        SceneManagerEX.OnSwitchSceneToBattle -= MakeCantUpdatePanel;
        SceneManagerEX.OnSwitchSceneToBattle += MakeCantUpdatePanel;

        SceneManagerEX.OnSwitchSceneToMap -= MakeCanUpdatePanel;
        SceneManagerEX.OnSwitchSceneToMap += MakeCanUpdatePanel;

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

        notInRangePanel.color = Color.black;
        notInRangePanel.gameObject.SetActive(true);

        if (!player.isForPCTest)
            getGPSInfo();

        StartCoroutine(CheckArea());
    }
   
    private void Update()
    {
        //목적지 영역 내에 들어왔는지 체크.
        //Player.Instance().isInDest = (GetDistance(latitude, longitude, player.destLat, player.destLong) <= player.destRad);

        //if (isIn && GetDistance(latitude, longitude, player.destLat, player.destLong) >= player.destRad)
        //{
        //    StopCoroutine("Fade");
        //    StartCoroutine(Fade(true));
        //}
        //else if (!isIn && GetDistance(latitude, longitude, player.destLat, player.destLong) < player.destRad)
        //{
        //    StopCoroutine("Fade");
        //    StartCoroutine(Fade(false));
        //}
        //isIn = GetDistance(latitude, longitude, player.destLat, player.destLong) <= player.destRad;


        //if (!isIn && GetDistance(latitude, longitude, player.destLat, player.destLong) <= player.destRad)
        //{
        //    StopCoroutine("Fade");
        //    StartCoroutine(Fade(false));
        //}

        //isIn = GetDistance(latitude, longitude, player.destLat, player.destLong) <= player.destRad;
        //if (!isIn)
        //{
        //    notInRangePanel.color = Color.black;
        //    notInRangePanel.gameObject.SetActive(true);
        //}


        if (player.isForPCTest)
        {
            player.xCor = GetDistance(0, longitude, 0, player.longForXCor);
            player.zCor = GetDistance(latitude, 0, player.latForZCor, 0);
            latitude = homeTestLat;
            longitude = homeTestLon;
        }

        longText.text = "경도\t" + longitude;
        latText.text = "위도\t" + latitude;

        // 확인용 Panel 사용.
        // 추후 없애주자.
        //TestText.text = String.Format("{2}\nxCor : {0}\nzCor : {1}", player.xCor, player.zCor, checkText);
        TestText.text = String.Format(isIn ? "Now INUniversity" : "등교하세요.");

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

        SceneManagerEX.OnSwitchSceneToBattle -= MakeCantUpdatePanel;
        SceneManagerEX.OnSwitchSceneToMap -= MakeCanUpdatePanel;
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

    private void MakeCanUpdatePanel()
    {
        canUpdatePanel = true;
    }

    private void MakeCantUpdatePanel()
    {
        canUpdatePanel = false;
    }

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
        // 권한을 허용해달라는 것.
        // Location 관련한 허가를 받아오도록 함.

        int count = 0;
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {            
            yield return new WaitForSeconds(1.0f);
            Permission.RequestUserPermission(Permission.FineLocation);

            if (count++ > 5)
                break;
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            Application.Quit();

        // GPS를 사용할 수 있는지 확인.
        if (!Input.location.isEnabledByUser)
        {
            message = "GPS is not enabled";
            yield break;
        }

        // 위치 서비스를 시작함.
        // 첫 째 인자 : 정확도(미터당 정확도) - 정확도가 뭐지
        // desiredAccuracyInMeters : 현재 위치에 대한 최대 오류거리(미터)
        // 두 번째 인자 : 갱신할 거리(?)
        // updateDistanceInMeters : 갱신을 위한 이동 거리.
        Input.location.Start(1f, .7f);

        // 위치 서비스의 초기화 과정.
        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            message = maxWait.ToString() + " wait...";
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // 10초 안에 초기화가 되지 않으면, 위치 서비스의 이용을 취소함.
        if (maxWait < 1)
        {
            message = "Timed out";
            print("Timed out");
            yield break;
        }

        // 위치 서비스의 connection이 실패하면 취소함.
        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            message = "Unabled to determine device location";
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // 한 번만 실행되지 않고 계속 갱신하도록 while문을 사용.
            // yield를 사용하여 프레임마다 뻣지 않도록 함.
            while(true)
            {
                yield return null;
                // connection이 성공하면, 현재 location을 표시할 수 있도록
                // 위도 등등을 받아옴.
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
                            playerMovement.moveDirection = new Vector3((float)(longitude - exLongitude), 0, (float)(latitude - exLatitude));

                        print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
                    }
                }
            }
        }

        // Stops the location service if there is no need to query location updates continuously.
        // 계속 켜놔야 하기 때문에 주석처리.
        // Input.location.Stop();
    }    

    public void EndGPS()
    {
        Input.location.Stop();
    }

    // 하버사인 공식을 이용.(m단위로 변환됨)
    // 지표면 거리 계산 공식이라고 함.
    // 현재 위도 / 경도와 목적지의 위도 / 경도가 인자.
    private double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double theta = lon1 - lon2;
        double dist = Math.Sin(lat1 * Mathf.Deg2Rad) * Math.Sin(lat2 * Mathf.Deg2Rad)
                        + Math.Cos(lat1 * Mathf.Deg2Rad) * Math.Cos(lat2 * Mathf.Deg2Rad) * Math.Cos(theta * Mathf.Deg2Rad);
        
        dist = Math.Acos(dist);
        dist = dist * Mathf.Rad2Deg;
        dist = dist * 60 * 1.1515;
        dist = dist * 1609.344;     // 미터 변환.

        return dist;
    }

    IEnumerator CheckArea()
    {
        while(true)
        {
            yield return new WaitForSeconds(fadeTime + 0.05f);

            if (!canUpdatePanel)
                continue;

            if (isIn && GetDistance(latitude, longitude, player.destLat, player.destLong) >= player.destRad)
            {
                StopCoroutine("Fade");
                StartCoroutine(Fade(true));
            }
            else if (!isIn && GetDistance(latitude, longitude, player.destLat, player.destLong) < player.destRad)
            {
                StopCoroutine("Fade");
                StartCoroutine(Fade(false));
            }
            isIn = GetDistance(latitude, longitude, player.destLat, player.destLong) <= player.destRad;
        }
    }

    IEnumerator Fade(bool isIn)
    {
        //Debug.Log($"IsIn? {isIn}");

        float time = 0.0f;
        Color panelColor = notInRangePanel.color;

        panelColor = new Color(0f, 0f, 0f, isIn ? 0f : 1f);
        notInRangePanel.color = panelColor;
        notInRangePanel.gameObject.SetActive(true);

        while (time < fadeTime)
        {
            yield return null;

            float alpha = notInRangePanel.color.a;
            time += Time.deltaTime;

            float ratio = time / fadeTime;
            if (Mathf.Abs(time - fadeTime) < 0.01f)
                ratio = isIn ? 1.0f : 0.0f;

            float newAlpha = isIn ? Mathf.Lerp(alpha, 1.0f, ratio) : Mathf.Lerp(alpha, 0.0f, ratio);
            panelColor = new Color(0.0f, 0.0f, 0.0f, newAlpha);

            notInRangePanel.color = panelColor;
        }

        if (!isIn)
            notInRangePanel.gameObject.SetActive(false);
    }
}
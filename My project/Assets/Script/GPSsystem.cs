using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using TMPro;

public class GPSsystem : MonoBehaviour
{

    public TextMeshProUGUI gpsOut;
    public bool isUpdating = true;
    private double ini_x;
    private double ini_z;
    public bool isCamera = false;
    public Loading Loading;


    public Vector3 unityCoor; // unityCoor를 담을 변수
    private void Awake()
    {
        Loading = GameObject.Find("LoadingPanel").GetComponent<Loading>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGPS();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isUpdating)
        {
            StartCoroutine(GetLocation());
            isUpdating = !isUpdating;
        }
    }

    public void StartGPS(string permissionName = null) //GPS를 실행하는 함수
    {
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            InitializeGPS();
        }
        else //아직 위치 권한을 획득하지 못했으면
        {
            PermissionCallbacks callbacks = new(); //콜백 함수 생성 후
            callbacks.PermissionGranted += StartGPS; //현재 함수를 재귀로 들어오도록
            Permission.RequestUserPermission(Permission.FineLocation, callbacks); //권한 요청 후, 다시 GPS를 시작하도록 함수 실행
        }
    }
    private void InitializeGPS()
    {
        Loading.LoadingPan.SetActive(true); // 권한 요청 중 로딩 화면 활성화

        if (SceneDataManager.Instance.ini_x != 0 && SceneDataManager.Instance.ini_z != 0)
        {
            // 기존 GPS 데이터 로드
            ini_x = SceneDataManager.Instance.ini_x;
            ini_z = SceneDataManager.Instance.ini_z;
            GPSEncoder.SetLocalOrigin(new Vector2((float)ini_z, (float)ini_x));
        }
        else
        {
            StartCoroutine(IniLocation());
        }

        StartCoroutine(WaitiniPanel());
        StartCoroutine(WaitiniUpdate());
    }

        IEnumerator IniLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS is disabled by user.");
            gpsOut.text = "Please enable GPS in settings.";
            yield break;
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            gpsOut.text = "Timed out";
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            gpsOut.text = "Unable to determine device location";
            print("Unable to determine device location");
            yield break;
        }
        else
        {

            ini_z = Input.location.lastData.latitude;
            ini_x = Input.location.lastData.longitude;
            GPSEncoder.SetLocalOrigin(new Vector2((float)ini_z, (float)ini_x));

            //TextMesh textObject = GameObject.Find("gpsOut").GetComponent<TextMesh>();
            //textObject.text = "Initial location" + ini_z + " " + ini_x;
        }
        Input.location.Stop();

        // Stop service if there is no need to query location updates continuously
    }

    IEnumerator GetLocation()
    {

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 3;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            gpsOut.text = "Timed out";
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            gpsOut.text = "Unable to determine device location";
            print("Unable to determine device location");
            yield break;
        }
        else
        {

            unityCoor = GPSEncoder.GPSToUCS(Input.location.lastData.latitude, Input.location.lastData.longitude); //GPSEncoder의 GPSToUCS 사용
            //unityCoor = GPSEncoder.GPSToUCS((float)ini_z, (float)ini_z); //GPSEncoder의 GPSToUCS 사용
            float z = unityCoor.z;
            float x = unityCoor.x;
            gpsOut.text = "latitude: " + z + "\nlongitude: " + x;


            // Access granted and location value could be retrieved

            this.transform.position = new Vector3(x/1000, 1.0f, z/1000);

        }

        StartCoroutine(WaitCamera());

        // Stop service if there is no need to query location updates continuously
        isUpdating = !isUpdating;
        //Input.location.Stop();
    }

    IEnumerator WaitCamera()
    {
        if (SceneDataManager.Instance.ini_x != 0 && SceneDataManager.Instance.ini_z != 0)
        {
            yield return new WaitForSeconds(2.0f);
            isCamera = true;
        }
        else
        {
            yield return new WaitForSeconds(5.0f);
            isCamera = true;
        }
    }

    IEnumerator WaitiniUpdate()
    {
        if (SceneDataManager.Instance.ini_x != 0 && SceneDataManager.Instance.ini_z != 0)
        {
            isUpdating = false;
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
            isUpdating = false;
        }
    }

    IEnumerator WaitiniPanel()
    {
        if (SceneDataManager.Instance.ini_x != 0 && SceneDataManager.Instance.ini_z != 0)
        {
            yield return new WaitForSeconds(2.5f);
            Loading.LoadingPan.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(5.0f);
            Loading.LoadingPan.SetActive(false);
        }

    }
}
    
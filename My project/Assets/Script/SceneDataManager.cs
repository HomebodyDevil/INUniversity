using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneDataManager : MonoBehaviour
{
    public static SceneDataManager Instance;

    // GPS 초기값
    public double ini_x;
    public double ini_z;

    // 오브젝트 상태
    public List<Vector3> objectPositions = new List<Vector3>();

    // 선택된 오브젝트 상태
    public GameObject selectedBox;
    public Vector3 selectedBoxPos;

    //Hp
    public Slider Hp;
    public Slider Exp;

    public GameObject GetSelectedBox() { return selectedBox; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Scene 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void NextSceneWithNum(int num)
    {
        if (selectedBox != null && num == 1)
        {
            SceneManager.LoadScene(1);      // 씬 전환

            // 씬이 로드된 후 해당 박스 위치 변경
            StartCoroutine(RespawnSelectedBox());

            // 씬 전환 후 player 다시 생성
            StartCoroutine(RespawnPlayer());
        }
        else if (num == 2)
        {
            SceneManager.LoadScene(2);
        }
    }

    private IEnumerator RespawnSelectedBox()
    {
        // Scene 로드가 완료될 때까지 대기
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 1);

        // 새로운 씬에서 selectedBox의 위치 변경
        selectedBox.transform.position = new Vector3(0.5f, 0.5f, -3);
    }

    private IEnumerator RespawnPlayer()
    {
        // Scene 로드가 완료될 때까지 대기
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 1);

        // Resources 폴더에서 Prefab 로드 후 생성
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Player");
        if (prefab != null)
        {
            Instantiate(prefab, new Vector3(-0.25f, 0.2f, -8), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Prefab not found in Resources: " + prefab.name);
        }
    }

    public void RemoveBox(GameObject box)
    {
        if (box != null)
        {
            Destroy(box);   // selectedBox 파괴
            selectedBox = null;     // 참조 초기화
            Debug.Log("selectedBox destroyed and reference cleared!");
        }
        else
        {
            Debug.LogWarning("No selectedBox to destroy.");
        }
    }

}

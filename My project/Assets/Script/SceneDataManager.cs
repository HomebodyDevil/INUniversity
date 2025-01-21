using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneDataManager : MonoBehaviour
{
    public static SceneDataManager Instance;

    // GPS �ʱⰪ
    public double ini_x;
    public double ini_z;

    // ������Ʈ ����
    public List<Vector3> objectPositions = new List<Vector3>();

    // ���õ� ������Ʈ ����
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
            DontDestroyOnLoad(gameObject); // Scene ��ȯ �� �ı����� ����
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
            SceneManager.LoadScene(1);      // �� ��ȯ

            // ���� �ε�� �� �ش� �ڽ� ��ġ ����
            StartCoroutine(RespawnSelectedBox());

            // �� ��ȯ �� player �ٽ� ����
            StartCoroutine(RespawnPlayer());
        }
        else if (num == 2)
        {
            SceneManager.LoadScene(2);
        }
    }

    private IEnumerator RespawnSelectedBox()
    {
        // Scene �ε尡 �Ϸ�� ������ ���
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 1);

        // ���ο� ������ selectedBox�� ��ġ ����
        selectedBox.transform.position = new Vector3(0.5f, 0.5f, -3);
    }

    private IEnumerator RespawnPlayer()
    {
        // Scene �ε尡 �Ϸ�� ������ ���
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == 1);

        // Resources �������� Prefab �ε� �� ����
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
            Destroy(box);   // selectedBox �ı�
            selectedBox = null;     // ���� �ʱ�ȭ
            Debug.Log("selectedBox destroyed and reference cleared!");
        }
        else
        {
            Debug.LogWarning("No selectedBox to destroy.");
        }
    }

}

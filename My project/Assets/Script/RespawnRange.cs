using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RespawnRange : MonoBehaviour
{
    public GameObject rangeObject;
    public int BoxCount = 0;
    public LayerMask boxLayer; // Box의 Layer를 설정해 주세요
    GameObject instantBox;
    BoxCollider rangeCollider;

    Vector3 _willremove = Vector3.zero;

    public GameObject prefabBox;


    private void Awake()
    {
        rangeCollider = rangeObject.GetComponent<BoxCollider>();

        // Resources 폴더에서 Prefab 로드 후 생성
        prefabBox = Resources.Load<GameObject>("Prefabs/Cube");
    }

    Vector3 Return_RandomPos()
    {
        Vector3 originPosition = rangeObject.transform.position;
        float range_X = rangeCollider.bounds.size.x;
        float range_Z = rangeCollider.bounds.size.z;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Z = Random.Range((range_Z / 2) * -1, range_Z / 2);

        Vector3 randomPosition = new Vector3(range_X, 0f, range_Z);
        Vector3 respawnPosition = originPosition + randomPosition;
        return respawnPosition;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        // Box 크기와 위치 기반으로 overlap 확인
        Collider[] colliders = Physics.OverlapBox(position, prefabBox.transform.localScale / 2, Quaternion.identity, boxLayer);
        return colliders.Length > 0;
    }

    private void Start()
    {
        
        // 기존 위치가 있으면 재생성, 없으면 새로 생성
        if (SceneDataManager.Instance.objectPositions.Count > 0)
        {
            foreach (var position in SceneDataManager.Instance.objectPositions)
            {
                if(position != SceneDataManager.Instance.selectedBoxPos)
                {
                    Instantiate(prefabBox, position, Quaternion.identity);
                }
                else
                {
                    _willremove = position;
                }
            }
            SceneDataManager.Instance.objectPositions.Remove(_willremove);
            BoxCount = SceneDataManager.Instance.objectPositions.Count;
        }
        StartCoroutine(Random_Respawn_Coroutine());
    }

    IEnumerator Random_Respawn_Coroutine()
    {
        while (BoxCount < 10)
        {
            yield return new WaitForSeconds(1f);

            Vector3 randomPos;
            int attempts = 0;

            // 최대 10회 시도하여 빈 위치 찾기
            do
            {
                randomPos = Return_RandomPos();
                attempts++;
            }
            while (IsPositionOccupied(randomPos) && attempts < 10);

            // 위치가 겹치지 않으면 prefabBox 생성
            if (!IsPositionOccupied(randomPos))
            {
                instantBox = Instantiate(prefabBox, randomPos, Quaternion.identity);
                SceneDataManager.Instance.objectPositions.Add(randomPos); // 위치 저장
                BoxCount++;
            }
        }
    }
}

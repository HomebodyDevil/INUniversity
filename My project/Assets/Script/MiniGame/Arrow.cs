using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameManager GameManager;
    public GameObject[] ArrowPrefabs;
    public GameObject group;
    public AudioManager audioManager;

    //KeyValuePair는 단일 키-값 쌍을 저장하지만, Dictionary는 여러 키-값 쌍을 관리
    public Queue<KeyValuePair<string, GameObject>> arrowQueue;
    private PlayerAction action;
    private AudioSource[] audioSources;

    bool RArrow;
    bool LArrow;
    bool UArrow;
    bool DArrow;

    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        action = GameObject.Find("Player").GetComponent<PlayerAction>();
        arrowQueue = new Queue<KeyValuePair<string, GameObject>>();
        audioSources = gameObject.GetComponents<AudioSource>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        //SpawnRandomImages();
    }

    public void SpawnRandomImages(int spawncount)
    {
        if (group == null || ArrowPrefabs == null || ArrowPrefabs.Length == 0)
        {
            Debug.Log("error");
            return;
        }

        for (int i = 0; i < spawncount; i++)
        {
            int randomIndex = Random.Range(0, ArrowPrefabs.Length);
            GameObject randomArrow = Instantiate(ArrowPrefabs[randomIndex], group.transform, false);

            string arrowType = GetArrowType(randomIndex); //방향키 유형 설정
            arrowQueue.Enqueue(new KeyValuePair<string, GameObject>(arrowType, randomArrow));
        }
    }

    private void Update()
    {
        if (action.GetDown() && arrowQueue.Count > 0)
        {
            var currentArrow = arrowQueue.Peek(); // 현재 큐의 첫 번째 값 확인

            if (IsMatchingArrow(currentArrow.Key)) // 플레이어 입력과 화살표 비교
            {
                // 오디오 진행
                audioManager.PlayNote();

                audioSources[0].Play();
                var removedArrow = arrowQueue.Dequeue(); // 큐에서 제거
                Destroy(removedArrow.Value); // 오브젝트 삭제
                Debug.Log("Delete");

                if (arrowQueue.Count == 0 && audioManager.timeLimitCoroutine != null)
                {
                    audioManager.StopCoroutine(audioManager.timeLimitCoroutine); // 제한 시간 취소
                    audioManager.timeLimitCoroutine = null;
                    audioManager.TimeShow.SetActive(false);
                    audioManager.ContinueAudio();
                    GameManager.AdvanceStep(); // 다음 단계 이동
                }

            }
            else
            {
                audioSources[1].Play(); //실패했을때 효과음
                GameManager.LossHeart(); //하트 잃음
            }
        }
    }

    private bool IsMatchingArrow(string arrowType)
    {
        switch (arrowType)
        {
            case "R": return action.GetRdown();
            case "L": return action.GetLdown();
            case "U": return action.GetUdown();
            case "D": return action.GetDdown();
            default: return false;
        }
    }

    private string GetArrowType(int randomIndex)
    {
        switch (randomIndex)
        {
            case 0: return "R";
            case 1: return "L";
            case 2: return "U";
            case 3: return "D";
            default:
                Debug.Log("arrow type error");
                return string.Empty;
        }
    }
}

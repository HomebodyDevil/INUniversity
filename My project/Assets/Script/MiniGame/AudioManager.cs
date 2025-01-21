using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public GameManager GameManager;
    public AudioSource[] StageAudio;
    public Arrow Arrow;
    public GameObject TimeShow;

    public float Stoptime;
    public float incrementTime;
    public Coroutine timeLimitCoroutine;


    private bool isPaused = false;
    private AudioSource StartAudio;
    private int prev;
    private Coroutine pauseCoroutine; // 중복 코루틴 방지용
    private bool isProcessingNote = false; // 노트 처리 상태 확인



    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Arrow = GameObject.Find("Arrow Set").GetComponent<Arrow>();
        StartAudio = gameObject.GetComponent<AudioSource>();

        if (StageAudio == null || StageAudio.Length == 0)
        {
            Debug.Log("audio not find");
            return; 
        }

        StartAudio.Play();
    }

    public void StartStageWithConfig(int stage, int arrowCount, float stoptime)
    {
        if (!StageAudio[stage].isPlaying)
        {
            StageAudio[stage].Play();
        }

        Stoptime = stoptime;
        StartCoroutine(ControlAudio(arrowCount, Stoptime));
    }

    public IEnumerator ControlAudio(int arrowCount, float stopTime)
    {
        yield return new WaitForSeconds(Stoptime); // stoptime초 만큼 음악을 재생했다가

        StageAudio[GameManager.stage].Pause();
        isPaused = true;

        //방향키 입력
        Arrow.SpawnRandomImages(arrowCount);

        //제한시간
        timeLimitCoroutine = StartCoroutine(TimeLimit());
        TimeShow.SetActive(true);

    }
    
    public IEnumerator TimeLimit( )
    {
        float limitTime = 4.0f;
        float elapsedTime = 0f;
        float ShowTime;

        while (elapsedTime < limitTime)
        {
            if (Arrow.arrowQueue.Count == 0)
            {
                yield break;
            }

            elapsedTime += Time.deltaTime;
            ShowTime = (int)(limitTime - elapsedTime);
            TimeShow.GetComponent<TextMeshProUGUI>().text = ShowTime.ToString(); 
            yield return null;
        }

        if (Arrow.arrowQueue.Count > 0 )
        {
            GameManager.GameOver();
        }
    }

    public void PlayNote()
    {
        if (isPaused)
        {
            if (StageAudio[GameManager.stage].isPlaying || isProcessingNote) return; //이미 재생 중이면 무시 (빠른 입력 대처)

            isProcessingNote = true;
            StageAudio[GameManager.stage].Play();

            if (pauseCoroutine != null)
            {
                StopCoroutine(pauseCoroutine);
            }
            pauseCoroutine = StartCoroutine(PauseAfterIncrement());

        }
    }

    public void ContinueAudio()
    {
        // 큐가 비었을 때 노래를 끝까지 재생
        if (isPaused)
        {
            isPaused = false; // 상태 해제
            StageAudio[GameManager.stage].UnPause();
            Debug.Log("Queue is empty. Continuing audio to the end.");
        }
    }

    IEnumerator PauseAfterIncrement()
    {
        yield return new WaitForSeconds(incrementTime); // 재생 시간만큼 대기

        if (Arrow.arrowQueue.Count > 0)
        {
            StageAudio[GameManager.stage].Pause();
        }
        else
        {
            ContinueAudio();
        }

        isProcessingNote = false;

        Debug.Log("Audio paused after playing a note.");
    }
}

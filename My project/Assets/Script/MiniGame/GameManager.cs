using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public AudioManager AudioManager;
    public Animations Animations;
    public PlayerAction PlayerAction;
    public Arrow Arrow;

    public Button nextStageButton;

    public GameObject LoadingPan;
    public AudioSource LoadingMusic;
    public AudioSource clearMusic;
    public TextMeshProUGUI text;
    public GameObject Hearts;
    private int heart = 3;
    

    public int stage = 0;
    public int stageStep = 0; // ���� ���������� �ܰ�
    public bool isNext;

    private AudioSource audioSource;
    private List<List<(int arrowCount, float stopTime)>> stageConfig = new List<List<(int, float)>>()
{
    // �������� 0: ����, ���ߴ� �ð�
    new List<(int, float)>
    {
        (3, 5), // 1�ܰ�: 3�� ���, 5�� ����
        (3, 5.75f),  // 2�ܰ�
        (3, 5.75f), // 3�ܰ�
        (3, 5.75f) // 4�ܰ�
    },
    // �������� 1
    new List<(int, float)>
    {
        (4, 4), // 1�ܰ�
        (4, 5)  // 2�ܰ�
    },
    // �������� 2
    new List<(int, float)>
    {
        (5, 3), // 1�ܰ�
        (5, 4)  // 2�ܰ�
    }
};

    public void NextButton() { isNext = true; }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Animations = GameObject.Find("AnimationManager").GetComponent<Animations>();
        PlayerAction = GameObject.Find("Player").GetComponent<PlayerAction>();
        Arrow = GameObject.Find("Arrow Set").GetComponent<Arrow>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void StartButton()
    {
        Animations.EndAnim();
        Animations.StartGame();
        Hearts.SetActive(true);
        StartCoroutine(loading());
    }

    IEnumerator loading()
    {
        LoadingPan.SetActive(true);
        LoadingMusic.Play();

        text.text = 3.ToString();

        yield return new WaitForSeconds(0.688f);

        text.text = 2.ToString();

        yield return new WaitForSeconds(0.688f);

        text.text = 1.ToString();

        yield return new WaitForSeconds(0.688f);

        LoadingPan.SetActive(false);

        StartStage(stage, stageStep);
    }

    public void StartStage(int stageIndex, int stepIndex)
    {
        if (stageIndex > stageConfig.Count || stepIndex > stageConfig[stageIndex].Count)
        {
            return;
        }

        var config = stageConfig[stageIndex][stepIndex];
        int arrowCount = config.arrowCount;
        float stopTime = config.stopTime;

        Debug.Log("stage :" + stageIndex + " step:" +  stepIndex);
        AudioManager.StartStageWithConfig(stageIndex, arrowCount, stopTime);
    }

    public void AdvanceStep()
    {
        stageStep++;
        if (stageStep >= stageConfig[stage].Count) //���� ��������
        {
            AudioManager.StageAudio[stage].Stop(); //���� ���߱�
            stage++;
            stageStep = 0;

            if (stage >= stageConfig.Count)
            {
                Debug.Log("All stages completed!");
                return;
            }

            // ���� ���������� �̵��� �غ� �Ǿ����� �˸�
            clearMusic.Play();
            ShowNextStageButton();
            return; // ��ư�� ������ ������ ���
        }

        StartStage(stage, stageStep);
    }

    public void ShowNextStageButton()
    {
        Animations.StageClear();

        nextStageButton.onClick.AddListener(() =>
        {
            nextStageButton.gameObject.SetActive(false);
            Animations.StageClearDown();
            StartCoroutine(loading());
        });

        nextStageButton.gameObject.SetActive(true);
    }


    public void LossHeart()
    {
        if (heart == 3) Hearts.transform.GetChild(2).gameObject.SetActive(false);
        else if (heart == 2) Hearts.transform.GetChild(1).gameObject.SetActive(false);
        else if (heart == 1) Hearts.transform.GetChild(0).gameObject.SetActive(false);

        heart--;

        if (heart <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Hearts.SetActive(false);

        // ť�� �ִ� ��� ȭ��ǥ ������Ʈ ����
        foreach (var arrow in Arrow.arrowQueue)
        {
            Destroy(arrow.Value); // ȭ��ǥ ������Ʈ ����
        }

        Arrow.arrowQueue.Clear();

        Animations.EndGame();
        audioSource.Play();
        Debug.Log("gameover");
    }

    public void Restart()
    {
        SceneManager.LoadScene(2);
    }
}

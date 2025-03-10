using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using Unity.Burst.CompilerServices;

public class IntroController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image overPanel;
    [SerializeField] private Image intro;
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject Title;
    [SerializeField] private VideoPlayer BackGroundvideo;

    [SerializeField] private float textBlinkTime;
    [SerializeField] private float startTextDelayTime;
    [SerializeField] private float waitTime;
    [SerializeField] private float fadeTime;
    [SerializeField] private float changeVolumeTime;

    private bool canStart;
    private bool isText;

    private int count = 0;

    private void Start()
    {
        canStart = false;
        audioSource.volume = 1.0f;

        BackGroundvideo.loopPointReached += OnVideoEnd;

        StartCoroutine(StartIntro(waitTime));
        //StartCoroutine(DelayedActiveText(true, false, startTextDelayTime));
        //StartCoroutine(DelayedActiveText(true, true, startTextDelayTime));

        StartCoroutine(BlinkText(true));
        StartCoroutine(DelayCanStart(0.5f));
    }

    private void OnDestroy()
    {
        BackGroundvideo.loopPointReached -= OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {

        //이미지 띄우기
        Debug.Log("finish");

        //StartCoroutine(DelayedActiveText(true, false, 0.01f));
        //StartCoroutine(DelayedActiveText(true, true, 0.01f));

        //StartCoroutine(Fade(false));

        Title.SetActive(true);

        StartCoroutine(StartIntro(waitTime));
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canStart)
            StartCoroutine(GoToMapScene());
    }

    private void StartFade(bool isIn)
    {
        float alpha = isIn ? 0.0f : 1.0f;
        overPanel.color = new Color(1f, 1f, 1f, alpha);

        StartCoroutine(Fade(isIn));
    }

    IEnumerator DelayCanStart(float sec = 0.5f)
    {
        yield return new WaitForSecondsRealtime(sec);
        canStart = true;
    }

    IEnumerator StartIntro(float wait)
    {
        yield return new WaitForSeconds(wait);

        //StartCoroutine(DelayedActiveText(true, true, 0.01f));

        yield return StartCoroutine(Fade(true));

        Title.SetActive(false);

        BackGroundvideo.Play();

        StartCoroutine(Fade(false));
    }

    IEnumerator DelayedActiveText(bool toActive, bool isText, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (toActive)
        {
            introText.gameObject.SetActive(isText);
            introText.color = new Color(1f, 1f, 1f, 0f);

            //intro.gameObject.SetActive(!isText);
            //intro.color = new Color(1f, 1f, 1f, 0f);

            //if (count < 2)
            //{
            //    StartCoroutine("BlinkText", isText);

            //    count++;
            //}

        }
        else
        {
            //StopCoroutine("BlinkText");
            //introText.gameObject.SetActive(toActive);
        }

        canStart = true;
    }

    IEnumerator BlinkText(bool isText)
    {
        float amount = (float)(Time.deltaTime / textBlinkTime) ;
        bool doAdd = true;

        while(true)
        {
            float alpha;

            if (isText)
            {
                alpha = introText.color.a;
            }
            else
            {
                alpha = intro.color.a;
            }

            if (Mathf.Abs(alpha - 1.0f) < 0.01f)
                doAdd = false;
            if (alpha < 0.01f)
                doAdd = true;

            alpha = doAdd ? alpha + amount : alpha - amount;

            alpha = Mathf.Clamp(alpha, 0f, 1f);

            if (isText)
            {
                introText.color = new Color(1f, 1f, 1f, alpha);
            }
            else
            {
                intro.color = new Color(1f, 1f, 1f, alpha);
            }

            yield return null;
        }
    }


    IEnumerator Fade(bool isIn)
    {
        float target = isIn ? 0.0f : 1.0f;
        overPanel.color = new Color(0f, 0f, 0f, target);
        overPanel.gameObject.SetActive(true);

        //target = isIn ? 0.0f : 1.0f;
        float time = 0.0f;

        while(time < fadeTime)
        {
            yield return null;
            time += Time.deltaTime;

            if (Mathf.Abs(time - fadeTime) < 0.01)
                time = fadeTime;

            float ratio = time / fadeTime;
            float alpha = overPanel.color.a;
            alpha = isIn ? Mathf.Lerp(alpha, 1.0f, ratio) : Mathf.Lerp(alpha, 0.0f, ratio);

            Color overPanelColor = overPanel.color;
            overPanel.color = new Color(overPanelColor.r, overPanelColor.g, overPanelColor.b, alpha);
        }
    }

    IEnumerator VolumeDown()
    {
        float volume = audioSource.volume;
        float time = 0.0f;
        while(time < changeVolumeTime)
        {
            yield return null;

            time += Time.deltaTime;

            float ratio =  (float)(time / changeVolumeTime);
            ratio = Mathf.Clamp(ratio, 0.0f, 1.0f);

            audioSource.volume = Mathf.Lerp(audioSource.volume, 0.0f, ratio);
        }

        audioSource.gameObject.SetActive(false);
    }

    IEnumerator GoToMapScene()
    {
        canStart = false;

        yield return VolumeDown();
        yield return Fade(true);

        AsyncOperation ao = SceneManager.LoadSceneAsync("MapScene");
        ao.allowSceneActivation = false;

        while(!ao.isDone)
        {
            yield return null;

            if (ao.progress < 0.9f)
            {
                Debug.Log("Loading");
            }
            else
            {
                ao.allowSceneActivation = true;
            }
        }        
    }
}

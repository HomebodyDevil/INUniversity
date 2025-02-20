using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Burst.CompilerServices;

public class IntroController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image overPanel;
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float textBlinkTime;
    [SerializeField] private float startTextDelayTime;
    [SerializeField] private float waitTime;
    [SerializeField] private float fadeTime;
    [SerializeField] private float changeVolumeTime;

    private bool canStart;

    private void Start()
    {
        canStart = false;
        audioSource.volume = 1.0f;

        StartCoroutine(DelayedActiveText(true, startTextDelayTime));
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

    IEnumerator DelayedActiveText(bool toActive, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if (toActive)
        {
            introText.gameObject.SetActive(toActive);
            introText.color = new Color(1f, 1f, 1f, 0f);

            StartCoroutine("BlinkText");
        }
        else
        {
            StopCoroutine("BlinkText");
            introText.gameObject.SetActive(toActive);
        }

        canStart = true;
    }

    IEnumerator BlinkText()
    {
        float amount = (float)(Time.deltaTime / textBlinkTime) ;
        bool doAdd = true;

        while(true)
        {
            float alpha = introText.color.a;
            if (Mathf.Abs(alpha - 1.0f) < 0.01f)
                doAdd = false;
            if (alpha < 0.01f)
                doAdd = true;

            alpha = doAdd ? alpha + amount : alpha - amount;

            alpha = Mathf.Clamp(alpha, 0f, 1f);

            introText.color = new Color(1f, 1f, 1f, alpha);

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

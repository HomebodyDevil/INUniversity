using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class IntroPanel : MonoBehaviour
{
    static private bool canStart = true;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float fadeTime;

    private Image panelImage;
    private RectTransform rectTransform;
    private float width;
    private float height;
        
    //private bool started;
    private bool sub;

    private Color orgColor;

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (canStart)
    //    {
    //        canStart = false;
    //        StartCoroutine("FadeOut");

    //        SoundManager.Instance().ChangeToMapBackgroundAudio();
    //    }

    //    //panelImage.color = Color.red;
    //}

    void Start()
    {
        panelImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        orgColor = Color.white;
        text.color = orgColor;

        //Debug.Log(rectTransform.sizeDelta);
        
        //gameObject.SetActive(true);
        //canStart = false;
        text.gameObject.SetActive(false);
        gameObject.SetActive(canStart);
        sub = true;

        if (canStart)
        {
            StartCoroutine("WaitForDistance");
        }
    }

    float CheckPlayerCamDistance()
    {
        float distance = 0.0f;

        distance = Vector3.Distance(Player.Instance().gameObject.transform.position,
                                        CameraManager.Instance().GetFreeLookCamera().position);

        return distance;
    }

    IEnumerator BlinkText()
    {
        Color tmpColor = orgColor;

        int rep = 0;
        while (true)
        {
            if (rep++ > 10000)
            {
                Debug.Log("Many Loop");
                break;
            }

            if (sub)
            { 
                tmpColor.a = Mathf.Max(0.0f, tmpColor.a - (255 * Time.deltaTime) / 255);
                //tmpColor.a = Mathf.Max(0.0f, tmpColor.a - 0.1f);

                //sub = tmpColor.a > 0;
                if (tmpColor.a <= 0)
                    sub = false;
            }
            else
            {
                tmpColor.a = Mathf.Min(1, tmpColor.a + (255 * Time.deltaTime) / 255);
                //tmpColor.a = Mathf.Max(1.0f, tmpColor.a + 0.1f);

                //sub = tmpColor.a >= 255;
                if (tmpColor.a >= 1)
                    sub = true;
            }

            //Debug.Log(tmpColor);
            //text.color = Color.red;
            text.color = tmpColor;

            yield return null;
        }
    }

    IEnumerator WaitForDistance()
    {
        canStart = false;

        int rep = 0;
        while(true)
        {
            if (rep++ > 10000)
            {
                Debug.Log("Many Loop");
                break;
            }

            yield return new WaitForSeconds(0.1f);
            //Debug.Log("TickTock");

            if (CheckPlayerCamDistance() < 30.0f)
            {
                //text.gameObject.SetActive(true);
                text.gameObject.SetActive(false);
                //StartCoroutine("BlinkText");
                //canStart = true;

                StartCoroutine(Fade(false));

                break;
            }
        }
    }

    IEnumerator Fade(bool isIn)
    {
        float target = isIn ? 0.0f : 1.0f;
        panelImage.color = new Color(0f, 0f, 0f, target);
        //panelImage.gameObject.SetActive(true);

        //target = isIn ? 0.0f : 1.0f;
        float time = 0.0f;

        bool initData = false;
        while (time < fadeTime)
        {
            yield return null;
            time += Time.deltaTime;

            if (Mathf.Abs(time - fadeTime) < 0.01)
                time = fadeTime;

            float ratio = time / fadeTime;
            float alpha = panelImage.color.a;
            alpha = isIn ? Mathf.Lerp(alpha, 1.0f, ratio) : Mathf.Lerp(alpha, 0.0f, ratio);

            Color overPanelColor = panelImage.color;
            panelImage.color = new Color(overPanelColor.r, overPanelColor.g, overPanelColor.b, alpha);

            if (!initData && ratio > 0.5f)
            {
                initData = true;

                if (DataManager.Instance() != null)
                    DataManager.Instance().LoadData();

                if (SoundManager.OnPlayBGM != null)
                    SoundManager.OnPlayBGM.Invoke();

                MapUIManager.Instance().UpdatePlayerLevelText();
            }
        }
    }

    //IEnumerator FadeOut()
    //{
    //    StopCoroutine("BlinkText");
    //    text.gameObject.SetActive(false);

    //    int rep = 0;
    //    while(true)
    //    {
    //        if (rep++ > 100000)
    //        {
    //            Debug.Log("Too many rep");
    //            break;
    //        }

    //        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,
    //                                            Mathf.Lerp(rectTransform.sizeDelta.y, 0.0f, 0.3f));

    //        SoundManager.Instance().ChangeToMapBackgroundAudio();

    //        if ((int)rectTransform.sizeDelta.y == 0)
    //        {
    //            gameObject.SetActive(false);
    //            break;
    //        }

    //        yield return null;
    //    }
    //}
}

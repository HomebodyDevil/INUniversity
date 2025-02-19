using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class IntroPanel : MonoBehaviour, IPointerClickHandler
{
    static private bool canStart = true;

    [SerializeField] private TextMeshProUGUI text;

    private Image panelImage;
    private RectTransform rectTransform;
    private float width;
    private float height;
        
    //private bool started;
    private bool sub;

    private Color orgColor;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canStart)
        {
            canStart = false;
            StartCoroutine("FadeOut");

            SoundManager.Instance().ChangeToMapBackgroundAudio();
        }

        //panelImage.color = Color.red;
    }

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
                text.gameObject.SetActive(true);
                StartCoroutine("BlinkText");
                canStart = true;

                break;
            }
        }
    }

    IEnumerator FadeOut()
    {
        StopCoroutine("BlinkText");
        text.gameObject.SetActive(false);

        int rep = 0;
        while(true)
        {
            if (rep++ > 100000)
            {
                Debug.Log("Too many rep");
                break;
            }

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,
                                                Mathf.Lerp(rectTransform.sizeDelta.y, 0.0f, 0.3f));

            if ((int)rectTransform.sizeDelta.y == 0)
            {
                gameObject.SetActive(false);
                break;
            }

            yield return null;
        }
    }
}

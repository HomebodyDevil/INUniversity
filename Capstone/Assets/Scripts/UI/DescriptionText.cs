using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DescriptionText : MonoBehaviour
{
    [SerializeField] float movingTime;
    [SerializeField] float speed;

    private TextMeshProUGUI text;
    private Color initialColor;

    private void Start()
    {
        BattleManager.OnBattleWin -= DestroySelf;
        BattleManager.OnBattleWin += DestroySelf;

        BattleManager.OnBattleLose -= DestroySelf;
        BattleManager.OnBattleLose += DestroySelf;

        text = GetComponent<TextMeshProUGUI>();
        initialColor = text.color;

        StartCoroutine(Move());
    }

    private void OnDestroy()
    {
        BattleManager.OnBattleWin -= DestroySelf;
        BattleManager.OnBattleLose -= DestroySelf;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void SetColor(Color color)
    {
        GetComponent<TextMeshProUGUI>().color = color;
    }

    public void SetText(string txt)
    {
        GetComponent<TextMeshProUGUI>().text = txt;
    }

    public void SetHeight(float height)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.rect.width, height);
    }

    IEnumerator Move()
    {
        float time = 0;
        while(time < movingTime)
        {
            yield return null;

            time += Time.deltaTime;

            transform.position = transform.position + new Vector3(0, speed * Time.deltaTime, 0);

            float alpha = 1 - time / movingTime;
            if (alpha < 0.01f)
                alpha = 0.0f;

            text.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1 - time / movingTime);
        }

        Destroy(gameObject);
    }
}

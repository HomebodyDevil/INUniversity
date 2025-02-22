using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour
{
    public static Action<bool, bool, bool, string, bool> ShowDescription;

    [SerializeField] private GameObject descriptionTextPrefab;

    [SerializeField] private bool isPlayer;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float randomAmount;
    [SerializeField] private float textHegiht;

    private void Start()
    {
        ShowDescription -= OnShowDescription;
        ShowDescription += OnShowDescription;

        //StartCoroutine(TEST());
    }

    private void OnDestroy()
    {
        ShowDescription -= OnShowDescription;
    }

    private void OnShowDescription(bool player, bool randPos, bool isHeal, string str, bool isBuff = false)
    {
        if (isPlayer == player)
            ShowText(str, isHeal, randPos, isBuff);
    }

    public void ShowText(string str, bool isHeal, bool randomPos, bool isBuff = false)
    {
        GameObject go = Instantiate(descriptionTextPrefab);
        go.transform.SetParent(canvas.transform, false);

        DescriptionText descText = go.GetComponent<DescriptionText>();
        if (descText == null)
        {
            Debug.Log("DescriptionText is null");
            return;
        }

        descText.SetText(str);
        descText.SetHeight(textHegiht);
        if (isHeal)
        {
            descText.SetColor(Color.yellow);
            //descText.SetColor(new Color(1.0f, 1.0f, 0.0f, 1.0f));
        }
        else if (isBuff)
        {
            descText.SetColor(Color.blue);
        }
        else
        {
            descText.SetColor(Color.red);
            //descText.SetColor(new Color(1.0f, 0.0f, 0.0f, 1.0f));
        }

        UnityEngine.Random.InitState((int)((DateTime.Now.Ticks * 1000) % int.MaxValue));
        if (randomPos)
        {
            float randX = UnityEngine.Random.Range(-randomAmount, randomAmount);
            float randY = UnityEngine.Random.Range(-randomAmount, randomAmount);
            go.transform.localPosition += new Vector3(randX, randY, 0);
        }
    }

    //IEnumerator TEST()
    //{
    //    while(true)
    //    {
    //        yield return new WaitForSeconds(1.5f);

    //        ShowDescription.Invoke(true, true, , false, "ABCABC");
    //    }
    //}
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingContent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankingText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SetRankingText(string str)
    {
        rankingText.text = str;
    }

    public void SetNameText(string str)
    {
        nameText.text = str;
    }

    public void SetScoreText(string str)
    {
        scoreText.text = str;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using UnityEngine;

public class RankingPanel : MonoBehaviour
{
    [SerializeField, TextArea] private string rankingContentPath;
    [SerializeField] private Transform content;
    [SerializeField] private RankingContent playerRankingContent;

    private List<RankingContent> contents;

    private async void OnEnable()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            DataManager.Instance().ActivateCloudPanel();
            gameObject.SetActive(false);
            return;
        }

        playerRankingContent.gameObject.SetActive(false);

        contents = new List<RankingContent>();

        while (content.childCount > 0)
        {
            DestroyImmediate(content.GetChild(0).gameObject);
        }

        var addScore = await LeaderboardsService.Instance.AddPlayerScoreAsync("Ranking", PlayerSpecManager.Instance().currentPlayerLevel);
        var getScores = await LeaderboardsService.Instance.GetScoresAsync("Ranking");

        int i = 0;
        for (; i < getScores.Total; i++)
        {
            GameObject rankContentObject = Resources.Load<GameObject>(rankingContentPath);
            rankContentObject = Instantiate(rankContentObject);
            rankContentObject.transform.SetParent(content);

            RankingContent newContent = rankContentObject.GetComponent<RankingContent>();
            newContent.SetRankingText((getScores.Results[i].Rank + 1).ToString());
            //newContent.SetNameText($"(본인){getScores.Results[i].PlayerName.ToString()}");
            newContent.SetScoreText(getScores.Results[i].Score.ToString());

            if (getScores.Results[i].PlayerId == AuthenticationService.Instance.PlayerId)
            {
                playerRankingContent.gameObject.SetActive(true);

                playerRankingContent.SetRankingText((getScores.Results[i].Rank + 1).ToString());
                //playerRankingContent.SetNameText(getScores.Results[i].PlayerName.ToString());
                playerRankingContent.SetNameText($"(본인){getScores.Results[i].PlayerName.ToString()}");
                playerRankingContent.SetScoreText(getScores.Results[i].Score.ToString());

                newContent.SetNameText($"(본인){getScores.Results[i].PlayerName.ToString()}");
            }
            else
                newContent.SetNameText(getScores.Results[i].PlayerName.ToString());
        }
    }

    public void ActivatePanel()
    {
        gameObject.SetActive(true);
    }

    //private async Task AddScore()
    //{
    //    await LeaderboardsService.Instance.AddPlayerScoreAsync("Ranking", 0.0f);
    //}

    //private async Task GetScores()
    //{
    //    await LeaderboardsService.Instance.GetPlayerScoreAsync("Ranking");
    //}
}

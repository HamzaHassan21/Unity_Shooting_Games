using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class GlobalLeaderboard : MonoBehaviour
{
    int maxResults = 3;
    public LeaderboardPopup leaderboardPopup;

    public void SubmitScore(int playerScore)
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate()
                {
                    StatisticName = "MostKills",
                    Value = playerScore
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnSubmitScoreSuccess, OnSubmitScoreError);
    }

    void OnSubmitScoreSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score Submitted!");
        Debug.Log(result.ToJson());
        GetLeaderboard();
    }

    void OnSubmitScoreError(PlayFabError error)
    {
        Debug.LogError("Score Submit Failed!");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void GetLeaderboard()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            StatisticName = "MostKills",
            StartPosition = 0,
            MaxResultsCount = maxResults
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardError);
    }

    void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log("Leaderboard retrieved!");

        foreach (PlayerLeaderboardEntry item in result.Leaderboard)
        {
            Debug.Log(item.Position + " / " + item.PlayFabId + " / " + item.StatValue);
        }

        Debug.Log("Leaderboard retrieved!");
        leaderboardPopup.UpdateUI(result.Leaderboard);
    }

    void OnGetLeaderboardError(PlayFabError error)
    {
        Debug.LogError("Get Leaderboard Failed!");
        Debug.LogError(error.GenerateErrorReport());
    }
}

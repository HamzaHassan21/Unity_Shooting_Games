using UnityEngine;
using System.Collections.Generic;
using PlayFab.ClientModels;

public class LeaderboardPopup : MonoBehaviour
{
    public GameObject scoreHolder;
    public GameObject noScoreText;

    public GameObject leaderboardItem; // prefab reference

    private void OnEnable()
    {
        // This gets called automatically when the panel is enabled
        GameManager.instance.globalLeaderboard.GetLeaderboard();
    }

    public void UpdateUI(List<PlayerLeaderboardEntry> playerLeaderboardEntries)
    {
        if (playerLeaderboardEntries.Count > 0)
        {
            noScoreText.SetActive(false);
            scoreHolder.SetActive(true);

            DestroyChildren(scoreHolder.transform);

            for (int i = 0; i < playerLeaderboardEntries.Count; i++)
            {
                GameObject newLeaderboardItem =
                    Instantiate(leaderboardItem, scoreHolder.transform);

                newLeaderboardItem.GetComponent<LeaderboardItem>().SetScores(
                    i + 1,
                    playerLeaderboardEntries[i].DisplayName,
                    playerLeaderboardEntries[i].StatValue
                );
            }
        }
        else
        {
            scoreHolder.SetActive(false);
            noScoreText.SetActive(true);
        }
    }

    void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }
}

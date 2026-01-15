using UnityEngine;
using UnityEngine.UI;

public class PersonalBestPopup : MonoBehaviour
{
    public GameObject holder;
    public GameObject noScoreText;

    public Text usernameValue;
    public Text bestScoreValue;
    public Text dateValue;
    public Text totalPlayersValue;
    public Text roomNameValue;

    void OnEnable()
    {
        UpdatePersonalBestUI();
    }

    void UpdatePersonalBestUI()
    {
        PlayerData playerData = GameManager.instance.playerData;

        if (playerData.username != null)
        {
            usernameValue.text = playerData.username;
            bestScoreValue.text = playerData.bestScore.ToString();
            dateValue.text = playerData.date;
            totalPlayersValue.text = playerData.totalPlayersInTheGame.ToString();
            roomNameValue.text = playerData.roomName;

            holder.SetActive(true);
            noScoreText.SetActive(false);
        }
        else
        {
            holder.SetActive(false);
            noScoreText.SetActive(true);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    public Text orderText;
    public Text usernameText;
    public Text scoreText;

    public void SetScores(int order, string username, int score)
    {
        
        orderText.text = order + ")";
        usernameText.text = username;
        scoreText.text = score.ToString();
    }
}

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultiplayerScore : MonoBehaviourPunCallbacks
{
    public GameObject playerScorePrefab;
    public Transform panel;

    Dictionary<int, GameObject> playerScore = new Dictionary<int, GameObject>();

    void Start()
    {
        // Clear any existing children in the panel
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);

            var playerScoreObject = Instantiate(playerScorePrefab, panel);
            var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
            playerScoreObjectText.text = $"{player.NickName} Score: {player.GetScore()}";

            playerScore[player.ActorNumber] = playerScoreObject;
        }
    }


    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        var playerScoreObject = playerScore[targetPlayer.ActorNumber];
        var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
        playerScoreObjectText.text = string.Format("{0} Score: {1}", targetPlayer.NickName, targetPlayer.GetScore());
    }
}


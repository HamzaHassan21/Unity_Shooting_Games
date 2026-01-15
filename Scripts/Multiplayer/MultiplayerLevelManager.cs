using Photon.Pun;
using Photon.Pun.UtilityScripts; // gives you GetScore() and AddScore()
using Photon.Realtime;           // gives you Photon.Realtime.Player and NickName
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject gameOverPopup;
    public Text winnerText;
    public int maxKills = 3;

    Vector3[] spawnPoints = new Vector3[]
    {
        new Vector3(-5, 0, -5),
        new Vector3(5, 0, -5),
        new Vector3(-5, 0, 5),
        new Vector3(5, 0, 5)
    };

    void Start()
    {
        // Making sure the EndGameCanvas starts hidden
        if (gameOverPopup != null)
            gameOverPopup.SetActive(false);

        int index = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Length;
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[index], Quaternion.identity);
    }

    // Called whenever a player's custom properties change (including score)
    
    public override void OnPlayerPropertiesUpdate(
    Photon.Realtime.Player targetPlayer,
    ExitGames.Client.Photon.Hashtable changedProps)
    {
        int score = targetPlayer.GetScore();

        if (score <= 0) return; // ignore default syncs

        if (score == maxKills)
        {
            winnerText.text = targetPlayer.NickName + " has won the game!";
            gameOverPopup.SetActive(true);
            StorePersonalBest();
        }
    }
    public void StorePersonalBest()
    {
        Debug.Log("StorePersonalBest CALLED");

        int currentScore = PhotonNetwork.LocalPlayer.GetScore();

        PlayerData playerData = GameManager.instance.playerData;

        if (currentScore <= 0) return;

        if (currentScore > playerData.bestScore)
        {
            playerData.username = PhotonNetwork.NickName;
            playerData.bestScore = currentScore;
            playerData.date = DateTime.Now.ToString("yyyy-MM-dd");
            playerData.totalPlayersInTheGame = PhotonNetwork.CurrentRoom.PlayerCount;
            playerData.roomName = PhotonNetwork.CurrentRoom.Name;

            GameManager.instance.globalLeaderboard.SubmitScore(currentScore);
            GameManager.instance.SavePlayerData();
        }
    }

    // Called by the Leave Game button 
    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Lobby");
    }
}
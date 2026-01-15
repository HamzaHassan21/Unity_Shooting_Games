using UnityEngine;
using System.IO;
using Leguar.TotalJSON;
using PlayFab;
using PlayFab.ClientModels;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerData playerData;
    public string filePath;
    public GlobalLeaderboard globalLeaderboard;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            playerData = new PlayerData();

            if (string.IsNullOrEmpty(playerData.username))
            {
                playerData.username = "Player" + UnityEngine.Random.Range(100000, 999999);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            filePath = "PlayerData.txt";
        }

        LoadPlayerData();

        // Guranteeing use actually exists
        if (string.IsNullOrEmpty(playerData.username))
        {
            playerData.username = "Player" + UnityEngine.Random.Range(100000, 999999);
            SavePlayerData();
        }

        LoginToPlayFab();
    }

    

    public void SavePlayerData()
    {
        string serialisedDataString = JSON.Serialize(playerData).CreateString();
        File.WriteAllText(filePath, serialisedDataString);
    }

    public void LoadPlayerData()
    {
        if (!File.Exists(filePath))
        {
            playerData = new PlayerData();
            SavePlayerData();
            return;
        }

        string fileContents = File.ReadAllText(filePath);
        playerData = JSON.ParseString(fileContents).Deserialize<PlayerData>();
    }

    void LoginToPlayFab()
    {
        var request = new LoginWithCustomIDRequest
        {
            CreateAccount = true,
            CustomId = playerData.uid
        };

        PlayFabClientAPI.LoginWithCustomID(
            request,
            PlayFabLoginResult,
            PlayFabLoginError
        );
    }

    void PlayFabLoginResult(LoginResult loginResult)
    {
        Debug.Log("PlayFab login success!");
        Debug.Log(loginResult.ToJson());

        SetPlayFabDisplayNameIfNeeded(loginResult);

        // Tutorial test value
        globalLeaderboard.SubmitScore(3);
        globalLeaderboard.GetLeaderboard();
    }

    void PlayFabLoginError(PlayFabError loginError)
    {
        Debug.LogError("PlayFab login failed!");
        Debug.LogError(loginError.ErrorMessage);
    }
    void SetPlayFabDisplayNameIfNeeded(LoginResult loginResult)
    {
        // If PlayFab already has a name, this does nothing
        if (!string.IsNullOrEmpty(loginResult.InfoResultPayload?.PlayerProfile?.DisplayName))
        {
            Debug.Log("Display name already set: " +
                loginResult.InfoResultPayload.PlayerProfile.DisplayName);
            return;
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = playerData.username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(
            request,
            result => Debug.Log("DisplayName set to: " + result.DisplayName),
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }
}






using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class MultiplayerLobby : MonoBehaviourPunCallbacks
{
    public Transform LoginPanel;
    public Transform SelectionPanel;
    public Transform CreateRoomPanel;
    public Transform InsideRoomPanel;
    public Transform ListRoomsPanel;

    public InputField roomNameInput;
    public InputField playerNameInput;   // player nickname input
    private string playerName;

    public GameObject textPrefab;        // prefab for player name text
    public Transform insideRoomPlayerList; // Content object of ScrollView

    public Transform listRoomPanel;         // ListRooms panel
    public GameObject roomEntryPrefab;      // Prefab with RoomEntry script
    public Transform listRoomPanelContent;  // ScrollView Content under ListRooms

    private Dictionary<string, RoomInfo> cachedRoomList;

    public GameObject startGameButton;
    public Chat chat;


    private void Start()
    {
        Debug.Log(" Game is running");
        ActivatePanel("Login");
        playerNameInput.text = playerName = string.Format("Player {0}", Random.Range(1, 1000000));

        cachedRoomList = new Dictionary<string, RoomInfo>();

        PhotonNetwork.AutomaticallySyncScene = true;

    }

    public void ActivatePanel(string panelName)
    {
        LoginPanel.gameObject.SetActive(false);
        SelectionPanel.gameObject.SetActive(false);
        CreateRoomPanel.gameObject.SetActive(false);
        InsideRoomPanel.gameObject.SetActive(false);
        ListRoomsPanel.gameObject.SetActive(false);

        switch (panelName)
        {
            case "Login":
                LoginPanel.gameObject.SetActive(true);
                break;
            case "Selection":
                SelectionPanel.gameObject.SetActive(true);
                break;
            case "CreateRoom":
                CreateRoomPanel.gameObject.SetActive(true);
                break;
            case "InsideRoom":
                InsideRoomPanel.gameObject.SetActive(true);
                break;
            case "ListRooms":
                ListRoomsPanel.gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown panel name: " + panelName);
                break;
        }
    }

    public void LoginButtonClicked()
    {
        string playerName = playerNameInput.text.Trim();

        if (playerName != "")
        {
            // Setting nickname before connecting
            PhotonNetwork.LocalPlayer.NickName = playerName = playerNameInput.text;

        Debug.Log("Login button clicked! Attempting to connect...");
        PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Player name is invalid");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        ActivatePanel("Selection");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        ActivatePanel("ListRooms");
    }

    public void DisconnectButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from the master server!");
        ActivatePanel("Login"); // return to Login panel
    }

    public void CreateARoom()
    {
        Debug.Log("CreateARoom() called");

        if (roomNameInput == null)
        {
            Debug.LogError("roomNameInput not assigned!");
            return;
        }

        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning(" Room name is empty!");
            return;
        }

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true
        };

        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }

    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            // Checking for bad rooms: Closed, Invisible, OR Removed.
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                // If bad, remove it from the cache.
                cachedRoomList.Remove(room.Name);
            }
            else
            {
                // If good add/update it in the cache.
                // How the room gets saved for display
                cachedRoomList[room.Name] = room;
            }
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to create room: " + message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room has been joined!");

        ActivatePanel("InsideRoom");

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        // Clear old player list UI
        foreach (Transform child in insideRoomPlayerList)
            Destroy(child.gameObject);

        // Populate all current players
        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            playerListEntry.GetComponent<Text>().text = player.NickName;
            playerListEntry.name = player.NickName; // make removal easy later
        }

        var auth = new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName);

        chat.userName = PhotonNetwork.NickName;
        chat.ConnectChat(auth);

    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    // Start Game button
    public void StartGameClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("Multiplayer");
    }

    public void JoinRandomRoom()
    {
        Debug.Log("Attempting to join a random room...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(" Failed to join random room: " + message);
        Debug.Log("Creating a new room instead...");

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true
        };

        string fallbackRoomName = "Room_" + Random.Range(1000, 9999);
        PhotonNetwork.CreateRoom(fallbackRoomName, roomOptions);
    }

    public void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void LeaveLobbyClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("Leaving lobby...");
            PhotonNetwork.LeaveLobby();
        }
        else
        {
            Debug.LogWarning("Cannot leave lobby — not currently in a lobby.");
        }
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby!"); 

        DestroyChildren(listRoomPanelContent); 
        cachedRoomList.Clear(); 

        ActivatePanel("Selection"); 
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ListRoomsClicked()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room Update: " + roomList.Count);

        DestroyChildren(listRoomPanelContent);

        // Update the cache first
        UpdateCachedRoomList(roomList);

        // Iterate over the DICTIONARY (the clean list)
        foreach (var room in cachedRoomList)
        {
            var newRoomEntry = Instantiate(roomEntryPrefab, listRoomPanelContent);
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();

            // Access via .Key (the room name string)
            newRoomEntryScript.roomName = room.Key;

            // Must use room.Value to access RoomInfo properties
            newRoomEntryScript.roomText.text = string.Format("[{0} ({1}/{2})]",
                room.Key,
                room.Value.PlayerCount, // Accesses via .Value
                room.Value.MaxPlayers);  // Accessese via .Value
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left the room!");
        ActivatePanel("CreateRoom"); // returns to CreateRoom panel
        chat.Disconnect();
        DestroyChildren(insideRoomPlayerList);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("A Player joined the Room");
        var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
        playerListEntry.GetComponent<Text>().text = newPlayer.NickName;
        playerListEntry.name = newPlayer.NickName; // so we can find/destroy it later
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("A Player left the Room");
        foreach (Transform child in insideRoomPlayerList)
        {
            if (child.name == otherPlayer.NickName)
            {
                Destroy(child.gameObject);
                break; 
            }
        }
    }

    void UpdatePlayfabUsername(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(
            request,
            OnUpdateDisplayNameSuccess,
            OnUpdateDisplayNameError
        );
    }

    void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("PlayFab display name updated!");
        Debug.Log(result.ToJson());
    }

    void OnUpdateDisplayNameError(PlayFabError error)
    {
        Debug.LogError("PlayFab display name update failed!");
        Debug.LogError(error.GenerateErrorReport());
    }
}



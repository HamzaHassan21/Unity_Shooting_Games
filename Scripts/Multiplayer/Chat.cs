using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour, IChatClientListener
{
    [Header("UI")]
    public GameObject chatPanel;          // panel you show/hide
    public InputField inputField;         // message input
    public Text channelMessagesText;      // text inside scroll content
    public string userName;

    private ChatClient chatClient;

    private bool isConnected;
    private bool isSubscribed;
    private string currentChannel;

    // IMPORTANT: this object must stay active so Update() runs!
    void Awake()
    {
        // If you change scenes and still want chat, keep it:
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Hide UI at start, but DO NOT disable the object that has this script.
        if (chatPanel != null) chatPanel.SetActive(false);
        if (inputField != null) inputField.interactable = false;
    }

    void Update()
    {
        // Photon Chat needs Service() called every frame to connect + receive messages.
        if (chatClient != null)
            chatClient.Service();
    }

    // Call this from MultiplayerLobby.OnJoinedRoom()
    public void ConnectChat(Photon.Chat.AuthenticationValues auth)
    {
        isConnected = false;
        isSubscribed = false;

        // Create client
        chatClient = new ChatClient(this)
        {
            AuthValues = auth
        };

        // OPTIONAL: if you want chat region to follow PUN region
        // Only set if CloudRegion exists (e.g. "eu")
        if (!string.IsNullOrEmpty(PhotonNetwork.CloudRegion))
            chatClient.ChatRegion = PhotonNetwork.CloudRegion;

        string appIdChat = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;
        string appVersion = PhotonNetwork.AppVersion; // keep consistent with PUN

        Debug.Log("[CHAT] Connecting... AppVersion=" + appVersion + " Region=" + PhotonNetwork.CloudRegion);

        chatClient.Connect(appIdChat, appVersion, auth);
    }

    public void ActivateChatUI()
    {
        if (chatPanel != null) chatPanel.SetActive(true);
    }

    public void SendChatMessage()
    {
        if (!isConnected || !isSubscribed)
        {
            Debug.LogWarning("[CHAT] Chat not ready yet (not connected or not subscribed).");
            return;
        }

        if (inputField == null || string.IsNullOrWhiteSpace(inputField.text))
            return;

        // Send ONLY the message text. Sender name comes from Photon Chat automatically.
        chatClient.PublishMessage(currentChannel, inputField.text.Trim());

        inputField.text = "";
        inputField.ActivateInputField();
    }

    public void Disconnect()
    {
        if (chatClient != null)
            chatClient.Disconnect();
    }

    // ----------------- Photon Chat Callbacks -----------------

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("[CHAT] " + level + " - " + message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("[CHAT] State: " + state);
    }

    public void OnConnected()
    {
        Debug.Log("[CHAT] Connected!");
        isConnected = true;

        // Must be in a room to use room name as channel
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning("[CHAT] Connected but not in a PUN room yet.");
            return;
        }

        currentChannel = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("[CHAT] Subscribing to: " + currentChannel);

        chatClient.Subscribe(new string[] { currentChannel });
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("[CHAT] Subscribed! channels[0]=" + channels[0] + " result=" + results[0]);
        Debug.Log("[CHAT UI] Enabling chat panel: " + chatPanel.name);
        isSubscribed = true;


        // Now enable UI input
        if (chatPanel != null) chatPanel.SetActive(true);
        if (inputField != null) inputField.interactable = true;

        // Optional: system message
        chatClient.PublishMessage(currentChannel, userName + " joined the chat");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            // sender is already who sent it
            channelMessagesText.text += "\n" + senders[i] + ": " + messages[i];
        }
    }

    public void OnDisconnected()
    {
        Debug.Log("[CHAT] Disconnected.");
        isConnected = false;
        isSubscribed = false;

        if (inputField != null) inputField.interactable = false;
    }

    // Required but unused in your case
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnUnsubscribed(string[] channels) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }
}

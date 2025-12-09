using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    public Text roomText; // UI label for this entry (room)
    public string roomName; // target room name

    public void JoinRoom()
    {
        Debug.Log("Room has been joined!");

        PhotonNetwork.LeaveLobby();      // leave lobby state
        PhotonNetwork.JoinRoom(roomName); // join the selected room
    }
}

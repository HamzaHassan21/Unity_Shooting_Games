using System;
[System.Serializable]
public class PlayerData
{
    public string username;
    public int bestScore;
    public string date;
    public int totalPlayersInTheGame;
    public string roomName;
    public string uid;

    public PlayerData()
    {
        uid = Guid.NewGuid().ToString();
        
    }
}
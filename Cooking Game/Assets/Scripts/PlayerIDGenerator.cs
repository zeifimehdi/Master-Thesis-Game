using UnityEngine;

public class PlayerIDGenerator : MonoBehaviour
{
    public void PlayerIDToCreateFile()
    {
        int lastPlayerID = PlayerPrefs.GetInt("LastPlayerID", 0) + 1;
        string uniqueID = "Player_" + lastPlayerID + "_data";

        PlayerPrefs.SetInt("LastPlayerID", lastPlayerID);
        PlayerPrefs.SetString("PlayerID", uniqueID);

        FileHandler.CreateFile(uniqueID);
    }
}
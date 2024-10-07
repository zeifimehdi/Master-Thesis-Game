using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance; // Singleton instance

    private int[] levelCompletionStatus; // Array to store completion status of each level
    private int currentDay = 1; // Current day

    void Awake()
    {
        if (instance == null)
        {
            instance = this; // Assign this object as the singleton instance
            DontDestroyOnLoad(gameObject); // Don't destroy this object when loading new scenes
            InitializeLevelCompletionStatus(); // Initialize level completion status array
            LoadCurrentDay(); // Load current day from PlayerPrefs
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void InitializeLevelCompletionStatus()
    {
        levelCompletionStatus = new int[5]; // Assuming 5 levels, initialize the array

        // Load completion status of each level from PlayerPrefs
        for (int i = 0; i < levelCompletionStatus.Length; i++)
        {
            levelCompletionStatus[i] = PlayerPrefs.GetInt("Level_" + (i + 1), 0); // Default is 0 (not completed)
        }
    }

    public void MarkLevelCompleted(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelCompletionStatus.Length)
        {
            levelCompletionStatus[levelIndex] = 1; // Set completion status to 1 (completed)
            PlayerPrefs.SetInt("Level_" + (levelIndex + 1), 1); // Save completion status in PlayerPrefs
            PlayerPrefs.Save(); // Save PlayerPrefs
        }
    }

    public bool IsLevelCompleted(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelCompletionStatus.Length)
        {
            return levelCompletionStatus[levelIndex] == 1; // Return true if level is completed
        }
        return false; // Default to false if levelIndex is out of range
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    public void IncrementDay()
    {
        currentDay++;
        PlayerPrefs.SetInt("CurrentDay", currentDay); // Save current day in PlayerPrefs
        PlayerPrefs.Save(); // Save PlayerPrefs
    }

    void LoadCurrentDay()
    {
        currentDay = PlayerPrefs.GetInt("CurrentDay", 1); // Default is day 1
    }
}

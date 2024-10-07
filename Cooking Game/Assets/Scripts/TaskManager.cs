using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class TaskManager : MonoBehaviour
{
    public static TaskManager instance;

    public GameObject[] helpers; // Added a reference to helper objects in the game canvas
    public GameObject mainPlayer;

    public GameObject letsGoBG;

    public float displayDuration = 14f; // Duration in seconds for which the dialogue will be displayed

    public string[] levelDialogues = {
        "Get ready to put on your chef's hat! We're about to cook up something delicious.\nJoin us in the kitchen and let's whip up some tasty dishes together!",
        "Hurry up!! The thunder storm is about to hit the town",
        "This is dialogue for Day 3.",
        "This is dialogue for Day 4.",
        "This is dialogue for Day 5."
    };

    public Text dialogueText;
    public Text levelStartText;
    public Text instructionText;

    public Transform tasklistParent;
    public GameObject tasklistPrefab;
    public GameObject capacityOverflowWarning;
    public GameObject ScoreText;

    public Canvas DialogueCanvas;

    [HideInInspector] public int currentLevel = 1;
    [HideInInspector] public int assignmentsCompleted = 0;
    [HideInInspector] public int score = 0;
    string fileName;
    private string currentLevelTaskFilePath;
    private int totalAssignmentsNeeded = 4;

    GameObject[] taskPrefabs;

    public int[,] helperCapacities = new int[5, 3] {
    {3, 2, 3}, // Capacities for Level 1 helpers
{2, 4, 1}, // Capacities for Level 2 helpers
{2, 4, 1}, // Capacities for Level 3 helpers
{3, 4, 2}, // Capacities for Level 4 helpers
{2, 2, 3}  // Capacities for Level 5 helpers
};

    private struct TaskDetails
    {
        public string taskText;
        public GameObject targetHelper;
    }

    // A list to hold all task details before instantiation
    private List<TaskDetails> pendingTaskDetails = new List<TaskDetails>();

    private void Awake()
    {
        instance = this;
        fileName = PlayerPrefs.GetString("PlayerID"); // Get the unique player ID from PlayerPrefs
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel",1);
        }
        FileHandler.WriteToFile(fileName, "Level-" + currentLevel.ToString());

        LoadTasksForLevel(currentLevel);
        LoadTaskPrefabsForLevel(currentLevel);
        ShowDialogueForCurrentLevel();
    }

   public void ShowDialogueForCurrentLevel()
   {
        levelStartText.text = "Day " + currentLevel;

        instructionText.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(true);

        if (currentLevel <= levelDialogues.Length)
        {
            dialogueText.text = levelDialogues[currentLevel - 1]; // Display dialogue for current level
        }
        else
        {
            Debug.LogWarning("No dialogue set for level " + currentLevel);
        }
        Invoke("HideDialogue", displayDuration); // Hide dialogue after displayDuration seconds
   }

    private void HideDialogue()
    {
        dialogueText.text = ""; // Clear the dialogue text

        TransitionManager.instance.taskCanvas.gameObject.SetActive(true); // Activate the task canvas

        letsGoBG.SetActive(true);

        dialogueText.gameObject.SetActive(true);
        DialogueCanvas.gameObject.SetActive(false); // Deactivate the dialogue canvas
    }
   
    public void LoadTaskPrefabsForLevel(int level)
    {
        string folderName = "Level" + level;
        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>(folderName);

        // Assign loaded prefabs to the public array
        taskPrefabs = new GameObject[loadedPrefabs.Length];
        for (int i = 0; i < loadedPrefabs.Length; i++)
        {
            taskPrefabs[i] = loadedPrefabs[i];
        }
    }
    void LoadTasksForLevel(int level)
    {
        string fileName = "level_" + level + "_tasks" + ".txt";
        currentLevelTaskFilePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(currentLevelTaskFilePath))
        {
            string[] taskLines = File.ReadAllLines(currentLevelTaskFilePath);

            foreach (string taskLine in taskLines)
            {
                GameObject newTask = Instantiate(tasklistPrefab, tasklistParent);
                newTask.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = taskLine;
            }
        }
        else
        {
            Debug.LogError("Task file not found for level " + level);
        }
    }

    public void AssignTaskToHelper(string taskText, GameObject targetHelper)
    {
        GameObject taskPrefab = null;
        foreach (GameObject prefab in taskPrefabs)
        {
            if (prefab.name == taskText)
            {
                taskPrefab = prefab;
                break;
            }

        }

        if (taskPrefab == null)
        {
            Debug.LogError("Task prefab not found for task text: " + taskText);
            return;
        }

        HelperReference helperScript = targetHelper.GetComponent<HelperReference>();

        int currentLevelCapacity = helperCapacities[currentLevel - 1, helperScript.helperIndex];
        helperScript.capacity = currentLevelCapacity;


        // Store the details of the task assignment
        pendingTaskDetails.Add(new TaskDetails { taskText = taskText, targetHelper = targetHelper });
       
        helperScript.assignedTasks.Add(taskPrefab);
       
        score += Random.Range(50, 90);

        assignmentsCompleted++;
        if (assignmentsCompleted == 3)
        {
            TransitionManager.instance.TaskAssigned();
        }
        CheckAssignmentsComplete();
        //StartCoroutine(EnableAdditionalTasks(helperScript));
    }

    public void AssignTaskToPlayer(string taskText)
    {
        GameObject taskPrefab = taskPrefabs.FirstOrDefault(prefab => prefab.name == taskText);

        if (taskPrefab == null)
        {
            Debug.LogError("Task prefab not found for task text: " + taskText);
            return;
        }

        InstantiateTask(taskText, mainPlayer);
        StartCoroutine("WaitForAnim");
    }

    public void CheckAssignmentsComplete()
    {
        // Check if all assignments are done (including the player task)
        if (assignmentsCompleted >= totalAssignmentsNeeded )
        {
            // If all tasks are assigned, instantiate them
            foreach (TaskDetails details in pendingTaskDetails)
            {
                InstantiateTask(details.taskText, details.targetHelper);
            }
            pendingTaskDetails.Clear();
        }
    }
    IEnumerator EnableAdditionalTasks(HelperReference helperScript)
    {
        yield return new WaitForSeconds(5f);

        // Enable additional prefabs one by one after a delay
        for (int i = 1; i < helperScript.assignedTasks.Count; i++)
        {
            yield return new WaitForSeconds(20f); // Wait before enabling the next task

            // Disable all prefabs except the one at index 'i'
            for (int j = 0; j < helperScript.assignedTasks.Count; j++)
            {
                if (j != i)
                {
                    Transform taskContainer = helperScript.myHelper.transform.GetChild(1);
                    if (j < taskContainer.childCount)
                    {
                        taskContainer.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }

            // Enable the task prefab at index 'i'
            Transform currentTask = helperScript.myHelper.transform.GetChild(1).GetChild(i);
            if (currentTask != null)
            {
                currentTask.gameObject.SetActive(true);
            }
        }
    }

    private void InstantiateTask(string taskText, GameObject target)
    {
        GameObject taskPrefab = taskPrefabs.FirstOrDefault(prefab => prefab.name == taskText);
        if (taskPrefab != null)
        {
            if (target.CompareTag("Helper"))
            {
                HelperReference helperScript = target.GetComponent<HelperReference>();
                GameObject taskObject = Instantiate(taskPrefab, helperScript.myHelper.transform.GetChild(1).transform);
                taskObject.transform.localPosition = Vector3.zero;
              
            }
            else if (target.CompareTag("Player"))
            {
                GameObject taskObject = Instantiate(taskPrefab, target.transform.GetChild(1).transform);
                taskObject.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            Debug.LogError("Task prefab not found for task text: " + taskText);
        }
    }

    IEnumerator WaitForAnim()
    {
        yield return new WaitForSeconds(5f); // waits for 5 seconds
        TransitionManager.instance.GameCompleteCanvas.gameObject.SetActive(true);
        TransitionManager.instance.gameCanvas.gameObject.SetActive(false);
        
        TransitionManager.instance.GameCompleteCanvas.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = "we successfully completed the all important tasks. you got " + score.ToString() + " points. Now see you tomorrow";

        if (currentLevel == 5)
        {
            TransitionManager.instance.GameCompleteCanvas.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = "you got " + score.ToString() + " points.You have successfully completed the game\n Press esc to close";
            TransitionManager.instance.GameCompleteCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        }
    }
    
    public void CompleteLevel()
    {
        ClearTaskWindow();
        assignmentsCompleted = 0;
       // score = 0;
        pendingTaskDetails.Clear();

        currentLevel++;

        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
        TransitionManager.instance.GameCompleteCanvas.gameObject.SetActive(false);
        DialogueCanvas.gameObject.SetActive(true);

        FileHandler.WriteToFile(fileName,"Level-"+ currentLevel.ToString());
        ShowDialogueForCurrentLevel();

        TransitionManager.instance.playerTaskSelected = false;

        LoadTasksForLevel(currentLevel);
        LoadTaskPrefabsForLevel(currentLevel);
    }

    void ClearTaskWindow()
    {
        foreach (Transform child in tasklistParent)
        {
            Destroy(child.gameObject);
        }
    }
}
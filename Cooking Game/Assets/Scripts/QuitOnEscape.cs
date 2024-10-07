using UnityEngine;

public class QuitOnEscape : MonoBehaviour
{
    private static QuitOnEscape instance;

    private void Awake()
    {
        // Ensure only one instance of QuitOnEscape exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene changes
        }
        else
        {
            // If another instance already exists, destroy this one
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Quit the application
            Application.Quit();
        }
    }
}

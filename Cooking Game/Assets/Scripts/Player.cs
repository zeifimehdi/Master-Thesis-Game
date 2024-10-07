using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public GameObject myPlayer;

    private void Awake()
    {
        instance = this;
    }

    public void ClearAssignedTask()
    {
        // Iterate through the children of the current GameObject and destroy them
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // Iterate through the children of the current GameObject and destroy them
        for (int i = myPlayer.transform.GetChild(1).childCount - 1; i >= 0; i--)
        {
            // Destroy the child at the current index
            Destroy(myPlayer.transform.GetChild(1).GetChild(i).gameObject);
        }
    }
}

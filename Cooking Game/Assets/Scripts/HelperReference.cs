using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperReference : MonoBehaviour
{
    public static HelperReference instance;
    public GameObject myHelper;

    public List<GameObject> assignedTasks = new List<GameObject>();  // List to hold assigned tasks

    [HideInInspector] public int capacity = 2;
    [HideInInspector]public int helperIndex;


    private void Awake()
    {
        instance = this;

        // Determine the helper index based on the order in the parent
        helperIndex = transform.GetSiblingIndex();
    }

    public void ClearAssignedTasks()
    {
        // Clear the assigned tasks list
        assignedTasks.Clear();

        // Iterate through the children of the current GameObject and destroy them
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
               Destroy(transform.GetChild(i).gameObject);
        }

        // Iterate through the children of the current GameObject and destroy them
        for (int i = myHelper.transform.GetChild(1).childCount - 1; i >= 0; i--)
        {
            // Destroy the child at the current index
            Destroy(myHelper.transform.GetChild(1).GetChild(i).gameObject);
        }
    }
}
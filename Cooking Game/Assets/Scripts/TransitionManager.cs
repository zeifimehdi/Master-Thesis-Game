using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    public Canvas taskCanvas;
    public Canvas gameCanvas;
    public Canvas GameCompleteCanvas;


    public Text instructionText;


    [HideInInspector]public bool playerTaskSelected = false;


    private void Awake()
    {
        instance = this;
    }

    public void TaskAssigned()
    {
       instructionText.text = "Drop one task for main chef at bottom";
    }

    public void TaskSelected()
    {
        if (!playerTaskSelected)
        {
            playerTaskSelected = true;
            taskCanvas.gameObject.SetActive(false);
            gameCanvas.gameObject.SetActive(true);
        }
    }
}

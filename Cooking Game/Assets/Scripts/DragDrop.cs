using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    private void Start()
    {
        TaskManager.instance.capacityOverflowWarning.SetActive(false);

        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        canvasGroup.blocksRaycasts = false;
        TaskManager.instance.capacityOverflowWarning.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    private void ResetToOriginalPosition()
    {
            // Disable the grid layout group temporarily
            GridLayoutGroup gridLayout = GetComponentInParent<GridLayoutGroup>();
            bool wasGridLayoutEnabled = gridLayout.enabled;
            gridLayout.enabled = false;

            // Reset the position of the last dropped task only
            rectTransform.localPosition = originalPosition;

            // Re-enable the grid layout group
            gridLayout.enabled = wasGridLayoutEnabled;        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!eventData.pointerEnter || (!eventData.pointerEnter.CompareTag("Helper") && !eventData.pointerEnter.CompareTag("Player")))
        {
            ResetToOriginalPosition();
        }
        else
        {
            if (eventData.pointerEnter.CompareTag("Helper"))
            {
                GameObject droppedObject = eventData.pointerEnter;
                HelperReference helperScript = droppedObject.GetComponent<HelperReference>();

                if (helperScript.assignedTasks.Count >= helperScript.capacity)
                {
                    TaskManager.instance.capacityOverflowWarning.SetActive(true);
                    ResetToOriginalPosition();
                }

                else
                {
                    TaskManager.instance.capacityOverflowWarning.SetActive(false);

                    transform.SetParent(droppedObject.transform);
                    rectTransform.localPosition = Vector3.zero;

                    string taskText = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    string fileName = PlayerPrefs.GetString("PlayerID"); 

                    FileHandler.WriteToFile(fileName, droppedObject.name+" -"+taskText);
                        
                    TaskManager.instance.AssignTaskToHelper(taskText, droppedObject);
                    TaskManager.instance.ScoreText.transform.GetComponent<TextMeshProUGUI>().text = TaskManager.instance.score.ToString();
                }               
            }
            else if (eventData.pointerEnter.CompareTag("Player"))
            {
                transform.SetParent(eventData.pointerEnter.transform);
                rectTransform.localPosition = Vector3.zero;
                
                TaskManager.instance.assignmentsCompleted++;
                TaskManager.instance.CheckAssignmentsComplete();
               

                string taskText = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                string fileName = PlayerPrefs.GetString("PlayerID"); 
                FileHandler.WriteToFile(fileName, "Player -"+taskText);
                TransitionManager.instance.TaskSelected();

                // Assign task to the player (main chef)
                TaskManager.instance.AssignTaskToPlayer(taskText);
            }
        }
    }
}

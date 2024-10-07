using UnityEngine;
using UnityEngine.UI;

public class DecreasingSlider : MonoBehaviour
{
    public Slider slider;
    public float decreaseSpeed = 1f;
    private bool decreasing = false;

    void Start()
    {
        // Set the initial value of the slider to its maximum value
        slider.value = slider.maxValue;

        // Start decreasing the slider value
        StartDecreasing();
    }

    void Update()
    {
        // If the slider value reaches 0, reset it to its maximum value
        if (slider.value == 0 && !decreasing)
        {
            ResetSlider();
        }
    }

    void StartDecreasing()
    {
        decreasing = true;
        DecreaseSlider();
    }

    void DecreaseSlider()
    {
        // Decrease the slider value gradually
        slider.value -= decreaseSpeed * Time.deltaTime;

        // If the slider value reaches 1 and it's already decreasing, stop decreasing
        if (slider.value <= 1 && decreasing)
        {
            decreasing = false;
        }

        // If still decreasing, continue decreasing recursively
        if (decreasing)
        {
            Invoke("DecreaseSlider", 0.1f); // Adjust the speed here (seconds)
        }
    }

    void ResetSlider()
    {
        slider.value = slider.maxValue;
        StartDecreasing();
    }
}

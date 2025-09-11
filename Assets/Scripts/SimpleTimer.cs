using UnityEngine;
using TMPro;

public class SimpleTimer : MonoBehaviour
{
    public float timeElapsed = 0f;  // Rename to timeElapsed for clarity
    public bool timerIsRunning = true;

    private TextMeshProUGUI timerText;

    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();

        if (timerText == null)
        {
            Debug.LogError("TextMeshProUGUI component NOT found on " + gameObject.name);
        }
        else
        {
            Debug.Log("TextMeshProUGUI component found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (timerIsRunning)
        {
            timeElapsed += Time.deltaTime;

            DisplayTime(timeElapsed);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

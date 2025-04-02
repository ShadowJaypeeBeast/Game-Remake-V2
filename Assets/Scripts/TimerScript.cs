using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private float startTime = 60f; // The time the player has to complete the level
    [SerializeField] private TextMeshProUGUI timerDisplay; // The text that will display the time

    private float currentTime; // The current time left
    private bool levelEnded = false; // Whether the level has ended

    void Start()
    {
        currentTime = startTime; // Initialize the timer
        UpdateTimerUI(); // Update the timer display
    }

    // Update is called once per frame
    void Update()
    {
        // Only update the timer if the level has not ended
        if (!levelEnded)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime; // Countdown the timer
                currentTime = Mathf.Max(0, currentTime); // Ensure the timer does not go below 0
                UpdateTimerUI(); // Update the timer display
            }
            else
            {
                EndLevel(); // Trigger level end if time runs out
            }
        }
    }

    // Update the timer display
    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60); // Calculate the minutes
        int seconds = Mathf.FloorToInt(currentTime % 60); // Calculate the seconds
        timerDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Display the timer in MM:SS format
    }

    // Handle the end of the level and restart the scene
    void EndLevel()
    {
        if (levelEnded) return; // Prevent the level from ending multiple times
        levelEnded = true; // Mark the level as ended
        Debug.Log("Time has Run Out! Restarting level...");

        // Restart the level after a delay
        Invoke("RestartLevel", 1f); // Wait for 1 second before restarting the level
    }

    // Restart the current scene
    void RestartLevel()
    {
        Debug.Log("Restarting Level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }

    // Stop the timer if the player completes the level early
    public void PlayerFinishedLevelEarly()
    {
        // Immediately stop the timer and trigger the level end
        currentTime = 0; // Set time to 0
        levelEnded = true; // Mark the level as ended
        Debug.Log("Player has finished the level early!");
    }
}
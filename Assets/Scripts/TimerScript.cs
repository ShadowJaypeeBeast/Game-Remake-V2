using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using JetBrains.Annotations;


public class TimerScript : MonoBehaviour
{
    [SerializeField] private float startTime; = 60f; // The time the player has to complete the level
    [SerializeField] private TextMeshProUGUI timerDisplay; // The text that will display the time

    private float currentTime; // The current time left
    private bool levelEnded = false; // Whether the level has ended

    void Start()
    {
        currentTime = startTime;
        updateTimerUI(); //Initialise the timer
    }

    // Update is called once per frame
    void Update()
    {
        //Only Update the timer if the level has not ended
        if (!levelEnded)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime; // Countdown the timer
                currentTime = Mathf.Max(0, currentTime 0f); // Ensure the timer does not go below 0
                UpdateTimerUI(); // Update the timer display
            }
            else
            {
                EndLevel()
            }
        }
    }
    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60); // Calculate the minutes
        int seconds = Mathf.FloorToInt(currentTime % 60); // Calculate the seconds
        timerDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Update the timer display
    }
    //Handle the end of the level and restarts the scene
    void EndLevel()
    {
        if (levelEnded) return; // Prevent the level from ending multiple times
        levelEnded = true;
        Debug.Log("Time has Run Out! Restarting level...");

        //restart the level after a delay
        invoke("RestartLeel", 1f);//wait forr a cond before restartitng the level

        // restart the current scene
        void RestartLevel()
        {
            Debug.Log("Restarting Level...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        //call this method to stop the timer if the player has completed the level
        
        public void PlayerFinishedLevelEarly()
    {
        // Immediately stop the timer and trigger the level end 
        currentTime = 0;
        levelEnded();
        Debug.Log("Player has finished the level early!");
    }
}
}


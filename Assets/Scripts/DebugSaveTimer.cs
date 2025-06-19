using UnityEngine;
using System;

public class DebugSaveTimer : MonoBehaviour
{
    private float timeRemaining;
    private bool timerIsRunning = false;
    
    void Start()
    {
        Debug.Log("Play time: " + PlayerPrefs.GetInt("PlayTimeMinutes"));
        Debug.Log("Focus time: " + PlayerPrefs.GetInt("FocusTimeMinutes"));
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                // Subtract delta time (time between frames) from our timer
                timeRemaining -= Time.deltaTime;
                Debug.Log("Time remaining: " + FormatTime(timeRemaining));
            }
            else
            {
                // Timer has finished
                timeRemaining = 0;
                timerIsRunning = false;
                Debug.Log("Timer complete!");
                // Add any code to execute when timer completes
            }
        }
    }

    public void BeginTimer(float startTimeInSeconds = 0)
    {
        // If no time provided, try to get it from PlayerPrefs (in minutes)
        if (startTimeInSeconds <= 0)
        {
            int minutes = PlayerPrefs.GetInt("PlayTimeMinutes", 10); // Default 10 minutes if not set
            startTimeInSeconds = minutes * 60f;
        }
        
        timeRemaining = startTimeInSeconds;
        timerIsRunning = true;
        Debug.Log("Timer started with " + FormatTime(timeRemaining));
    }
    
    // Helper method to format seconds as mm:ss
    private string FormatTime(float timeInSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
    }
    
    // You can call this method to stop the timer
    public void StopTimer()
    {
        timerIsRunning = false;
        Debug.Log("Timer stopped at " + FormatTime(timeRemaining));
    }
    
    // You can call this to get the current remaining time
    public float GetRemainingTime()
    {
        return timeRemaining;
    }
}

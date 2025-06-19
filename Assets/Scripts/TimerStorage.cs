using UnityEngine;
using TMPro;

public class TimerStorage : MonoBehaviour
{
    // Text components to display timer values
    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private TextMeshProUGUI focusTimeText;

    // Timer values in minutes
    private int playTimeMinutes = 0;
    private int focusTimeMinutes = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Load saved timer values when the script starts
        LoadTimerValues();
        
        UpdatePlayTimeDisplay();
        UpdateFocusTimeDisplay();
    }

    // Play Time increment functions
    public void AddPlayTime5()
    {
        playTimeMinutes += 5;
        UpdatePlayTimeDisplay();
        Debug.Log("Play Time increased by 5 minutes. New value: " + playTimeMinutes + " minutes");
        
        // Save values automatically when changed
        SaveTimerValues();
    }

    public void AddPlayTime10()
    {
        playTimeMinutes += 10;
        UpdatePlayTimeDisplay();
        Debug.Log("Play Time increased by 10 minutes. New value: " + playTimeMinutes + " minutes");
        
        // Save values automatically when changed
        SaveTimerValues();
    }

    public void AddPlayTime20()
    {
        playTimeMinutes += 20;
        UpdatePlayTimeDisplay();
        Debug.Log("Play Time increased by 20 minutes. New value: " + playTimeMinutes + " minutes");
        
        // Save values automatically when changed
        SaveTimerValues();
    }

    // Focus Time increment functions
    public void AddFocusTime5()
    {
        focusTimeMinutes += 5;
        UpdateFocusTimeDisplay();
        Debug.Log("Focus Time increased by 5 minutes. New value: " + focusTimeMinutes + " minutes");
        
        // Save values automatically when changed
        SaveTimerValues();
    }

    public void AddFocusTime10()
    {
        focusTimeMinutes += 10;
        UpdateFocusTimeDisplay();
        Debug.Log("Focus Time increased by 10 minutes. New value: " + focusTimeMinutes + " minutes");
        
        // Save values automatically when changed
        SaveTimerValues();
    }

    public void AddFocusTime20()
    {
        focusTimeMinutes += 20;
        UpdateFocusTimeDisplay();
        Debug.Log("Focus Time increased by 20 minutes. New value: " + focusTimeMinutes + " minutes");
        
        // Save values automatically when changed
        SaveTimerValues();
    }

    // Reset functions
    public void ResetPlayTime()
    {
        playTimeMinutes = 0;
        UpdatePlayTimeDisplay();
        Debug.Log("Play Time reset to 0");
        
        // Save values automatically when changed
        SaveTimerValues();
    }

    public void ResetFocusTime()
    {
        focusTimeMinutes = 0;
        UpdateFocusTimeDisplay();
        Debug.Log("Focus Time reset to 0");
        
        // Save values automatically when changed
        SaveTimerValues();
    }

    // Update the Play Time text display
    private void UpdatePlayTimeDisplay()
    {
        if (playTimeText != null)
        {
            playTimeText.text = "Play Time: " + playTimeMinutes + " min";
        }
    }

    // Update the Focus Time text display
    private void UpdateFocusTimeDisplay()
    {
        if (focusTimeText != null)
        {
            focusTimeText.text = "Focus Time: " + focusTimeMinutes + " min";
        }
    }

    // Save timer values to PlayerPrefs
    public void SaveTimerValues()
    {
        PlayerPrefs.SetInt("PlayTimeMinutes", playTimeMinutes);
        PlayerPrefs.SetInt("FocusTimeMinutes", focusTimeMinutes);
        PlayerPrefs.Save();
        Debug.Log("Timer values saved to PlayerPrefs");
    }

    // Load timer values from PlayerPrefs
    public void LoadTimerValues()
    {
        if (PlayerPrefs.HasKey("PlayTimeMinutes"))
        {
            playTimeMinutes = PlayerPrefs.GetInt("PlayTimeMinutes");
            Debug.Log("Loaded Play Time value: " + playTimeMinutes + " minutes");
        }

        if (PlayerPrefs.HasKey("FocusTimeMinutes"))
        {
            focusTimeMinutes = PlayerPrefs.GetInt("FocusTimeMinutes");
            Debug.Log("Loaded Focus Time value: " + focusTimeMinutes + " minutes");
        }

        UpdatePlayTimeDisplay();
        UpdateFocusTimeDisplay();
    }
    
    // Get the current play time value (for other scripts to access)
    public int GetPlayTimeMinutes()
    {
        return playTimeMinutes;
    }
    
    // Get the current focus time value (for other scripts to access)
    public int GetFocusTimeMinutes()
    {
        return focusTimeMinutes;
    }
}
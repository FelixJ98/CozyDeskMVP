using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    // Text components to display timer values
    [SerializeField] private TextMeshProUGUI timerText;
    private locomotion motion;

    // Timer values in minutes
    private float playTimeMinutes = 0;
    private float focusTimeMinutes = 0;

    private float currentTime = 0f;
    private bool timerRunning = false;
    public static Timer Instance { get; private set;}
    public bool ItsPlaytime { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // Load saved timer values when the script starts
        //LoadTimerValues();
        
        playTimeMinutes = 0.1f;  // 1 minute playtime
        focusTimeMinutes = 10; // 0 minute focus time for quick test
        
        ItsPlaytime = true;
        StartTimer();
    }

    void Update()
    {
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0f)
            {
                timerRunning = false;
                SwitchMode();
            }
        }
    }

    public void StartTimer()
    {
        if (ItsPlaytime)
        {
            currentTime = playTimeMinutes * 60;
        }
        else
        {
            currentTime = focusTimeMinutes * 60;
        }

        timerRunning = true;
        UpdateTimerDisplay();
    }

    private void SwitchMode()
    {
        ItsPlaytime = !ItsPlaytime;
        Debug.Log(ItsPlaytime ? "Switched to PLAYTIME" : "Switched to FOCUS TIME");
        if (ItsPlaytime)
        {
            motion.GoHome();
        }
        StartTimer();
        
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        string mode = ItsPlaytime ? "Play Time" : "Focus Time";
        timerText.text = $"{mode} - {minutes:00}:{seconds:00}";    }
    // Play Time increment functions
   
}
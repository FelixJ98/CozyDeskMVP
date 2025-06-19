using UnityEngine;

public class MenuNav : MonoBehaviour
{  
    public GameObject mainMenuParent;
    public GameObject modeMenuParent;
    public GameObject timerMenuParent;
    private GameObject currentActiveMenu;
    
    // Keep track of the last known position and rotation
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    
    void Start()
    {
        // Set initial states
        if (mainMenuParent != null) 
        {
            mainMenuParent.SetActive(true);
            // Store initial position/rotation
            lastPosition = mainMenuParent.transform.position;
            lastRotation = mainMenuParent.transform.rotation;
        }
        
        if (modeMenuParent != null) modeMenuParent.SetActive(false);
        if (timerMenuParent != null) timerMenuParent.SetActive(false);
        
        currentActiveMenu = mainMenuParent;
    }
    
    // Add this method to update position/rotation when menu is moved
    void Update()
    {
        // If current menu exists, track its position/rotation
        if (currentActiveMenu != null)
        {
            lastPosition = currentActiveMenu.transform.position;
            lastRotation = currentActiveMenu.transform.rotation;
        }
    }
    
    public void ShowMainMenu()
    {
        SwitchMenu(mainMenuParent);
    }
    
    public void ShowModeMenu()
    {
        SwitchMenu(modeMenuParent);
    }
    
    public void ShowTimerMenu()
    {
        SwitchMenu(timerMenuParent);
    }
    
    private void SwitchMenu(GameObject targetMenu)
    {
        // Return if target is null or already active
        if (targetMenu == null || targetMenu == currentActiveMenu)
            return;
            
        // First turn off current menu
        if (currentActiveMenu != null)
        {
            currentActiveMenu.SetActive(false);
        }
        
        // Position new menu using the tracked position/rotation
        targetMenu.transform.position = lastPosition;
        targetMenu.transform.rotation = lastRotation;
        
        // Now activate the new menu
        targetMenu.SetActive(true);
        
        // Update reference
        currentActiveMenu = targetMenu;
    }
}
using UnityEngine;

public class FocusZone : MonoBehaviour 
{
    [Header("Setup")]
    public Camera playerCamera;
    public Transform desktopScreen;
    public float maxDistance = 10f;
    public float maxAngle = 30f;
    
    [Header("Debug")]
    public bool showRay = true;
    
    private bool isLooking = false;
    
    void Start() 
    {
        if (playerCamera == null) 
            playerCamera = Camera.main ?? FindObjectOfType<Camera>();
            
        if (desktopScreen == null) 
            desktopScreen = GetComponentInChildren<Renderer>().transform;
            
        Debug.Log($"Simple detector setup - Camera: {playerCamera?.name}, Screen: {desktopScreen?.name}");
    }
    
    void Update() 
    {
        if (playerCamera == null || desktopScreen == null) return;
        
        bool currentlyLooking = CheckIfLookingAtScreen();
        
        if (currentlyLooking && !isLooking) 
        {
            Debug.Log("Looking at screen");
        } 
        else if (!currentlyLooking && isLooking) 
        {
            Debug.Log("Stopped looking at screen");
        }
        
        isLooking = currentlyLooking;
        
        if (showRay) 
        {
            Color rayColor = isLooking ? Color.green : Color.red;
            Debug.DrawRay(playerCamera.transform.position, 
                         playerCamera.transform.forward * maxDistance, 
                         rayColor);
        }
    }
    
    bool CheckIfLookingAtScreen() 
    {
        Vector3 cameraPos = playerCamera.transform.position;
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 toScreen = (desktopScreen.position - cameraPos).normalized;
        
        // Check distance
        float distance = Vector3.Distance(cameraPos, desktopScreen.position);
        if (distance > maxDistance) return false;
        
        // Check angle
        float angle = Vector3.Angle(cameraForward, toScreen);
        if (angle > maxAngle) return false;
        
        // Raycast check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(cameraPos, toScreen, out hit, distance)) 
        {
            // Check if we hit the screen or something in front of it
            return hit.transform == desktopScreen || 
                   hit.transform.IsChildOf(desktopScreen) ||
                   desktopScreen.IsChildOf(hit.transform);
        }
        
        return true; // No obstacles, looking at screen
    }
    
    // Public access for other scripts
    public bool IsLookingAtScreen => isLooking;
}
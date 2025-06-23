using UnityEngine;
using UnityEngine.XR;
using System.Collections;

public class GazeDetector : MonoBehaviour 
{
    [Header("Gaze Settings")]
    public Transform targetObject; // Desktop display
    public Camera playerCamera;
    public float gazeAngleThreshold = 30f;
    public float gazeTimeThreshold = 0.5f; // How long to look before triggering
    public LayerMask raycastLayers = -1;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    public bool showGazeRay = true;
    public Color gazeRayColor = Color.blue;
    
    // Gaze tracking
    private bool isLookingAt = false;
    private float gazeStartTime = 0f;
    private bool gazeConfirmed = false;
    
    // Events
    public System.Action OnGazeStarted;
    public System.Action OnGazeEnded;
    public System.Action OnGazeConfirmed; // After threshold time
    
    void Start() 
    {
        if (playerCamera == null) 
        {
            playerCamera = Camera.main;
        }
        
        if (targetObject == null) 
        {
            var desktopScript = FindObjectOfType<GrabbableDesktopDisplay>();
            if (desktopScript != null) 
            {
                targetObject = desktopScript.transform;
            }
        }
    }
    
    void Update() 
    {
        CheckGaze();
    }
    
    void CheckGaze() 
    {
        if (playerCamera == null || targetObject == null) return;
        
        bool currentlyLookingAt = IsLookingAtTarget();
        
        if (currentlyLookingAt && !isLookingAt) 
        {
            // Started looking
            StartGaze();
        } 
        else if (!currentlyLookingAt && isLookingAt) 
        {
            // Stopped looking
            EndGaze();
        } 
        else if (currentlyLookingAt && isLookingAt && !gazeConfirmed) 
        {
            // Check if gaze has been held long enough
            if (Time.time - gazeStartTime >= gazeTimeThreshold) 
            {
                ConfirmGaze();
            }
        }
    }
    
    bool IsLookingAtTarget() 
    {
        Vector3 directionToTarget = (targetObject.position - playerCamera.transform.position).normalized;
        Vector3 cameraForward = playerCamera.transform.forward;
        
        float angle = Vector3.Angle(cameraForward, directionToTarget);
        
        // Additional raycast check to ensure target is visible
        if (angle <= gazeAngleThreshold) 
        {
            Ray ray = new Ray(playerCamera.transform.position, directionToTarget);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayers)) 
            {
                // Check if we hit the target object or its children
                return hit.transform == targetObject || hit.transform.IsChildOf(targetObject);
            }
        }
        
        return false;
    }
    
    void StartGaze() 
    {
        isLookingAt = true;
        gazeStartTime = Time.time;
        gazeConfirmed = false;
        
        OnGazeStarted?.Invoke();
        
        if (enableDebugLogs) 
        {
            Debug.Log("Looking at screen - Gaze started");
        }
    }
    
    void EndGaze() 
    {
        isLookingAt = false;
        gazeConfirmed = false;
        
        OnGazeEnded?.Invoke();
        
        if (enableDebugLogs) 
        {
            Debug.Log("Looking at screen - Gaze ended");
        }
    }
    
    void ConfirmGaze() 
    {
        gazeConfirmed = true;
        OnGazeConfirmed?.Invoke();
        
        if (enableDebugLogs) 
        {
            Debug.Log("Looking at screen - Gaze confirmed (sustained)");
        }
    }
    
    void OnDrawGizmos() 
    {
        if (!showGazeRay || playerCamera == null || targetObject == null) return;
        
        Gizmos.color = isLookingAt ? Color.green : gazeRayColor;
        Vector3 direction = (targetObject.position - playerCamera.transform.position).normalized;
        Gizmos.DrawRay(playerCamera.transform.position, direction * Vector3.Distance(playerCamera.transform.position, targetObject.position));
        
        // Draw gaze cone
        if (isLookingAt) 
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetObject.position, 0.2f);
        }
    }
    
    public bool IsCurrentlyLookingAt() 
    {
        return isLookingAt;
    }
    
    public bool IsGazeConfirmed() 
    {
        return gazeConfirmed;
    }
    
    public float GetGazeDuration() 
    {
        return isLookingAt ? Time.time - gazeStartTime : 0f;
    }
}
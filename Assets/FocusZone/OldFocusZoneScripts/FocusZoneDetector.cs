using UnityEngine;

public class FocusZoneDetector : MonoBehaviour 
{
    [Header("Detection Settings")]
    public bool enableDebugLogs = true;
    public string playerTag = "Player";
    
    [Header("Detection Status")]
    public bool playerInZone = false;
    
    // Events for other scripts
    public System.Action OnPlayerEnterZone;
    public System.Action OnPlayerExitZone;
    
    private Transform playerTransform;
    
    void Start() 
    {
        // Find player (camera or player rig)
        FindPlayer();
        
        if (enableDebugLogs) 
        {
            Debug.Log($"✓ Focus Zone Detector active on {gameObject.name}");
            Debug.Log($"  Player found: {(playerTransform ? playerTransform.name : "NULL")}");
        }
    }
    
    void FindPlayer() 
    {
        // Try to find by tag first
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        
        // Fallback to camera
        if (playerObject == null) 
        {
            Camera mainCam = Camera.main;
            if (mainCam != null) 
            {
                playerObject = mainCam.gameObject;
            }
        }
        
        // Fallback to any camera
        if (playerObject == null) 
        {
            Camera anyCamera = FindObjectOfType<Camera>();
            if (anyCamera != null) 
            {
                playerObject = anyCamera.gameObject;
            }
        }
        
        if (playerObject != null) 
        {
            playerTransform = playerObject.transform;
        }
    }
    
    void Update() 
    {
        // Manual check in case trigger events don't work
        if (playerTransform != null) 
        {
            CheckPlayerInZone();
        }
    }
    
    void CheckPlayerInZone() 
    {
        Collider zoneCollider = GetComponent<Collider>();
        if (zoneCollider == null) return;
        
        bool currentlyInZone = zoneCollider.bounds.Contains(playerTransform.position);
        
        if (currentlyInZone && !playerInZone) 
        {
            // Player entered zone
            playerInZone = true;
            HandlePlayerEnterZone();
        } 
        else if (!currentlyInZone && playerInZone) 
        {
            // Player left zone
            playerInZone = false;
            HandlePlayerExitZone();
        }
    }
    
    void OnTriggerEnter(Collider other) 
    {
        if (IsPlayer(other.transform)) 
        {
            playerInZone = true;
            HandlePlayerEnterZone();
        }
    }
    
    void OnTriggerExit(Collider other) 
    {
        if (IsPlayer(other.transform)) 
        {
            playerInZone = false;
            HandlePlayerExitZone();
        }
    }
    
    bool IsPlayer(Transform other) 
    {
        return other == playerTransform || 
               other.CompareTag(playerTag) || 
               other.GetComponent<Camera>() != null;
    }
    
    void HandlePlayerEnterZone() 
    {
        if (enableDebugLogs) 
        {
            Debug.Log("Focus Ray"); 
        }
        
        OnPlayerEnterZone?.Invoke();
    }
    
    void HandlePlayerExitZone() 
    {
        if (enableDebugLogs) 
        {
            Debug.Log("Player left focus zone");
        }
        
        OnPlayerExitZone?.Invoke();
    }
    
    // Public methods
    public bool IsPlayerInZone() 
    {
        return playerInZone;
    }
}
using UnityEngine;

public class FocusZoneCreatorHands : MonoBehaviour 
{
    [Header("Input Settings - Controllers")]
    public OVRInput.Button createZoneButton = OVRInput.Button.PrimaryIndexTrigger;
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    
    [Header("Input Settings - Hand Tracking")]
    public bool enableHandTracking = true;
    public OVRHand.HandFinger pinchFinger = OVRHand.HandFinger.Index;
    public float pinchThreshold = 0.8f; // How much to pinch to activate
    
    [Header("Hand Tracking - Manual Assignment")]
    public OVRHand rightHand; // Drag from Inspector
    public OVRHand leftHand;  // Drag from Inspector
    public bool useRightHand = true;
    
    [Header("Zone Settings")]
    public Material previewMaterial;
    public Material finalMaterial;
    public Color previewColor = Color.yellow;
    public Color finalColor = Color.green;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    // Private variables
    private bool isCreatingZone = false;
    private Vector3 startPosition;
    private GameObject previewCube;
    private GameObject finalZone;
    private Transform controllerTransform;
    private bool isUsingHandTracking = false;
    
    // Hand tracking variables
    private bool wasPinching = false;
    private OVRHand currentHand;
    
    void Start() 
    {
        InitializeCreator();
    }
    
    void InitializeCreator() 
    {
        // Find controller transform
        controllerTransform = FindControllerTransform();
        
        // Setup hand tracking
        SetupHandTracking();
        
        // Create materials if not assigned
        CreateDefaultMaterials();
        
        if (enableDebugLogs) 
        {
            Debug.Log("✓ Focus Zone Creator initialized (Controller + Hand Tracking)");
            Debug.Log($"  Controller found: {(controllerTransform ? controllerTransform.name : "NULL")}");
        }
    }
    
    void SetupHandTracking() 
    {
        if (!enableHandTracking) return;
        
        currentHand = useRightHand ? rightHand : leftHand;
        
        if (currentHand == null) 
        {
            Debug.LogWarning("No hand assigned! Please drag OVRHand components to the Inspector.");
        }
        
        if (enableDebugLogs) 
        {
            Debug.Log($"Hand Tracking Setup:");
            Debug.Log($"  Right Hand: {(rightHand ? rightHand.name : "Not Assigned")}");
            Debug.Log($"  Left Hand: {(leftHand ? leftHand.name : "Not Assigned")}");
            Debug.Log($"  Current Hand: {(currentHand ? currentHand.name : "None")}");
        }
    }
    
    Transform FindControllerTransform() 
    {
        // Try multiple common controller names
        string[] controllerNames = {
            "RightHandAnchor", "Right Controller", "RightHand",
            "LeftHandAnchor", "Left Controller", "LeftHand",
            "Controller (right)", "Controller (left)"
        };
        
        foreach (string name in controllerNames) 
        {
            GameObject controller = GameObject.Find(name);
            if (controller != null) 
            {
                return controller.transform;
            }
        }
        
        // Fallback to camera
        Camera mainCam = Camera.main;
        if (mainCam != null) 
        {
            Debug.LogWarning("Controller not found, using camera as fallback");
            return mainCam.transform;
        }
        
        Debug.LogError("No controller or camera found!");
        return null;
    }
    
    void CreateDefaultMaterials() 
    {
        if (previewMaterial == null) 
        {
            previewMaterial = new Material(Shader.Find("Standard"));
            previewMaterial.color = previewColor;
            previewMaterial.SetFloat("_Mode", 3); // Transparent mode
            previewMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            previewMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            previewMaterial.SetInt("_ZWrite", 0);
            previewMaterial.DisableKeyword("_ALPHATEST_ON");
            previewMaterial.EnableKeyword("_ALPHABLEND_ON");
            previewMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            previewMaterial.renderQueue = 3000;
            
            Color transparentPreview = previewColor;
            transparentPreview.a = 0.3f;
            previewMaterial.color = transparentPreview;
        }
        
        if (finalMaterial == null) 
        {
            finalMaterial = new Material(Shader.Find("Standard"));
            finalMaterial.color = finalColor;
            // Make final material also semi-transparent
            finalMaterial.SetFloat("_Mode", 3);
            finalMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            finalMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            finalMaterial.SetInt("_ZWrite", 0);
            finalMaterial.EnableKeyword("_ALPHABLEND_ON");
            finalMaterial.renderQueue = 3000;
            
            Color transparentFinal = finalColor;
            transparentFinal.a = 0.5f;
            finalMaterial.color = transparentFinal;
        }
    }
    
    void Update() 
    {
        // Check which input method is active
        DetermineActiveInputMethod();
        
        // Handle zone creation based on active input
        if (isUsingHandTracking) 
        {
            HandleHandTrackingInput();
        } 
        else 
        {
            HandleControllerInput();
        }
        
        // Update preview cube regardless of input method
        UpdatePreviewCube();
        
        // Debug controls
        if (enableDebugLogs) 
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                LogCurrentInputStatus();
            }
            
            if (Input.GetKeyDown(KeyCode.P) && isUsingHandTracking) 
            {
                LogPinchStrength();
            }
        }
    }
    
    void DetermineActiveInputMethod() 
    {
        // Check if hand tracking is available and active
        bool handTrackingActive = enableHandTracking && 
                                  currentHand != null && 
                                  currentHand.IsTracked;
        
        // Check if controllers are connected
        bool controllersActive = OVRInput.IsControllerConnected(controller);
        
        // Prioritize hand tracking when both are available
        if (handTrackingActive) 
        {
            isUsingHandTracking = true;
        } 
        else if (controllersActive) 
        {
            isUsingHandTracking = false;
        }
    }
    
    void HandleControllerInput() 
    {
        if (controllerTransform == null) return;
        
        // Standard controller input
        if (OVRInput.GetDown(createZoneButton, controller) && !isCreatingZone) 
        {
            StartZoneCreation();
        }
        
        if (OVRInput.GetUp(createZoneButton, controller) && isCreatingZone) 
        {
            FinishZoneCreation();
        }
    }
    
    void HandleHandTrackingInput() 
    {
        if (currentHand == null || !currentHand.IsTracked) return;
        
        // Check for pinch gesture
        bool isPinching = IsPinchingGesture();
        
        // Start zone creation on pinch start
        if (isPinching && !wasPinching && !isCreatingZone) 
        {
            StartZoneCreation();
        }
        
        // Finish zone creation on pinch release
        if (!isPinching && wasPinching && isCreatingZone) 
        {
            FinishZoneCreation();
        }
        
        wasPinching = isPinching;
    }
    
    bool IsPinchingGesture() 
    {
        if (currentHand == null || !currentHand.IsTracked) return false;
        
        float pinchStrength = currentHand.GetFingerPinchStrength(pinchFinger);
        return pinchStrength >= pinchThreshold;
    }
    
    void StartZoneCreation() 
    {
        isCreatingZone = true;
        startPosition = GetActiveHandPosition();
        
        CreatePreviewCube();
        
        if (enableDebugLogs) 
        {
            string inputMethod = isUsingHandTracking ? "Hand Tracking" : "Controller";
            Debug.Log($"Started creating zone with {inputMethod} at: {startPosition}");
        }
    }
    
    Vector3 GetActiveHandPosition() 
    {
        if (isUsingHandTracking && currentHand != null && currentHand.IsTracked) 
        {
            return currentHand.transform.position;
        } 
        else if (controllerTransform != null) 
        {
            return controllerTransform.position;
        }
        
        return Vector3.zero;
    }
    
    void CreatePreviewCube() 
    {
        if (previewCube != null) 
        {
            DestroyImmediate(previewCube);
        }
        
        previewCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        previewCube.name = "FocusZone_Preview";
        
        // Remove collider from preview (we don't want it to interfere)
        Collider previewCollider = previewCube.GetComponent<Collider>();
        if (previewCollider != null) 
        {
            DestroyImmediate(previewCollider);
        }
        
        // Apply preview material
        Renderer renderer = previewCube.GetComponent<Renderer>();
        if (renderer != null) 
        {
            renderer.material = previewMaterial;
        }
    }
    
    void UpdatePreviewCube() 
    {
        if (!isCreatingZone || previewCube == null) return;
        
        Vector3 currentPosition = GetActiveHandPosition();
        
        // Calculate cube center and size
        Vector3 center = (startPosition + currentPosition) / 2f;
        Vector3 size = new Vector3(
            Mathf.Abs(currentPosition.x - startPosition.x),
            Mathf.Abs(currentPosition.y - startPosition.y),
            Mathf.Abs(currentPosition.z - startPosition.z)
        );
        
        // Apply minimum size to prevent invisible cubes
        size = Vector3.Max(size, Vector3.one * 0.1f);
        
        // Update preview cube
        previewCube.transform.position = center;
        previewCube.transform.localScale = size;
    }
    
    void FinishZoneCreation() 
    {
        if (!isCreatingZone || previewCube == null) return;
        
        isCreatingZone = false;
        
        // Create final zone
        CreateFinalZone();
        
        // Clean up preview
        if (previewCube != null) 
        {
            DestroyImmediate(previewCube);
        }
        
        if (enableDebugLogs) 
        {
            Debug.Log($"Finished creating zone. Final size: {finalZone.transform.localScale}");
        }
    }
    
    void CreateFinalZone() 
    {
        // Remove previous zone if exists
        if (finalZone != null) 
        {
            DestroyImmediate(finalZone);
        }
        
        // Create final zone cube
        finalZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        finalZone.name = "FocusZone_Active";
        
        // Copy transform from preview
        finalZone.transform.position = previewCube.transform.position;
        finalZone.transform.localScale = previewCube.transform.localScale;
        
        // Set up collider as trigger
        Collider zoneCollider = finalZone.GetComponent<Collider>();
        if (zoneCollider != null) 
        {
            zoneCollider.isTrigger = true;
        }
        
        // Apply final material
        Renderer renderer = finalZone.GetComponent<Renderer>();
        if (renderer != null) 
        {
            renderer.material = finalMaterial;
        }
        
        // Add the focus zone detector component
        FocusZoneDetector detector = finalZone.AddComponent<FocusZoneDetector>();
        detector.enableDebugLogs = enableDebugLogs;
    }
    
    // Debug methods
    void LogCurrentInputStatus() 
    {
        Debug.Log("=== INPUT STATUS ===");
        Debug.Log($"Active Input Method: {(isUsingHandTracking ? "Hand Tracking" : "Controller")}");
        Debug.Log($"Hand Tracking Enabled: {enableHandTracking}");
        Debug.Log($"Current Hand Tracked: {(currentHand ? currentHand.IsTracked.ToString() : "No Hand Assigned")}");
        Debug.Log($"Controller Connected: {OVRInput.IsControllerConnected(controller)}");
        Debug.Log($"Creating Zone: {isCreatingZone}");
    }
    
    void LogPinchStrength() 
    {
        if (currentHand != null && currentHand.IsTracked) 
        {
            float pinchStrength = currentHand.GetFingerPinchStrength(pinchFinger);
            Debug.Log($"Pinch Strength: {pinchStrength:F2} (Threshold: {pinchThreshold})");
            Debug.Log($"Is Pinching: {IsPinchingGesture()}");
        }
    }
    
    // Public methods for runtime control
    public void SwitchToRightHand() 
    {
        useRightHand = true;
        currentHand = rightHand;
        controller = OVRInput.Controller.RTouch;
        
        if (enableDebugLogs) 
        {
            Debug.Log("Switched to right hand input");
        }
    }
    
    public void SwitchToLeftHand() 
    {
        useRightHand = false;
        currentHand = leftHand;
        controller = OVRInput.Controller.LTouch;
        
        if (enableDebugLogs) 
        {
            Debug.Log("Switched to left hand input");
        }
    }
    
    public void ToggleHandTracking(bool enable) 
    {
        enableHandTracking = enable;
        
        if (enableDebugLogs) 
        {
            Debug.Log($"Hand tracking {(enable ? "enabled" : "disabled")}");
        }
    }
    
    public void ClearCurrentZone() 
    {
        if (finalZone != null) 
        {
            DestroyImmediate(finalZone);
            finalZone = null;
            
            if (enableDebugLogs) 
            {
                Debug.Log("Focus zone cleared");
            }
        }
    }
    
    public bool HasActiveZone() 
    {
        return finalZone != null;
    }
    
    public GameObject GetActiveZone() 
    {
        return finalZone;
    }
    
    public bool IsCurrentlyCreatingZone() 
    {
        return isCreatingZone;
    }
}
using UnityEngine;

public class HandTrackingFocusZoneCreator : MonoBehaviour 
{
    [Header("Hand Tracking Settings")]
    public OVRHand.HandFinger pinchFinger = OVRHand.HandFinger.Index;
    public float pinchThreshold = 0.8f;
    
    [Header("Hand Assignment")]
    public OVRHand rightHand;
    public OVRHand leftHand;
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
    private OVRHand currentHand;
    private bool wasPinching = false;
    
    void Start() 
    {
        InitializeHandTracking();
        CreateDefaultMaterials();
        
        if (enableDebugLogs) 
        {
            Debug.Log("✓ Hand Tracking Focus Zone Creator initialized");
        }
    }
    
    void InitializeHandTracking() 
    {
        currentHand = useRightHand ? rightHand : leftHand;
        
        if (currentHand == null) 
        {
            Debug.LogError("No hand assigned! Please drag OVRHand components to the Inspector.");
        }
        
        if (enableDebugLogs) 
        {
            Debug.Log($"Hand Tracking Setup:");
            Debug.Log($"  Using: {(useRightHand ? "Right" : "Left")} Hand");
            Debug.Log($"  Hand Component: {(currentHand ? currentHand.name : "None")}");
        }
    }
    
    void Update() 
    {
        if (currentHand == null || !currentHand.IsTracked) return;
        
        HandleHandTrackingInput();
        UpdatePreviewCube();
        
        // Debug controls
        if (enableDebugLogs) 
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                LogHandTrackingStatus();
            }
            
            if (Input.GetKeyDown(KeyCode.P)) 
            {
                LogPinchStrength();
            }
        }
    }
    
    void HandleHandTrackingInput() 
    {
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
        startPosition = currentHand.transform.position;
        
        CreatePreviewCube();
        
        if (enableDebugLogs) 
        {
            Debug.Log($"Started creating zone with hand tracking at: {startPosition}");
        }
    }
    
    void CreatePreviewCube() 
    {
        if (previewCube != null) 
        {
            DestroyImmediate(previewCube);
        }
        
        previewCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        previewCube.name = "FocusZone_Preview";
        
        // Remove collider from preview
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
        if (!isCreatingZone || previewCube == null || currentHand == null) return;
        
        Vector3 currentPosition = currentHand.transform.position;
        
        // Calculate cube center and size
        Vector3 center = (startPosition + currentPosition) / 2f;
        Vector3 size = new Vector3(
            Mathf.Abs(currentPosition.x - startPosition.x),
            Mathf.Abs(currentPosition.y - startPosition.y),
            Mathf.Abs(currentPosition.z - startPosition.z)
        );
        
        // Apply minimum size
        size = Vector3.Max(size, Vector3.one * 0.1f);
        
        // Update preview cube
        previewCube.transform.position = center;
        previewCube.transform.localScale = size;
    }
    
    void FinishZoneCreation() 
    {
        if (!isCreatingZone || previewCube == null) return;
        
        isCreatingZone = false;
        
        CreateFinalZone();
        
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
    
    void CreateDefaultMaterials() 
    {
        if (previewMaterial == null) 
        {
            previewMaterial = new Material(Shader.Find("Standard"));
            previewMaterial.color = previewColor;
            previewMaterial.SetFloat("_Mode", 3);
            previewMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            previewMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            previewMaterial.SetInt("_ZWrite", 0);
            previewMaterial.EnableKeyword("_ALPHABLEND_ON");
            previewMaterial.renderQueue = 3000;
            
            Color transparentPreview = previewColor;
            transparentPreview.a = 0.3f;
            previewMaterial.color = transparentPreview;
        }
        
        if (finalMaterial == null) 
        {
            finalMaterial = new Material(Shader.Find("Standard"));
            finalMaterial.color = finalColor;
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
    
    void LogHandTrackingStatus() 
    {
        Debug.Log("=== HAND TRACKING STATUS ===");
        Debug.Log($"Current Hand: {(currentHand ? currentHand.name : "None")}");
        Debug.Log($"Hand Tracked: {(currentHand ? currentHand.IsTracked.ToString() : "No Hand")}");
        Debug.Log($"Hand Confidence: {(currentHand ? currentHand.HandConfidence.ToString() : "N/A")}");
        Debug.Log($"Creating Zone: {isCreatingZone}");
        Debug.Log($"Currently Pinching: {IsPinchingGesture()}");
    }
    
    void LogPinchStrength() 
    {
        if (currentHand != null && currentHand.IsTracked) 
        {
            float pinchStrength = currentHand.GetFingerPinchStrength(pinchFinger);
            Debug.Log($"Pinch Strength: {pinchStrength:F2} (Threshold: {pinchThreshold})");
            Debug.Log($"Is Pinching: {IsPinchingGesture()}");
        }
        else 
        {
            Debug.Log("Hand not tracked - cannot get pinch strength");
        }
    }
    
    // Public methods
    public void SwitchToRightHand() 
    {
        useRightHand = true;
        currentHand = rightHand;
        
        if (enableDebugLogs) 
        {
            Debug.Log("Switched to right hand");
        }
    }
    
    public void SwitchToLeftHand() 
    {
        useRightHand = false;
        currentHand = leftHand;
        
        if (enableDebugLogs) 
        {
            Debug.Log("Switched to left hand");
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
    
    public bool IsCurrentlyCreatingZone() 
    {
        return isCreatingZone;
    }
}
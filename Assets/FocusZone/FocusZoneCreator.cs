using UnityEngine;

public class FocusZoneCreator : MonoBehaviour 
{
    [Header("Input Settings")]
    public OVRInput.Button createZoneButton = OVRInput.Button.PrimaryIndexTrigger;
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    
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
    
    void Start() 
    {
        InitializeCreator();
    }
    
    void InitializeCreator() 
    {
        // Find controller transform 
        controllerTransform = FindControllerTransform();
        
        // materials 
        CreateDefaultMaterials();
        
        if (enableDebugLogs) 
        {
            Debug.Log("✓ Focus Zone Creator initialized");
            Debug.Log($"  Controller: {controller}");
            Debug.Log($"  Button: {createZoneButton}");
        }
    }
    
    Transform FindControllerTransform() 
    {
        // Try to find controller transform
        GameObject rightHand = GameObject.Find("RightHandAnchor");
        if (rightHand == null) rightHand = GameObject.Find("Right Controller");
        if (rightHand == null) rightHand = GameObject.Find("RightHand");
        
        if (rightHand != null) 
        {
            return rightHand.transform;
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
        if (controllerTransform == null) return;
        
        HandleZoneCreation();
        UpdatePreviewCube();
    }
    
    void HandleZoneCreation() 
    {
        // Check for button press to start zone creation
        if (OVRInput.GetDown(createZoneButton, controller) && !isCreatingZone) 
        {
            StartZoneCreation();
        }
        
        // Check for button release to finish zone creation
        if (OVRInput.GetUp(createZoneButton, controller) && isCreatingZone) 
        {
            FinishZoneCreation();
        }
    }
    
    void StartZoneCreation() 
    {
        isCreatingZone = true;
        startPosition = controllerTransform.position;
        
        // Create preview cube
        CreatePreviewCube();
        
        if (enableDebugLogs) 
        {
            Debug.Log($"Started creating zone at: {startPosition}");
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
        if (!isCreatingZone || previewCube == null || controllerTransform == null) return;
        
        Vector3 currentPosition = controllerTransform.position;
        
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
    
    // Public methods for external scripts
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
}
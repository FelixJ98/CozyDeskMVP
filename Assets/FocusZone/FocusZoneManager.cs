using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using uDesktopDuplication;

public class FocusZoneManager : MonoBehaviour 
{
    [Header("Focus Zone Settings")]
    public Material focusZoneMaterial;
    public Material previewMaterial;
    public Color focusZoneColor = new Color(0f, 1f, 0f, 0.3f);
    public Color previewColor = new Color(1f, 1f, 0f, 0.2f);
    
    [Header("Drawing Controls")]
    public InputActionReference drawStartAction;
    public InputActionReference drawEndAction;
    public LayerMask drawingSurface = 1; // Default layer
    
    [Header("Focus Detection")]
    public Transform playerHead; // Main Camera
    public Transform desktopDisplay; // Your desktop duplication object
    public float gazeDetectionAngle = 30f; // Degrees for "looking at" detection
    public float focusCheckInterval = 0.1f; // How often to check focus
    
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;
    public bool showGizmos = true;
    
    // Drawing state
    private bool isDrawing = false;
    private Vector3 drawStartPoint;
    private Vector3 drawEndPoint;
    private GameObject previewCube;
    private GameObject activeFocusZone;
    
    // Focus tracking
    private bool isPlayerInFocusZone = false;
    private bool isLookingAtScreen = false;
    private float lastFocusCheck = 0f;
    
    // Components
    private XRRayInteractor rayInteractor;
    private Camera playerCamera;
    
    void Start() 
    {
        SetupComponents();
        SetupInputActions();
        CreateMaterials();
    }
    
    void SetupComponents() 
    {
        // Find player head/camera if not assigned
        if (playerHead == null) 
        {
            playerHead = Camera.main?.transform;
            if (playerHead == null) 
            {
                playerHead = FindObjectOfType<Camera>()?.transform;
            }
        }
        
        playerCamera = playerHead?.GetComponent<Camera>();
        
        // Find desktop display if not assigned
        if (desktopDisplay == null) 
        {
            var desktopScript = FindObjectOfType<GrabbableDesktopDisplay>();
            if (desktopScript != null) 
            {
                desktopDisplay = desktopScript.transform;
            }
        }
        
        // Find ray interactor
        rayInteractor = FindObjectOfType<XRRayInteractor>();
        
        if (enableDebugLogs) 
        {
            Debug.Log($"Focus Zone Manager initialized. Player Head: {playerHead?.name}, Desktop: {desktopDisplay?.name}");
        }
    }
    
    void SetupInputActions() 
    {
        if (drawStartAction != null) 
        {
            drawStartAction.action.performed += OnDrawStart;
            drawStartAction.action.Enable();
        }
        
        if (drawEndAction != null) 
        {
            drawEndAction.action.performed += OnDrawEnd;
            drawEndAction.action.Enable();
        }
    }
    
    void CreateMaterials() 
    {
        if (focusZoneMaterial == null) 
        {
            focusZoneMaterial = CreateTransparentMaterial(focusZoneColor);
        }
        
        if (previewMaterial == null) 
        {
            previewMaterial = CreateTransparentMaterial(previewColor);
        }
    }
    
    Material CreateTransparentMaterial(Color color) 
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Surface", 1); // Transparent
        mat.SetFloat("_Blend", 0); // Alpha blend
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        mat.color = color;
        return mat;
    }
    
    void Update() 
    {
        HandleDrawingPreview();
        CheckFocusStatus();
    }
    
    void HandleDrawingPreview() 
    {
        if (isDrawing && rayInteractor != null) 
        {
            // Get current ray hit point
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) 
            {
                drawEndPoint = hit.point;
                UpdatePreviewCube();
            }
        }
    }
    
    void CheckFocusStatus() 
    {
        if (Time.time - lastFocusCheck < focusCheckInterval) return;
        lastFocusCheck = Time.time;
        
        // Check if player is in focus zone
        bool wasInZone = isPlayerInFocusZone;
        isPlayerInFocusZone = IsPlayerInFocusZone();
        
        if (isPlayerInFocusZone != wasInZone) 
        {
            if (isPlayerInFocusZone && enableDebugLogs) 
            {
                Debug.Log("Focus Ray - Player entered focus zone");
            } 
            else if (!isPlayerInFocusZone && enableDebugLogs) 
            {
                Debug.Log("Focus Ray - Player left focus zone");
            }
        }
        
        // Check if player is looking at screen
        bool wasLookingAtScreen = isLookingAtScreen;
        isLookingAtScreen = IsLookingAtScreen();
        
        if (isLookingAtScreen != wasLookingAtScreen) 
        {
            if (isLookingAtScreen && enableDebugLogs) 
            {
                Debug.Log("Looking at screen");
            }
        }
        
        // Combined focus state
        if (isPlayerInFocusZone && isLookingAtScreen && enableDebugLogs) 
        {
            Debug.Log("Focus Ray - Optimal focus state: In zone and looking at screen");
        }
    }
    
    void OnDrawStart(InputAction.CallbackContext context) 
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) 
        {
            drawStartPoint = hit.point;
            isDrawing = true;
            
            if (enableDebugLogs) 
            {
                Debug.Log($"Started drawing focus zone at {drawStartPoint}");
            }
        }
    }
    
    void OnDrawEnd(InputAction.CallbackContext context) 
    {
        if (isDrawing) 
        {
            isDrawing = false;
            CreateFocusZone();
            DestroyPreviewCube();
            
            if (enableDebugLogs) 
            {
                Debug.Log($"Finished drawing focus zone from {drawStartPoint} to {drawEndPoint}");
            }
        }
    }
    
    void UpdatePreviewCube() 
    {
        if (previewCube == null) 
        {
            previewCube = CreateCubeObject("FocusZone_Preview", previewMaterial);
        }
        
        // Calculate cube properties
        Vector3 center = (drawStartPoint + drawEndPoint) / 2f;
        Vector3 size = new Vector3(
            Mathf.Abs(drawEndPoint.x - drawStartPoint.x),
            Mathf.Abs(drawEndPoint.y - drawStartPoint.y),
            Mathf.Abs(drawEndPoint.z - drawStartPoint.z)
        );
        
        // Ensure minimum size
        size = Vector3.Max(size, Vector3.one * 0.1f);
        
        previewCube.transform.position = center;
        previewCube.transform.localScale = size;
    }
    
    void CreateFocusZone() 
    {
        // Destroy existing focus zone
        if (activeFocusZone != null) 
        {
            DestroyImmediate(activeFocusZone);
        }
        
        // Create new focus zone
        activeFocusZone = CreateCubeObject("FocusZone_Active", focusZoneMaterial);
        
        // Calculate final properties
        Vector3 center = (drawStartPoint + drawEndPoint) / 2f;
        Vector3 size = new Vector3(
            Mathf.Abs(drawEndPoint.x - drawStartPoint.x),
            Mathf.Abs(drawEndPoint.y - drawStartPoint.y),
            Mathf.Abs(drawEndPoint.z - drawStartPoint.z)
        );
        
        size = Vector3.Max(size, Vector3.one * 0.1f);
        
        activeFocusZone.transform.position = center;
        activeFocusZone.transform.localScale = size;
        
        // Add focus zone component
        var focusZone = activeFocusZone.AddComponent<FocusZone>();
        focusZone.Initialize(center, size);
    }
    
    GameObject CreateCubeObject(string name, Material material) 
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        
        // Remove collider for preview, keep for active zone
        if (name.Contains("Preview")) 
        {
            DestroyImmediate(cube.GetComponent<Collider>());
        }
        
        // Set material
        var renderer = cube.GetComponent<Renderer>();
        renderer.material = material;
        
        return cube;
    }
    
    void DestroyPreviewCube() 
    {
        if (previewCube != null) 
        {
            DestroyImmediate(previewCube);
            previewCube = null;
        }
    }
    
    bool IsPlayerInFocusZone() 
    {
        if (activeFocusZone == null || playerHead == null) return false;
        
        var focusZone = activeFocusZone.GetComponent<FocusZone>();
        if (focusZone != null) 
        {
            return focusZone.ContainsPoint(playerHead.position);
        }
        
        return false;
    }
    
    bool IsLookingAtScreen() 
    {
        if (playerHead == null || desktopDisplay == null) return false;
        
        Vector3 toScreen = (desktopDisplay.position - playerHead.position).normalized;
        Vector3 headForward = playerHead.forward;
        
        float angle = Vector3.Angle(headForward, toScreen);
        return angle <= gazeDetectionAngle;
    }
    
    void OnDrawGizmos() 
    {
        if (!showGizmos) return;
        
        // Draw gaze detection
        if (playerHead != null && desktopDisplay != null) 
        {
            Gizmos.color = isLookingAtScreen ? Color.green : Color.red;
            Gizmos.DrawWireSphere(playerHead.position, 0.1f);
            Gizmos.DrawLine(playerHead.position, desktopDisplay.position);
        }
        
        // Draw focus zone bounds
        if (activeFocusZone != null) 
        {
            Gizmos.color = isPlayerInFocusZone ? Color.green : Color.yellow;
            Gizmos.DrawWireCube(activeFocusZone.transform.position, activeFocusZone.transform.localScale);
        }
    }
    
    void OnDestroy() 
    {
        if (drawStartAction != null) 
        {
            drawStartAction.action.performed -= OnDrawStart;
        }
        
        if (drawEndAction != null) 
        {
            drawEndAction.action.performed -= OnDrawEnd;
        }
    }
}
using UnityEngine;
using Oculus.Interaction;

public class SimpleScale : MonoBehaviour 
{
    [Header("References")]
    public Transform desktopDisplay;
    
    [Header("Scaling Settings")]
    public float scaleSpeed = 1.0f;
    public float minScale = 0.3f;
    public float maxScale = 3.0f;
    public KeyCode testScaleUp = KeyCode.UpArrow;
    public KeyCode testScaleDown = KeyCode.DownArrow;
    
    private Grabbable grabbable;
    private Vector3 currentDesiredScale;
    private Vector3 lastAppliedScale;
    private bool isGrabbed = false;
    private bool scaleChanged = false;
    
    void Start() 
    {
        grabbable = GetComponent<Grabbable>();
        if (desktopDisplay == null) 
        {
            desktopDisplay = GetComponentInChildren<Renderer>().transform;
        }
        
        // Store initial scale as current desired scale
        currentDesiredScale = desktopDisplay.localScale;
        lastAppliedScale = currentDesiredScale;
        
        Debug.Log($"✓ Persistent Desktop Scaler initialized with scale: {currentDesiredScale}");
    }
    
    void Update() 
    {
        TestKeyboardScaling();
        CheckGrabStatus();
        
        // Handle two-hand scaling
        if (grabbable.SelectingPointsCount >= 2) 
        {
            HandleTwoHandScaling();
        }
        
        // Always enforce the desired scale
        EnforceDesiredScale();
    }
    
    void EnforceDesiredScale() 
    {
        // Check if scale has been changed by external forces
        if (Vector3.Distance(desktopDisplay.localScale, currentDesiredScale) > 0.01f) 
        {
            // Someone else changed the scale, restore our desired scale
            desktopDisplay.localScale = Vector3.Lerp(
                desktopDisplay.localScale, 
                currentDesiredScale, 
                Time.deltaTime * 10f // Fast restoration
            );
            
            if (Vector3.Distance(lastAppliedScale, currentDesiredScale) > 0.01f) 
            {
                Debug.Log($"Enforcing scale: {currentDesiredScale} (was {desktopDisplay.localScale})");
                lastAppliedScale = currentDesiredScale;
            }
        }
    }
    
    void CheckGrabStatus() 
    {
        bool currentlyGrabbed = grabbable.SelectingPointsCount > 0;
        if (currentlyGrabbed != isGrabbed) 
        {
            isGrabbed = currentlyGrabbed;
            Debug.Log($"Grab status: {(isGrabbed ? "GRABBED" : "RELEASED")} - Maintaining scale: {currentDesiredScale}");
            
            // When grab state changes, ensure scale is maintained
            if (isGrabbed) 
            {
                // Just grabbed - store current scale as desired
                currentDesiredScale = desktopDisplay.localScale;
            }
        }
    }
    
    void TestKeyboardScaling() 
    {
        if (Input.GetKey(testScaleUp)) 
        {
            SetDesiredScale(currentDesiredScale * 1.02f);
        }
        
        if (Input.GetKey(testScaleDown)) 
        {
            SetDesiredScale(currentDesiredScale * 0.98f);
        }
    }
    
    void HandleTwoHandScaling() 
    {
        var grabPoints = grabbable.GrabPoints;
        if (grabPoints.Count >= 2) 
        {
            float currentDistance = Vector3.Distance(
                grabPoints[0].position, 
                grabPoints[1].position
            );
            
            // Calculate scale based on distance (you may need to adjust this multiplier)
            float scaleMultiplier = currentDistance / 0.5f; // 0.5f is reference distance
            float targetScale = Mathf.Clamp(scaleMultiplier, minScale, maxScale);
            
            SetDesiredScale(Vector3.one * targetScale);
        }
    }
    
    void SetDesiredScale(Vector3 newScale) 
    {
        // Clamp the scale
        newScale = Vector3.Max(newScale, Vector3.one * minScale);
        newScale = Vector3.Min(newScale, Vector3.one * maxScale);
        
        if (Vector3.Distance(currentDesiredScale, newScale) > 0.01f) 
        {
            currentDesiredScale = newScale;
            Debug.Log($"New desired scale set: {currentDesiredScale}");
        }
    }
}
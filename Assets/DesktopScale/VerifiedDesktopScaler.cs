using UnityEngine;
using Oculus.Interaction;

public class VerifiedDesktopScaler : MonoBehaviour 
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
    private Vector3 initialScale;
    private bool isGrabbed = false;
    
    void Start() 
    {
        grabbable = GetComponent<Grabbable>();
        if (grabbable == null) 
        {
            Debug.LogError("No Grabbable component found!");
            return;
        }
        
        if (desktopDisplay == null) 
        {
            desktopDisplay = GetComponentInChildren<Renderer>().transform;
        }
        
        initialScale = desktopDisplay.localScale;
        
        // Test initial setup
        Debug.Log($"✓ Desktop Scaler initialized");
        Debug.Log($"✓ Initial scale: {initialScale}");
        Debug.Log($"✓ Press {testScaleUp}/{testScaleDown} to test scaling");
    }
    
    void Update() 
    {
        // Keyboard testing 
        TestKeyboardScaling();
        
        bool currentlyGrabbed = grabbable.SelectingPointsCount > 0;
        if (currentlyGrabbed != isGrabbed) 
        {
            isGrabbed = currentlyGrabbed;
            Debug.Log($"Grab status changed: {(isGrabbed ? "GRABBED" : "RELEASED")}");
            Debug.Log($"Grab points: {grabbable.SelectingPointsCount}");
        }
        
        // Handle two-hand scaling
        if (grabbable.SelectingPointsCount >= 2) 
        {
            HandleTwoHandScaling();
        }
    }
    
    void TestKeyboardScaling() 
    {
       
        if (Input.GetKey(testScaleUp)) 
        {
            ScaleDesktop(1.02f); // 2% larger per frame
            Debug.Log($"Scale up test - Current scale: {desktopDisplay.localScale.x:F2}");
        }
        
        if (Input.GetKey(testScaleDown)) 
        {
            ScaleDesktop(0.98f); // 2% smaller per frame
            Debug.Log($"Scale down test - Current scale: {desktopDisplay.localScale.x:F2}");
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
            
            // Use distance for scaling
            float targetScale = Mathf.Clamp(currentDistance * 2f, minScale, maxScale);
            Vector3 newScale = Vector3.one * targetScale;
            
            desktopDisplay.localScale = Vector3.Lerp(
                desktopDisplay.localScale, 
                newScale, 
                Time.deltaTime * scaleSpeed
            );
            
            Debug.Log($"Two-hand scaling: Distance={currentDistance:F2}, Scale={targetScale:F2}");
        }
    }
    
    void ScaleDesktop(float multiplier) 
    {
        Vector3 newScale = desktopDisplay.localScale * multiplier;
        newScale = Vector3.ClampMagnitude(newScale, maxScale);
        if (newScale.magnitude < minScale) 
        {
            newScale = Vector3.one * minScale;
        }
        
        desktopDisplay.localScale = newScale;
    }
}
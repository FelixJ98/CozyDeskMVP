using UnityEngine;
using Oculus.Interaction;
using System.Collections.Generic;
using Oculus.Interaction.HandGrab;

public class DesktopScaler : MonoBehaviour 
{
    [Header("Desktop References")]
    public Transform desktopDisplay;
    public DistanceHandGrabInteractable grabInteractable;
    
    [Header("Scaling Settings")]
    public float minScale = 0.3f;
    public float maxScale = 5.0f;
    public bool maintainAspectRatio = true;
    
    private Vector3 initialScale;
    private Dictionary<int, Vector3> grabPositions = new Dictionary<int, Vector3>();
    private float initialDistance;
    private bool isScaling = false;
    
    void Start() 
    {
        if (desktopDisplay == null) 
        {
            desktopDisplay = GetComponentInChildren<Renderer>().transform;
        }
        
        initialScale = desktopDisplay.localScale;
        
        // Subscribe to grab events
        if (grabInteractable != null) 
        {
            grabInteractable.WhenPointerEventRaised += HandleGrabEvent;
        }
    }
    
    void HandleGrabEvent(PointerEvent evt) 
    {
        switch (evt.Type) 
        {
            case PointerEventType.Select:
                AddGrabPoint(evt.Identifier, evt.Pose.position);
                break;
                
            case PointerEventType.Unselect:
                RemoveGrabPoint(evt.Identifier);
                break;
                
            case PointerEventType.Move:
                UpdateGrabPoint(evt.Identifier, evt.Pose.position);
                break;
        }
        
        UpdateScaling();
    }
    
    void AddGrabPoint(int id, Vector3 position) 
    {
        grabPositions[id] = position;
        
        if (grabPositions.Count == 2) 
        {
            // Start scaling with two hands
            isScaling = true;
            CalculateInitialDistance();
        }
    }
    
    void RemoveGrabPoint(int id) 
    {
        grabPositions.Remove(id);
        
        if (grabPositions.Count < 2) 
        {
            isScaling = false;
        }
    }
    
    void UpdateGrabPoint(int id, Vector3 position) 
    {
        if (grabPositions.ContainsKey(id)) 
        {
            grabPositions[id] = position;
        }
    }
    
    void CalculateInitialDistance() 
    {
        if (grabPositions.Count != 2) return;
        
        var positions = new List<Vector3>(grabPositions.Values);
        initialDistance = Vector3.Distance(positions[0], positions[1]);
    }
    
    void UpdateScaling() 
    {
        if (!isScaling || grabPositions.Count != 2) return;
        
        var positions = new List<Vector3>(grabPositions.Values);
        float currentDistance = Vector3.Distance(positions[0], positions[1]);
        
        if (initialDistance > 0) 
        {
            float scaleMultiplier = currentDistance / initialDistance;
            Vector3 newScale = initialScale * scaleMultiplier;
            
            // Apply constraints
            newScale = ConstrainScale(newScale);
            
            // Apply to desktop display
            desktopDisplay.localScale = newScale;
        }
    }
    
    Vector3 ConstrainScale(Vector3 scale) 
    {
        if (maintainAspectRatio) 
        {
            // Use the average scale for uniform scaling
            float avgScale = (scale.x + scale.y + scale.z) / 3f;
            avgScale = Mathf.Clamp(avgScale, minScale, maxScale);
            return Vector3.one * avgScale;
        } 
        else 
        {
            // Constrain each axis independently
            scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
            scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
            scale.z = Mathf.Clamp(scale.z, minScale, maxScale);
            return scale;
        }
    }
    
    void OnDestroy() 
    {
        if (grabInteractable != null) 
        {
            grabInteractable.WhenPointerEventRaised -= HandleGrabEvent;
        }
    }
}
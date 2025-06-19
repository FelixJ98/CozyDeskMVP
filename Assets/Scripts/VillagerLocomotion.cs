using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerLocomotion : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast the object moves in meters per second")]
    public float movementSpeed = 0.1f;
    
    [Tooltip("Maximum angle of random turns in degrees")]
    [Range(0, 180)]
    public float maxTurnAngle = 30f;
    
    [Tooltip("How frequently the object changes direction (in seconds)")]
    public float directionChangeInterval = 2f;
    
    [Header("Edge Detection")]
    [Tooltip("Length of the edge detection ray")]
    public float raycastLength = 0.2f;
    
    [Tooltip("Angle of the downward raycast in degrees")]
    [Range(-90, 90)]
    public float raycastAngle = 45f;
    
    [Tooltip("Layers to detect for surface")]
    public LayerMask surfaceLayers = -1; // Default to all layers
    
    [Header("Debugging")]
    public bool showDebugRays = true;
    public Color validSurfaceRayColor = Color.green;
    public Color edgeDetectedRayColor = Color.red;
    
    // Private variables
    private Vector3 currentDirection;
    private float timer;
    private bool edgeDetected = false;
    private Transform raycastOrigin;
    
    private void Start()
    {
        // Create an empty GameObject slightly in front of this object to cast rays from
        raycastOrigin = new GameObject("RaycastOrigin").transform;
        raycastOrigin.parent = transform;
        raycastOrigin.localPosition = new Vector3(0, 0, 0.05f); // Slightly in front
        
        // Initialize with a random direction
        ChangeDirection();
        
        // Start the timer
        timer = directionChangeInterval;
    }
    
    private void Update()
    {
        // Check for edge
        CheckEdge();
        
        // Move the object
        if (edgeDetected)
        {
            // Edge detected - turn left until we find surface again
            transform.Rotate(0, -90 * Time.deltaTime, 0);
        }
        else
        {
            // Move forward
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime, Space.Self);
            
            // Occasionally change direction if not at an edge
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ChangeDirection();
                timer = directionChangeInterval;
            }
        }
    }
    
    private void CheckEdge()
    {
        // Calculate direction for the angled downward ray
        Vector3 rayDirection = Quaternion.Euler(-raycastAngle, 0, 0) * transform.forward;
        
        // Cast the ray from slightly in front of the object
        bool hitSurface = Physics.Raycast(raycastOrigin.position, rayDirection, raycastLength, surfaceLayers);
        
        // Update edge detected status
        edgeDetected = !hitSurface;
        
        // Debug visualization
        if (showDebugRays)
        {
            Debug.DrawRay(raycastOrigin.position, rayDirection * raycastLength, 
                hitSurface ? validSurfaceRayColor : edgeDetectedRayColor);
        }
    }
    
    private void ChangeDirection()
    {
        // Generate a random rotation within the max turn angle
        float randomAngle = Random.Range(-maxTurnAngle, maxTurnAngle);
        transform.Rotate(0, randomAngle, 0);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw the path projection when selected in editor
        if (Application.isPlaying)
            return;
            
        Gizmos.color = Color.yellow;
        Vector3 rayOrigin = transform.position + transform.forward * 0.05f;
        Vector3 rayDirection = Quaternion.Euler(-raycastAngle, 0, 0) * transform.forward;
        Gizmos.DrawRay(rayOrigin, rayDirection * raycastLength);
    }
}
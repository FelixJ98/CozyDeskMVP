using UnityEngine;
using Oculus.Interaction;

public class CornerGrabPoints : MonoBehaviour 
{
    [Header("Desktop Display Settings")]
    public Transform desktopDisplay; // desktop streamer
    public Material cornerMaterial; // Transparent corners
    
    [Header("Corner Grab Settings")]
    public float cornerSize = 0.1f;
    public bool showCornerVisuals = true; // debugging
    
    private GameObject[] cornerGrabPoints;
    private Renderer desktopRenderer;
    
    void Start() 
    {
        if (desktopDisplay == null) 
        {
            desktopDisplay = GetComponentInChildren<Renderer>().transform;
        }
        
        desktopRenderer = desktopDisplay.GetComponent<Renderer>();
        CreateCornerGrabPoints();
    }
    
    void CreateCornerGrabPoints() 
    {
        if (desktopRenderer == null) return;
        
        cornerGrabPoints = new GameObject[4];
        Vector3 bounds = desktopRenderer.bounds.size;
        
        // Corner positions relative to desktop display
        Vector3[] cornerPositions = new Vector3[]
        {
            new Vector3(-bounds.x/2, bounds.y/2, 0),   // Top-left
            new Vector3(bounds.x/2, bounds.y/2, 0),    // Top-right
            new Vector3(-bounds.x/2, -bounds.y/2, 0),  // Bottom-left
            new Vector3(bounds.x/2, -bounds.y/2, 0)    // Bottom-right
        };
        
        for (int i = 0; i < 4; i++) 
        {
            CreateCornerGrabPoint(i, cornerPositions[i]);
        }
    }
    
    void CreateCornerGrabPoint(int index, Vector3 localPosition) 
    {
        // corner grab point
        GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Cube);
        corner.name = $"CornerGrab_{index}";
        corner.transform.SetParent(this.transform);
        corner.transform.localPosition = localPosition;
        corner.transform.localScale = Vector3.one * cornerSize;
        
        
        var cornerCollider = corner.GetComponent<Collider>();
        cornerCollider.isTrigger = false;
        
        
        var renderer = corner.GetComponent<Renderer>();
        if (cornerMaterial != null) 
        {
            renderer.material = cornerMaterial;
        }
        
        
        if (!showCornerVisuals) 
        {
            renderer.enabled = false;
        }
        
        cornerGrabPoints[index] = corner;
    }
    
    void Update() 
    {
        
        UpdateCornerPositions();
    }
    
    void UpdateCornerPositions() 
    {
        if (desktopRenderer == null || cornerGrabPoints == null) return;
        
        Vector3 bounds = desktopRenderer.bounds.size;
        Vector3[] cornerPositions = new Vector3[]
        {
            new Vector3(-bounds.x/2, bounds.y/2, 0),
            new Vector3(bounds.x/2, bounds.y/2, 0),
            new Vector3(-bounds.x/2, -bounds.y/2, 0),
            new Vector3(bounds.x/2, -bounds.y/2, 0)
        };
        
        for (int i = 0; i < cornerGrabPoints.Length; i++) 
        {
            if (cornerGrabPoints[i] != null) 
            {
                cornerGrabPoints[i].transform.localPosition = cornerPositions[i];
            }
        }
    }
}
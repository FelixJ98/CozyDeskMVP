using UnityEngine;

public class FocusZone : MonoBehaviour 
{
    [Header("Zone Properties")]
    public Vector3 zoneBounds;
    public Vector3 zoneCenter;
    
    [Header("Visual Feedback")]
    public bool pulseWhenActive = true;
    public float pulseSpeed = 2f;
    public float pulseIntensity = 0.3f;
    
    private Renderer zoneRenderer;
    private Material zoneMaterial;
    private Color originalColor;
    private bool isPlayerInside = false;
    
    public void Initialize(Vector3 center, Vector3 bounds) 
    {
        zoneCenter = center;
        zoneBounds = bounds;
        
        zoneRenderer = GetComponent<Renderer>();
        if (zoneRenderer != null) 
        {
            zoneMaterial = zoneRenderer.material;
            originalColor = zoneMaterial.color;
        }
    }
    
    void Update() 
    {
        if (pulseWhenActive && isPlayerInside) 
        {
            UpdatePulseEffect();
        }
    }
    
    void UpdatePulseEffect() 
    {
        if (zoneMaterial == null) return;
        
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        Color pulseColor = originalColor + Color.white * pulse;
        pulseColor.a = originalColor.a; // Maintain alpha
        
        zoneMaterial.color = pulseColor;
    }
    
    public bool ContainsPoint(Vector3 point) 
    {
        Vector3 localPoint = point - zoneCenter;
        
        bool inside = Mathf.Abs(localPoint.x) <= zoneBounds.x / 2f &&
                     Mathf.Abs(localPoint.y) <= zoneBounds.y / 2f &&
                     Mathf.Abs(localPoint.z) <= zoneBounds.z / 2f;
        
        if (inside != isPlayerInside) 
        {
            isPlayerInside = inside;
            OnPlayerEnterExit(inside);
        }
        
        return inside;
    }
    
    void OnPlayerEnterExit(bool entered) 
    {
        if (entered) 
        {
            // Player entered zone
            if (zoneMaterial != null) 
            {
                zoneMaterial.color = originalColor * 1.5f; // Brighten
            }
        } 
        else 
        {
            // Player exited zone
            if (zoneMaterial != null) 
            {
                zoneMaterial.color = originalColor;
            }
        }
    }
    
    void OnDrawGizmos() 
    {
        Gizmos.color = isPlayerInside ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(zoneCenter, zoneBounds);
    }
}
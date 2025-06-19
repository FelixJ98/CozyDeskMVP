using UnityEngine;

public class RoomLightDim : MonoBehaviour
{
    [Header("Passthrough Settings")]
    public OVRPassthroughLayer passthroughLayer;
    public float normalBrightness = 0.0f;
    public float dimmedBrightness = -0.8f;
    public float contrast = 0.0f;
    public float saturation = 0.0f;
    
    private bool isDimmed = true;
    
    void Start()
    {
        if (passthroughLayer == null)
        {
            passthroughLayer = FindObjectOfType<OVRPassthroughLayer>();
        }
        
        if (passthroughLayer != null)
        {
            passthroughLayer.SetBrightnessContrastSaturation(normalBrightness, contrast, saturation);
        }
    }
    
    public void ToggleDimming()
    {
        isDimmed = !isDimmed;
        
        if (passthroughLayer != null)
        {
            float brightness = isDimmed ? dimmedBrightness : normalBrightness;
            passthroughLayer.SetBrightnessContrastSaturation(brightness, contrast, saturation);
        }
        
        Debug.Log($"Passthrough dimming: {(isDimmed ? "ON" : "OFF")} - Brightness: {(isDimmed ? dimmedBrightness : normalBrightness)}");
    }
    
    public void DimPassthrough()
    {
        isDimmed = true;
        if (passthroughLayer != null)
        {
            passthroughLayer.SetBrightnessContrastSaturation(dimmedBrightness, contrast, saturation);
        }
    }
    
    public void BrightenPassthrough()
    {
        isDimmed = false;
        if (passthroughLayer != null)
        {
            passthroughLayer.SetBrightnessContrastSaturation(normalBrightness, contrast, saturation);
        }
    }
}
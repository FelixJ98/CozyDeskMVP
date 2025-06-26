using UnityEngine;

public class HandTrackingDebugger : MonoBehaviour 
{
    public OVRHand testHand; // Drag right hand here
    
    void Update() 
    {
        if (testHand == null) return;
        
        // Continuous debug info
        if (Input.GetKey(KeyCode.H)) 
        {
            Debug.Log($"=== HAND DEBUG ===");
            Debug.Log($"Hand Tracked: {testHand.IsTracked}");
            Debug.Log($"Hand Confidence: {testHand.HandConfidence}");
            Debug.Log($"Index Pinch: {testHand.GetFingerPinchStrength(OVRHand.HandFinger.Index):F2}");
            Debug.Log($"Hand Position: {testHand.transform.position}");
        }
        
        // Visual feedback
        if (testHand.IsTracked) 
        {
            float pinchStrength = testHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
            if (pinchStrength > 0.8f) 
            {
                Debug.Log($"PINCHING! Strength: {pinchStrength:F2}");
            }
        }
    }
}
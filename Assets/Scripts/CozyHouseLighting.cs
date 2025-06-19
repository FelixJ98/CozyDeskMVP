using UnityEngine;

public class CozyHouseLighting : MonoBehaviour
{
    [SerializeField] private Renderer targetMesh;
    [SerializeField] private Material ogMat;
    [SerializeField] private Material newMat;
    
    private bool isChanged = false;
    
    public void ToggleMaterial()
    {
        if (isChanged)
        {
            targetMesh.material = ogMat;
        }
        else
        {
            targetMesh.material = newMat;
        }
        
        isChanged = !isChanged;
    }
}

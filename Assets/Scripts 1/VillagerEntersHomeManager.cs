using UnityEngine;

public class VillagerEntersHomeManager : MonoBehaviour
{
    //[SerializeField] private GameObject targetObject;
    [SerializeField] private myCozyHouseLighting materialChanger;
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger hit by: {other.gameObject.name}, Tag: {other.tag}");
        if (other.CompareTag("NPC"))
        {
            if (other.attachedRigidbody == null)
            {
                Debug.LogWarning($"NPC '{other.gameObject.name}' entered trigger but has no attached Rigidbody!");
            }
            else if (other.attachedRigidbody.gameObject.activeSelf)
            {
                Debug.Log("Tag matched! Deactivating and changing material.");
                other.attachedRigidbody.gameObject.SetActive(false);

                if (materialChanger != null)
                {
                    materialChanger.ToggleMaterial();
                }
                else
                {
                    Debug.LogWarning("materialChanger is not assigned in inspector!");
                }
            }
        }
    }
}
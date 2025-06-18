using UnityEngine;

public class VillagerEntersHome : MonoBehaviour
{
    //[SerializeField] private GameObject targetObject;
    [SerializeField] private CozyHouseLighting materialChanger;
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger hit by: {other.gameObject.name}, Tag: {other.tag}");
        
        if (other.CompareTag("Villager") && other.attachedRigidbody.gameObject.activeSelf) // or whatever tag you want to use
        {
            Debug.Log("Tag matched! Deactivating and changing material.");
            other.attachedRigidbody.gameObject.SetActive(false);
            //targetObject.SetActive(false);
            materialChanger.ToggleMaterial();
        }
    }
}

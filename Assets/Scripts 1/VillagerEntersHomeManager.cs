using System.Collections;
using UnityEngine;

public class VillagerEntersHomeManager : MonoBehaviour
{
    [SerializeField] private myCozyHouseLighting materialChanger;
    [SerializeField] private Transform exitPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            Rigidbody rb = other.attachedRigidbody;

            if (rb == null)
            {
                Debug.LogWarning($"NPC '{other.gameObject.name}' entered trigger but has no attached Rigidbody!");
                return;
            }

            locomotion motion = rb.GetComponent<locomotion>();
            if (motion == null)
            {
                Debug.LogWarning($"NPC '{rb.gameObject.name}' missing locomotion script!");
                return;
            }

            // Only allow entering if NPC has no house assigned yet
            if (!motion.hasHouse)
            {
                // Assign this house as their home
                motion.AssignHouse(transform);

                Debug.Log($"Assigned house '{transform.name}' to NPC '{rb.gameObject.name}'");

                // Now "enter" the house: deactivate and toggle materials
                rb.gameObject.SetActive(false);

                if (materialChanger != null)
                {
                    materialChanger.ToggleMaterial();
                }
                else
                {
                    Debug.LogWarning("materialChanger is not assigned!");
                }

                StartCoroutine(ReactivateVillager(rb.gameObject));
            }
            else
            {
                Debug.Log($"NPC '{rb.gameObject.name}' already has a house, ignoring entry.");
            }
        }
    }

    IEnumerator ReactivateVillager(GameObject villager)
    {
        villager.transform.position = exitPoint.position;
        yield return new WaitForSeconds(3f);
        villager.SetActive(true);
        Debug.Log($"Villager {villager.name} has exited the house!");

        locomotion motion = villager.GetComponent<locomotion>();
        if (motion != null)
        {
            motion.StartCoroutine(motion.Routine());
        }
        else
        {
            Debug.LogWarning($"Villager {villager.name} is missing a locomotion script!");
        }
    }
}

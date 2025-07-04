using System;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class InitializeObjects : MonoBehaviour
{
    private LazyCoinManager lcm;
    private locomotion motion;
    
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject libraryPrefab;
    [SerializeField] private GameObject housePrefab;
    [SerializeField] private GameObject bowlingPrefab;
    [SerializeField] private GameObject villagerPrefab;
    [SerializeField] private GameObject villagerPrefab2;
    
    private void Start()
    {
        lcm = FindObjectOfType<LazyCoinManager>();
        motion = GetComponent<locomotion>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitializeVillager()
    {
        // Choose random prefab
        GameObject prefabToSpawn = Random.Range(0, 2) == 0 ? villagerPrefab : villagerPrefab2;
    
        var villagerInstance = Instantiate(prefabToSpawn, GetSpawnPosition(), Quaternion.identity);
        lcm.RegisterNPC();
    }
    
    public void InitializeHouse()
    {
        if (lcm.coinValue > 0)
        {
            lcm.coinValue -= 10;
            if (housePrefab == null)
            {
                Debug.LogError("❌ housePrefab is not assigned!");
                return;
            }

            if (lcm == null)
            {
                Debug.LogError("❌ LazyCoinManager not found!");
                return;
            }

            var houseInstance = Instantiate(housePrefab, GetSpawnPosition(), Quaternion.identity);
            lcm.RegisterHouse();

            // Find locomotion at this point
            locomotion motionInstance = FindObjectOfType<locomotion>();
            if (motionInstance != null)
            {
                motionInstance.AddNode(houseInstance.transform);
            }
            else
            {
                Debug.LogWarning("⚠ No locomotion instance found at runtime when trying to add node!");
            }
        }
    }

    public void InitializeLibrary()
    {
        if (lcm.coinValue > 30)
        {
            lcm.coinValue -= 100;
            var libraryInstance = Instantiate(libraryPrefab, GetSpawnPosition(), Quaternion.identity);
            lcm.RegisterLibrary();
            // Find locomotion at this point
            locomotion motionInstance = FindObjectOfType<locomotion>();
            if (motionInstance != null)
            {
                motionInstance.AddNode(transform);
            }
            else
            {
                Debug.LogWarning("⚠ No locomotion instance found at runtime when trying to add node!");
            }
        }
   
    }

    /*
    public void InitializeLibrary()
    {
        if (lcm.coinValue > 0)
        {
            lcm.coinValue -= 100;
            if (libraryPrefab == null)
            {
                Debug.LogError("libraryPrefab is not assigned!");
                return;
            }

            if (lcm == null)
            {
                Debug.LogError("LazyCoinManager not found!");
                return;
            }

            if (motion == null)
            {
                Debug.LogError("locomotion not found on this object!");
                return;
            }

            var libraryInstance = Instantiate(libraryPrefab, GetSpawnPosition(), Quaternion.identity);
            lcm.RegisterLibrary();
            // Find locomotion at this point
            locomotion motionInstance = FindObjectOfType<locomotion>();
            if (motionInstance != null)
            {
                motionInstance.AddNode(transform);
            }
            else
            {
                Debug.LogWarning("⚠ No locomotion instance found at runtime when trying to add node!");
            }
        }
    }
    */

    /*
    public void InitializeTheater()
    {
        var theaterInstance = Instantiate(theaterPrefab, GetSpawnPosition(), Quaternion.identity);
        lcm.RegisterTheater();
        motion.AddNode(transform);
    }
    */
    public void InitializeBowling()
    {
        if (lcm.coinValue > 30)
        {
            lcm.coinValue -= 1000;
            var bowlingInstance = Instantiate(bowlingPrefab, GetSpawnPosition(), Quaternion.identity);
            lcm.RegisterBowling();
            // Find locomotion at this point
            locomotion motionInstance = FindObjectOfType<locomotion>();
            if (motionInstance != null)
            {
                motionInstance.AddNode(transform);
            }
            else
            {
                Debug.LogWarning("⚠ No locomotion instance found at runtime when trying to add node!");
            }
        }
       
    }
    
    private Vector3 GetSpawnPosition()
    {
        // Spawn a few units in front of this object (e.g. 3 units forward)
        return spawnPoint.position;
    }
}

using TreeEditor;
using UnityEngine;
public class InitializeObjects : MonoBehaviour
{
    private LazyCoinManager lcm;
    private locomotion motion;
    
    [SerializeField] private GameObject libraryPrefab;
    [SerializeField] private GameObject housePrefab;
    [SerializeField] private GameObject bowlingPrefab;
    [SerializeField] private GameObject theaterPrefab;
    [SerializeField] private GameObject villagerPrefab;
    
    private void Start()
    {
        lcm = FindObjectOfType<LazyCoinManager>();
        motion = GetComponent<locomotion>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitializeVillager()
    {
        var villagerInstance = Instantiate(villagerPrefab, GetSpawnPosition(), Quaternion.identity);
        lcm.RegisterNPC();
    }
    
    public void InitializeHouse()
    {
        var houseInstance = Instantiate(housePrefab, GetSpawnPosition(), Quaternion.identity);
        lcm.RegisterHouse();
        motion.AddNode(transform);
    }
    
    public void InitializeLibrary()
    {
        var libraryInstance = Instantiate(libraryPrefab, GetSpawnPosition(), Quaternion.identity);
        lcm.RegisterLibrary();
        motion.AddNode(transform);
    }
    
    public void InitializeTheater()
    {
        var theaterInstance = Instantiate(theaterPrefab, GetSpawnPosition(), Quaternion.identity);
        lcm.RegisterTheater();
        motion.AddNode(transform);
    }
    
    public void InitializeBowling()
    {
        var bowlingInstance = Instantiate(bowlingPrefab, GetSpawnPosition(), Quaternion.identity);
        lcm.RegisterBowling();
        motion.AddNode(transform);
    }
    
    private Vector3 GetSpawnPosition()
    {
        // Spawn a few units in front of this object (e.g. 3 units forward)
        return transform.position + transform.forward * 3f;
    }
}

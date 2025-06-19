using System.Collections;
using System.Collections.Generic;
using Meta.XR.MultiplayerBlocks.Fusion.Editor;
using UnityEngine;

public class locomotion : MonoBehaviour
{
    private int Timer;

    public List<Transform> waypoints = new List<Transform>();
    public float speed = 0;
    private static System.Random sharedRNG = new System.Random(12345); // Shared, seeded RNG
    private System.Random movementRNG;
    private int currentWaypointIndex = 0;
    private bool isChatting = false;

    public Transform assignedHouse;
    private bool hasHouse = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    /*
     * Make sure to use the ADDNode() func when you initialize a house as this will add it to the list of houses to tra-
     * vel
     * Make sure to use the GoHome func when the pomodoro timer is done
     * 
     */
    void Awake()
    {
        movementRNG = new System.Random(GetInstanceID());
    }
    void Start()
    {
        GameObject[] houseNodes = GameObject.FindGameObjectsWithTag("HouseNode");
        waypoints = new List<Transform>();
        foreach (GameObject node in houseNodes)
        {
            waypoints.Add(node.transform);
        }
        StartCoroutine(Routine());
    }
    
    public void AssignHouse(Transform house)
    {
        assignedHouse = house;
        hasHouse = true;

        currentWaypointIndex = waypoints.IndexOf(house);
        if (currentWaypointIndex == -1)
        {
            Debug.Log($"House node {house.name} was NOT found in waypoints list!");
        }
        else
        {
            StartCoroutine(Routine());
        }
    }

    public void GoHome()
    {
        if (!hasHouse) return;

        currentWaypointIndex = waypoints.IndexOf(assignedHouse);
        if (currentWaypointIndex != -1)
        {
            
        }
        else
        {
            Debug.LogWarning($"Assigned house node {assignedHouse.name} not found in waypoints.");
        }
    }
    IEnumerator Routine()
    {
        while (true)
        {
            if (!isChatting && speed > 0f)
            {
                Transform target = waypoints[currentWaypointIndex];
                while (Vector3.Distance(transform.position, target.position) > 0.1f && !isChatting)
                {
                    transform.position =
                        Vector3.MoveTowards(transform.position, target.position, speed);
                    yield return null;
                }

                if (!isChatting)
                {
                    yield return new WaitForSeconds(1f);
                    
                    int newIndex;
                    do
                    {
                        newIndex = movementRNG.Next(0, waypoints.Count);
                    } while (newIndex == currentWaypointIndex);

                    currentWaypointIndex = newIndex;
                }

            }
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") && !isChatting)
        { 
            if (sharedRNG.NextDouble() < 0.9f)
            {
                StartCoroutine(ChatRoutine());
            }
        }
    }

    IEnumerator ChatRoutine()
    {
        isChatting = true;
        
        Debug.Log($"{gameObject.name} chatting...");

        yield return new WaitForSeconds(5f);
        Debug.Log($"Stopping for 3 seconds...");
        isChatting = false;
    }

    public void AddNode(Transform node)
    {
        waypoints.Add(node);
    }
}
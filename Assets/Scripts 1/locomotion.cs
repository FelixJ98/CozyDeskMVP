using System.Collections;
using Meta.XR.MultiplayerBlocks.Fusion.Editor;
using UnityEngine;

public class locomotion : MonoBehaviour
{
    private int Timer;

    public Transform[] waypoints;
    public float speed = 0;
    private static System.Random sharedRNG = new System.Random(12345); // Shared, seeded RNG
    private int currentWaypointIndex = 0;
    private bool isChatting = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Routine());
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
                        newIndex = Random.Range(0, waypoints.Length);
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
}
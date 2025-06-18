using System.Collections;
using Meta.XR.MultiplayerBlocks.Fusion.Editor;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private int Timer;

    public Transform[] waypoints;
    public float speed = 0.0009f;

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
            if (!isChatting)
            {
                Transform target = waypoints[currentWaypointIndex];
                while (Vector3.Distance(transform.position, target.position) > 0.1f)
                {
                    transform.position =
                        Vector3.MoveTowards(transform.position, target.position, 0.009f);
                    yield return null;
                }

                yield return new WaitForSeconds(1f);

                int newIndex;
                do
                {
                    newIndex = Random.Range(0, waypoints.Length);
                } while (newIndex == currentWaypointIndex);

                currentWaypointIndex = newIndex;
            }

            yield return null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC") && !isChatting)
        { 
            if (Random.value < 0.5f)
            {
                StartCoroutine(ChatRoutine());
            }
        }
    }

    IEnumerator ChatRoutine()
    {
        isChatting = true;
        
        Debug.Log($"{gameObject.name} chatting...");

        yield return new WaitForSeconds(3f);

        isChatting = false;
    }
}

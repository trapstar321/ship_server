using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonNPC : MonoBehaviour
{
    private State state;
    private NavMeshAgent agent;
    private Vector3 destination;
    public float range = 10f;

    enum State
    {
        PATROL
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position);
    }

    // Start is called before the first frame update
    void Update()
    {
        switch (state)
        {
            case State.PATROL:
                Patrol();
                break;
        }
    }

    void Patrol() {
        if(destination==new Vector3(0,0,0))
            RandomPoint(transform.position, range, out destination);

        float distance = Vector3.Distance(transform.position, destination);        
        if (Vector3.Distance(transform.position, destination) <= 1)
        {
            RandomPoint(transform.position, range, out destination);
        }        

        agent.destination = destination;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}

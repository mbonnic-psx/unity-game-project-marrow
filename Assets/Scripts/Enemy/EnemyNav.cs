using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{

    private NavMeshAgent navMeshAgent;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void StopMoving()
    {
        //Debug.Log("Stopped Moving");
        navMeshAgent.isStopped = true;
    }

    public void SetDestination(Vector3 target)
    {
        //Debug.Log("Set Destination Method");
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(target);
    }

    public void WarpTo(Vector3 position)
    {
        navMeshAgent.Warp(position);
        navMeshAgent.isStopped = false;
    }
}

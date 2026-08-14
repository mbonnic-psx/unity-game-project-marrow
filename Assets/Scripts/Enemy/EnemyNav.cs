using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{

    private NavMeshAgent navMeshAgent;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // The chase target is a moving lead point, not a fixed spot. autoBraking makes the agent decelerate
        // as it "arrives", so it hovers and stutters short of the player instead of running them down.
        navMeshAgent.autoBraking = false;

        // BillBoard overwrites rotation every frame for sprite enemies; don't have the agent fight it.
        if (GetComponent<BillBoard>() != null)
        {
            navMeshAgent.updateRotation = false;
        }
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

    // Speed/accel are what actually separate a Sprinter from a Brute, so they come from EnemyTypeSO on spawn
    // rather than from whatever the prefab's agent was authored with.
    public void ApplyMovement(float speed, float acceleration, float angularSpeed)
    {
        navMeshAgent.speed = speed;
        navMeshAgent.acceleration = acceleration;
        navMeshAgent.angularSpeed = angularSpeed;
    }

    // Crawlers sit low and need a smaller agent so they path under and through gaps a standing enemy can't.
    public void ApplyAgentSize(float radius, float height)
    {
        navMeshAgent.radius = radius;
        navMeshAgent.height = height;
    }

    public void WarpTo(Vector3 position)
    {
        navMeshAgent.Warp(position);
        navMeshAgent.isStopped = false;
    }
}

using UnityEngine;

public class ChaseState : IState
{
    private EnemyStateMachine esm;
    private float attackRange = 2.5f;       // must stay BELOW AttackState.attackExitRange or the two states flip-flop
    private float idleRange = 30f;
    private float timer;
    private float updateInterval = 0.2f;
    private float leadTime = 0.5f;          // secs ahead to lead the player by while chasing
    private float maxLeadDistance = 5f;     // cap how far the lead point can sit from the player's live position
    private float interceptRange = 4f;      // below this, chase the live position instead of the lead (avoids twitchy pathing near attackRange)
    private float repathThreshold = 0.75f;  // don't re-issue a path for a target that barely moved
    private Vector3 lastDestination;
    private bool hasDestination;

    public ChaseState(EnemyStateMachine enemyStateMachine)
    {
        this.esm = enemyStateMachine;
    }

    public void EnterState()
    {
        esm.EnemyAnimator.PlayAnimation("chaseAnim");
        hasDestination = false;                          // reset here, not ExitState — pooled enemies can skip exit paths
        timer = Random.Range(0f, updateInterval);        // stagger re-paths so a pack doesn't all recompute on one frame
        SetChaseDestination(esm.DistanceToPlayer());
    }

    public void Execute()
    {

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;

            float distanceToPlayer = esm.DistanceToPlayer();

            SetChaseDestination(distanceToPlayer);

            if (distanceToPlayer <= attackRange)
            {
                esm.ChangeState(esm.AttackState);
            }
            else if (distanceToPlayer >= idleRange)
            {
                esm.ChangeState(esm.IdleState);
            }
        }


    }

    public void ExitState()
    {
        esm.EnemyNav.StopMoving();
    }

    private void SetChaseDestination(float distanceToPlayer)
    {
        Vector3 target = distanceToPlayer <= interceptRange
            ? esm.PlayerTransform.position
            : esm.PredictedPlayerPosition(leadTime, maxLeadDistance);

        // Re-issuing SetDestination makes the agent throw away its path and plan a new one, which drops its
        // steering for a beat. Doing that every tick against a target that moved a few cm is the stutter.
        if (hasDestination && (target - lastDestination).sqrMagnitude < repathThreshold * repathThreshold)
        {
            return;
        }

        lastDestination = target;
        hasDestination = true;
        esm.EnemyNav.SetDestination(target);
    }
}

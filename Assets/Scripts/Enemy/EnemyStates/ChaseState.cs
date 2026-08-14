using UnityEngine;

public class ChaseState : IState
{
    // Fallbacks for an enemy with no EnemyTypeSO — the values the single skeleton was tuned at.
    private const float DefaultAttackRange = 2.5f;
    private const float DefaultLeadTime = 0.5f;
    private const float DefaultMaxLeadDistance = 5f;
    private const float DefaultInterceptRange = 4f;

    private EnemyStateMachine esm;

    // Resolved from EnemyTypeSO in EnterState; see AttackState for why these are cached rather than read live.
    private float attackRange = DefaultAttackRange;              // must stay BELOW AttackState's exit range or the states flip-flop
    private float leadTime = DefaultLeadTime;                    // secs ahead to lead the player by while chasing
    private float maxLeadDistance = DefaultMaxLeadDistance;      // cap how far the lead point sits from the player's live position
    private float interceptRange = DefaultInterceptRange;        // below this, chase the live position (avoids twitchy pathing up close)

    private float idleRange = 30f;
    private float timer;
    private float updateInterval = 0.2f;
    private float repathThreshold = 0.75f;  // don't re-issue a path for a target that barely moved
    private Vector3 lastDestination;
    private bool hasDestination;

    public ChaseState(EnemyStateMachine enemyStateMachine)
    {
        this.esm = enemyStateMachine;
    }

    public void EnterState()
    {
        EnemyTypeSO type = esm.Type;
        attackRange = type != null ? type.attackRange : DefaultAttackRange;
        leadTime = type != null ? type.leadTime : DefaultLeadTime;
        maxLeadDistance = type != null ? type.maxLeadDistance : DefaultMaxLeadDistance;
        interceptRange = type != null ? type.interceptRange : DefaultInterceptRange;

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

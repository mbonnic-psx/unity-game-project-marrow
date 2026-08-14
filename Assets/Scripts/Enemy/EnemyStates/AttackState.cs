using UnityEngine;

public class AttackState : IState
{
    // Fallbacks for an enemy with no EnemyTypeSO — these are the values the single skeleton was tuned at.
    private const float DefaultExitRange = 3.2f;
    private const float DefaultCoolDown = 2.0f;
    private const float DefaultWindup = 0.6f;

    private EnemyStateMachine esm;

    // Resolved from EnemyTypeSO on EnterState rather than read per-frame: the type can't change mid-state,
    // and caching keeps a null-check off the hot path.
    private float attackExitRange = DefaultExitRange;   // deliberately wider than ChaseState's attackRange: with one
                                                        // shared threshold the enemy flipped Chase<->Attack every tick
                                                        // against a moving player and the cooldown never completed
    private float attackCoolDown = DefaultCoolDown;
    private float attackWindup = DefaultWindup;         // warning beat before every swing — EnemyTelegraph presents this
                                                        // window; also the grace period for running past an enemy
    private float timer;
    private bool telegraphed;   // one tell per swing, not one per frame of the wind-up window

    public AttackState(EnemyStateMachine enemyStateMachine)
    {
        this.esm = enemyStateMachine;
    }

    public void EnterState()
    {
        EnemyTypeSO type = esm.Type;
        attackExitRange = type != null ? type.attackExitRange : DefaultExitRange;
        attackCoolDown = type != null ? type.attackCoolDown : DefaultCoolDown;
        attackWindup = type != null ? type.attackWindup : DefaultWindup;

        // Start the clock partway through so the first swing lands after attackWindup rather than a full
        // cooldown. Set in EnterState, not ExitState — pooled enemies can skip exit paths.
        timer = Mathf.Max(0f, attackCoolDown - attackWindup);
        telegraphed = false;
        esm.EnemyAnimator.PlayAnimation("attackAnim");
    }

    public void Execute()
    {
        float distanceToPlayer = esm.DistanceToPlayer();

        if (distanceToPlayer > attackExitRange)
        {
            esm.ChangeState(esm.ChaseState);
            return;
        }

        timer += Time.deltaTime;

        // Start the tell attackWindup seconds before the swing lands. Driving it off the same timer that
        // applies the damage keeps the warning honest — retune attackWindup and the tell follows.
        if (!telegraphed && timer >= attackCoolDown - attackWindup)
        {
            telegraphed = true;
            if (esm.EnemyTelegraph != null)
            {
                esm.EnemyTelegraph.BeginWindup(attackWindup);
            }
        }

        if (timer >= attackCoolDown)
        {
            timer = 0f;
            telegraphed = false;
            esm.EnemyAnimator.PlayAnimation("attackAnim");
            esm.EnemyAttack.Attack(attackExitRange);   // reach covers the whole hysteresis band, so edge swings connect
        }
    }

    public void ExitState()
    {
    }
}

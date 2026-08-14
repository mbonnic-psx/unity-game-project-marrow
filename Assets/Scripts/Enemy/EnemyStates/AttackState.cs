using UnityEngine;

public class AttackState : IState
{
    private EnemyStateMachine esm;
    private float attackExitRange = 3.2f;   // deliberately wider than ChaseState.attackRange (2.5f): with one shared
                                            // threshold the enemy flipped Chase↔Attack every tick against a moving
                                            // player, and the cooldown restarted each time, so a swing never landed
    private float attackCoolDown = 2.0f;
    private float attackWindup = 0.6f;      // beat of warning before the first swing (real telegraph is a Phase 1 task);
                                            // also the grace window for running past an enemy without eating a hit
    private float timer;

    public AttackState(EnemyStateMachine enemyStateMachine)
    {
        this.esm = enemyStateMachine;
    }

    public void EnterState()
    {
        // Start the clock partway through so the first swing lands after attackWindup rather than a full
        // cooldown. Set in EnterState, not ExitState — pooled enemies can skip exit paths.
        timer = Mathf.Max(0f, attackCoolDown - attackWindup);
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
        if (timer >= attackCoolDown)
        {
            timer = 0f;
            esm.EnemyAnimator.PlayAnimation("attackAnim");
            esm.EnemyAttack.Attack(attackExitRange);   // reach covers the whole hysteresis band, so edge swings connect
        }

    }

    public void ExitState()
    {
    }
}

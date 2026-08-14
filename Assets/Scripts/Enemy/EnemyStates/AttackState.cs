using UnityEngine;

public class AttackState : IState
{
    private EnemyStateMachine esm;
    private float attackExitRange = 3.2f;   // deliberately wider than ChaseState.attackRange (2.5f): with one shared
                                            // threshold the enemy flipped Chase↔Attack every tick against a moving
                                            // player, and the cooldown restarted each time, so a swing never landed
    private float attackCoolDown = 2.0f;
    private float attackWindup = 0.6f;      // warning beat before every swing — EnemyTelegraph presents this window;
                                            // also the grace window for running past an enemy without eating a hit
    private float timer;
    private bool telegraphed;   // one tell per swing, not one per frame of the wind-up window

    public AttackState(EnemyStateMachine enemyStateMachine)
    {
        this.esm = enemyStateMachine;
    }

    public void EnterState()
    {
        // Start the clock partway through so the first swing lands after attackWindup rather than a full
        // cooldown. Set in EnterState, not ExitState — pooled enemies can skip exit paths.
        timer = Mathf.Max(0f, attackCoolDown - attackWindup);
        telegraphed = false;   // reset in EnterState, not ExitState — pooled enemies can skip exit paths
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

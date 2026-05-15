using UnityEngine;

public class ChaseState : IState
{
    private EnemyStateMachine esm;
    private float attackRange = 2f;
    private float idleRange = 30f;
    private float timer;
    private float updateInterval = 0.2f;

    public ChaseState(EnemyStateMachine enemyStateMachine)
    {
        this.esm = enemyStateMachine;
    }

    public void EnterState()
    {
        esm.EnemyAnimator.PlayAnimation("chaseAnim");
        esm.EnemyNav.SetDestination(esm.PlayerTransform.position);
    }

    public void Execute()
    {

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;

            float distanceToPlayer = esm.DistanceToPlayer();

            esm.EnemyNav.SetDestination(esm.PlayerTransform.position);

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
}

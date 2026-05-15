using UnityEngine;

public class AttackState : IState
{
    private EnemyStateMachine esm;
    private float attackRange = 2f;
    private float attackCoolDown = 1.5f;
    private float timer;

    public AttackState(EnemyStateMachine enemyStateMachine)
    {
        this.esm = enemyStateMachine;
    }

    public void EnterState()
    {
        esm.EnemyAnimator.PlayAnimation("attackAnim");
        //esm.EnemyAttack.Attack();
    }

    public void Execute()
    {
        float distanceToPlayer = esm.DistanceToPlayer();

        if (distanceToPlayer > attackRange)
        {
            esm.ChangeState(esm.ChaseState);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= attackCoolDown)
        {
            timer = 0f;
            esm.EnemyAnimator.PlayAnimation("attackAnim");
            //esm.EnemyAttack.Attack();
        }

    }

    public void ExitState()
    {
        timer = 0f;
    }
}

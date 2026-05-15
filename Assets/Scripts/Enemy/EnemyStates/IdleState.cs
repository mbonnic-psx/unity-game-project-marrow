using System;
using Unity.VisualScripting;
using UnityEngine;

public class IdleState : IState
{
    private EnemyStateMachine esm;
    private float detectRange = 500.0f;

    public IdleState(EnemyStateMachine enemyStateMachine)
    {
        this.esm = enemyStateMachine;
    }

    public void EnterState()
    {
        esm.EnemyNav.StopMoving();
        esm.EnemyAnimator.PlayAnimation("idleAnim");
        esm.BillBoard.EnableLookAt();
    }

    public void Execute()
    {
        float distanceToPlayer = esm.DistanceToPlayer();

        if(distanceToPlayer <= detectRange)
        {
            esm.ChangeState(esm.ChaseState);
        }
        
    }

    public void ExitState(){}
}

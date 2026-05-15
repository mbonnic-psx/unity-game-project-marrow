using UnityEngine;

public class DeadState : IState
{
    private EnemyStateMachine esm;
    private float deathDelay = 3f;
    private float timer;

    public DeadState(EnemyStateMachine enemyStateMachine)
    {
        this.esm = enemyStateMachine;
    }

    public void EnterState()
    {
        esm.EnemyNav.StopMoving();
        esm.EnemyRagdoll.EnableRagdoll();
        esm.EnemyDrop.DropItems();
        esm.BillBoard.DisableLookAt();
        //esm.EnemyAnimator.PlayAnimation("deadAnim");
        esm.EnemyCollider.enabled = false;
        esm.PlayerUI.Counter();

    }

    public void Execute()
    {
        timer += Time.deltaTime;
        //Debug.Log("DeadState Timer: " + timer);
        if (timer >= deathDelay)
        {
            esm.WaveManager.EnemyDied();
            esm.ObjectPool.ReturnEnemy(esm.gameObject);
            esm.gameObject.SetActive(false);
        }
    }

    public void ExitState() { }
}

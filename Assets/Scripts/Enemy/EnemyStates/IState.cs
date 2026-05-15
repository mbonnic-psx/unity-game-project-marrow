using UnityEngine;

public interface IState
{
    public void EnterState();
    public void Execute();
    public void ExitState();
}

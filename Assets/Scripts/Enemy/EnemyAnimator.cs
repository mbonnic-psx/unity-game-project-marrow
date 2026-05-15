using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private float animTransition = 0.2f;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string animString)
    {
        animator.CrossFade(animString, animTransition);
    }
}

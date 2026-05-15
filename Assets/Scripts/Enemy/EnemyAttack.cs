using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float damageAmount = 33f;
    [SerializeField] private float hitRadius = 2f;
    [SerializeField] private LayerMask playerLayer;
    private EnemyStateMachine esm;
    private PlayerStats playerStats;

    void Awake()
    {
        esm = GetComponent<EnemyStateMachine>();
    }

    public void Attack()
    {
        playerStats = esm.PlayerTransform.GetComponent<PlayerStats>();
        if (playerStats == null)
            return;

        Collider[] hitbox = Physics.OverlapSphere(transform.position, hitRadius, playerLayer);
        foreach (var hitCollider in hitbox)
        {
            if (hitCollider.transform == esm.PlayerTransform)
            {
                playerStats.TakeDamage(damageAmount);
                break;
            }
        }
    }
}

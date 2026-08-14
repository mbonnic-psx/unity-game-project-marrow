using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    private EnemyStateMachine esm;
    private ShotgunCharge sc;

    #region 
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    #endregion

    void Awake()
    {
        esm = GetComponent<EnemyStateMachine>();
        sc = FindAnyObjectByType<ShotgunCharge>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        if (currentHealth <= 0)
        {
            //esm.WaveManager.EnemyDied(); 
            esm.ChangeState(esm.DeadState);


            if (sc != null)
            {
                sc.RegisterKill();
            }
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    // Driven by EnemyTypeSO on spawn: a pooled body may have last lived as a different enemy type entirely.
    public void SetMaxHealth(float value)
    {
        maxHealth = Mathf.Max(1f, value);
        currentHealth = maxHealth;
    }
}

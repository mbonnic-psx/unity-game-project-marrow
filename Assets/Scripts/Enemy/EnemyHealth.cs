using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    private EnemyStateMachine esm;

    #region 
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    #endregion

    void Awake()
    {
        esm = GetComponent<EnemyStateMachine>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        if(currentHealth <= 0)
        {
            //esm.WaveManager.EnemyDied(); 
            esm.ChangeState(esm.DeadState); 
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}

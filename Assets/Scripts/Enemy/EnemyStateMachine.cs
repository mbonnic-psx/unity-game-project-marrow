using UnityEngine;

//Has the current state and handles the transitions

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private EnemyNav enemyNav;
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private EnemyAnimator enemyAnim;
    [SerializeField] private EnemyRagdoll enemyRagdoll;
    [SerializeField] private EnemyDrop enemyDrop;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private BillBoard billBoard;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private float cullRange = 50f;          // beyond this → recycle to a fresh spawn ahead
    [SerializeField] private float cullCheckInterval = 0.5f;
    private float cullTimer;
    private IState currentState;
    private AttackState attackState;
    private ChaseState chaseState;
    private DeadState deadState;
    private IdleState idleState;
    //private StunState stunState;

    #region Getters
    public Transform PlayerTransform => playerTransform;
    public GameObject PlayerObject => playerObject;
    public PlayerStats PlayerStats => playerStats;
    public Collider EnemyCollider => enemyCollider;
    public EnemyNav EnemyNav => enemyNav;
    public EnemyAttack EnemyAttack => enemyAttack;
    public EnemyHealth EnemyHealth => enemyHealth;
    public EnemyAnimator EnemyAnimator => enemyAnim;
    public EnemyRagdoll EnemyRagdoll => enemyRagdoll;
    public EnemyDrop EnemyDrop => enemyDrop;
    public WaveManager WaveManager => waveManager;
    public ObjectPool ObjectPool => objectPool;
    public BillBoard BillBoard => billBoard;
    public PlayerUI PlayerUI => playerUI;
    public AttackState AttackState => attackState;
    public ChaseState ChaseState => chaseState;
    public DeadState DeadState => deadState;
    public IdleState IdleState => idleState;
    //public StunState StunState => stunState;
    #endregion

    void Awake()
    {
        enemyCollider = GetComponent<Collider>();
        enemyNav = GetComponent<EnemyNav>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAnim = GetComponent<EnemyAnimator>();
        enemyRagdoll = GetComponent<EnemyRagdoll>();
        enemyDrop = GetComponent<EnemyDrop>();
        waveManager = GetComponent<WaveManager>();
        objectPool = GetComponent<ObjectPool>();
        billBoard = GetComponent<BillBoard>();
        playerUI = FindFirstObjectByType<PlayerUI>();
        attackState = new AttackState(this);
        chaseState = new ChaseState(this);
        deadState = new DeadState(this);
        idleState = new IdleState(this);
        //stunState = new StunState(this);
    }

    void Start()
    {
        ChangeState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }

            // recycle hopeless stragglers into the forward cone
        if (currentState != deadState && waveManager != null && playerTransform != null)
        {
            cullTimer += Time.deltaTime;
            if (cullTimer >= cullCheckInterval)
            {
                cullTimer = 0f;
                if (DistanceToPlayer() >= cullRange)
                    waveManager.RelocateEnemy(this);
            }
        }
    }

    public void ChangeState(IState newState)
    {
        //Debug.Log("Changing state to: " + newState.GetType().Name);
        if (currentState != null)
        {
            currentState.ExitState();
        }

        currentState = newState;
        currentState.EnterState();
    }

    public float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, playerTransform.position);
    }

    public void ResetEnemy()
    {
        // if (currentState == deadState)
        // {
        //     return;
        // }
        EnemyHealth.ResetHealth();
        EnemyRagdoll.DisableRagdoll();
        EnemyCollider.enabled = true;
        ChangeState(idleState);
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    public void SetWaveManager(WaveManager wm)
    {
        this.waveManager = wm;
    }

    public void SetObjectPool(ObjectPool op)
    {
        this.objectPool = op;
    }
}

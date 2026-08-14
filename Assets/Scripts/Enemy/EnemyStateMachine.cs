using UnityEngine;
using UnityEngine.AI;

//Has the current state and handles the transitions

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private PlayerMovement playerMovement;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Collider enemyCollider;
    [SerializeField] private EnemyNav enemyNav;
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private EnemyAnimator enemyAnim;
    [SerializeField] private EnemyTelegraph enemyTelegraph;   // optional — enemies without one just don't tell
    [SerializeField] private EnemyIdentity enemyIdentity;     // optional — untyped enemies keep their prefab stats
    [SerializeField] private EnemyRagdoll enemyRagdoll;
    [SerializeField] private EnemyDrop enemyDrop;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private BillBoard billBoard;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private float cullRange = 50f;          // beyond this → recycle to a fresh spawn ahead
    [SerializeField] private float cullCheckInterval = 0.5f;
    [SerializeField] private float velocitySmoothing = 6f;   // higher = tracks the player's turns faster, but jitters more
    [SerializeField] private float navSampleRadius = 2f;     // keep tight: a wide sample can snap the lead point through a wall
    private float cullTimer;
    private Vector3 smoothedPlayerVelocity;
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
    public EnemyTelegraph EnemyTelegraph => enemyTelegraph;
    public EnemyIdentity EnemyIdentity => enemyIdentity;
    public EnemyTypeSO Type => enemyIdentity != null ? enemyIdentity.Type : null;   // states read tunables off this
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
        enemyTelegraph = GetComponent<EnemyTelegraph>();
        enemyIdentity = GetComponent<EnemyIdentity>();
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

        // Smooth the player's velocity before anyone predicts off it — raw bhop/strafe velocity swings
        // every frame, and feeding that straight into a chase target is what makes agents dance in place.
        if (playerMovement != null)
        {
            smoothedPlayerVelocity = Vector3.Lerp(
                smoothedPlayerVelocity,
                playerMovement.HorizontalVelocity,
                1f - Mathf.Exp(-velocitySmoothing * Time.deltaTime));   // framerate-independent
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

    // Where the player will be leadTime seconds from now, as a point this enemy can actually path to.
    // Three things matter here, all learned the hard way:
    //   1. it leads off the SMOOTHED velocity, so strafe/bhop noise doesn't make the target dance
    //   2. the lead is clamped BEFORE the NavMesh sample — sampling a far-off point first lets it snap
    //      to unrelated geometry (often on the far side of a wall), which teleports the destination
    //   3. velocity that is closing on THIS enemy is stripped out — leading into a player who is running
    //      at you just sends the enemy sprinting past them. Only lateral movement is worth cutting off.
    public Vector3 PredictedPlayerPosition(float leadTime, float maxLeadDistance)
    {
        Vector3 playerPos = playerTransform.position;
        Vector3 vel = smoothedPlayerVelocity;

        Vector3 toEnemy = transform.position - playerPos;
        toEnemy.y = 0f;
        if (toEnemy.sqrMagnitude > 0.01f)
        {
            Vector3 axis = toEnemy.normalized;
            float closing = Vector3.Dot(vel, axis);
            if (closing > 0f)
            {
                vel -= axis * closing;
            }
        }

        Vector3 predicted = playerPos + Vector3.ClampMagnitude(vel * leadTime, maxLeadDistance);

        if (NavMesh.SamplePosition(predicted, out NavMeshHit hit, navSampleRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return playerPos;   // lead point isn't reachable — just go at the player directly
    }

    public void ResetEnemy()
    {
        // if (currentState == deadState)
        // {
        //     return;
        // }
        // Resolved lazily, not in Awake: ObjectPool stamps EnemyIdentity onto the instance after Instantiate
        // has already run Awake, so a spawn-time lookup is the only one guaranteed to find it.
        if (enemyIdentity == null)
        {
            enemyIdentity = GetComponent<EnemyIdentity>();
        }

        // Push type stats before anything reads them — a pooled body may have last lived as another type.
        if (enemyIdentity != null)
        {
            enemyIdentity.ApplyStats(enemyHealth, enemyNav, enemyAttack);
        }

        EnemyHealth.ResetHealth();
        EnemyRagdoll.DisableRagdoll();
        EnemyCollider.enabled = true;
        smoothedPlayerVelocity = Vector3.zero;   // pooled enemies must not inherit the last owner's lead
        EnemyAttack.ResetAttack();               // ...nor the previous life's hit cooldown
        if (enemyTelegraph != null) enemyTelegraph.ResetTelegraph();   // ...nor respawn still red and swollen
        ChangeState(idleState);
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
        playerMovement = player.GetComponent<PlayerMovement>();
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

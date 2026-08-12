using UnityEngine;

/* <summary>
    Holds an external velocity (shotgun recoil, explosions, anything) and
    applies it on top of your normal movement script.

    You can't AddForce to a CharacterController, so this keeps its own velocity
    and calls cc.Move() a second time each frame. Unity allows multiple Move()
    calls per frame, so this stacks with your existing controller rather than
    fighting it — no edits to your movement script required.

    If your player is a Rigidbody instead, see RIGIDBODY NOTE at the bottom.

    Made Claude Opus 4.8 - High
*/

[RequireComponent(typeof(CharacterController))]
public class PlayerImpulse : MonoBehaviour
{
     [Header("Decay")]
    [Tooltip("How fast the boost bleeds off mid-air. Lower = floatier, longer flights.")]
    [SerializeField] float airDecay      = 2.5f;
    [Tooltip("How fast it bleeds off once you land. High = you stop on impact.")]
    [SerializeField] float groundedDecay = 14f;
 
    [Header("Limits")]
    [SerializeField] float maxSpeed = 30f;
 
    [Tooltip("Ignores grounded decay for this long after a boost, so launching " +
             "from the floor doesn't get instantly eaten by the ground check.")]
    [SerializeField] float groundedGrace = 0.15f;
 
    CharacterController cc;
    Vector3 velocity;
    float   graceTimer;
 
    public Vector3 Velocity  => velocity;
    public bool    IsBoosted => velocity.sqrMagnitude > 0.01f;
 
    void Awake() => cc = GetComponent<CharacterController>();
 
    public void AddImpulse(Vector3 impulse, bool cancelExisting = false)
    {
        if (cancelExisting) velocity = Vector3.zero;
 
        velocity += impulse;
        velocity  = Vector3.ClampMagnitude(velocity, maxSpeed);
        graceTimer = groundedGrace;
    }
 
    public void Clear() => velocity = Vector3.zero;
 
    void Update()
    {
        if (velocity.sqrMagnitude < 0.01f)
        {
            velocity = Vector3.zero;
            return;
        }
 
        cc.Move(velocity * Time.deltaTime);
 
        if (graceTimer > 0f)
        {
            graceTimer -= Time.deltaTime;
        }
        else
        {
            float decay = cc.isGrounded ? groundedDecay : airDecay;
            velocity = Vector3.Lerp(velocity, Vector3.zero, decay * Time.deltaTime);
        }
    }
}

/*  RIGIDBODY NOTE ----------------------------------------------------------
    If your player is a Rigidbody rather than a CharacterController, you don't
    need this component at all. Replace Shotgun.ApplySelfBoost() with:
 
        if (cancelPreviousBoost) rb.velocity = Vector3.zero;
        rb.AddForce(dir * boostForce, ForceMode.VelocityChange);
 
    ...and swap the PlayerImpulse field on Shotgun for a Rigidbody reference.
--------------------------------------------------------------------------- */

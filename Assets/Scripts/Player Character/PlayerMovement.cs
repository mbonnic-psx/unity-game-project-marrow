using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    CharacterController cc;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform player;
    [SerializeField] private PerkMachine pm;

    [Header("Speed")]
    [SerializeField] private float maxGroundSpeed = 8.0f;
    [SerializeField] private float maxAirSpeed = 8.0f;
    [SerializeField] private float jump = 8.0f;
    [SerializeField] private float sideStrafeSpeed = 1.0f;    // What the max speed to generate when side strafing

    [Header("Acceleration")]
    [SerializeField] private float groundAcceleration = 14.0f;
    [SerializeField] private float groundDeaccleration = 10.0f;
    [SerializeField] private float airAcceleration = 2.0f;
    [SerializeField] private float airDeacceleration = 2.0f;    // Deacceleration experienced when opposite strafing
    [SerializeField] private float sideStrafeAcceleration = 50.0f;   // How fast acceleration occurs to get up to sideStrafeSpeed when side strafing

    [Header("Air")]
    [SerializeField] private float airControl = 0.3f;  // How precise air control is
    [SerializeField] private bool JumpQueue = false;
    [SerializeField] private bool wishJump = false;
    [SerializeField] private float airControlScale = 32.0f;

    [Header("Sliding")]
    [SerializeField] private KeyCode slideKey = KeyCode.LeftControl;
    [SerializeField] private float slideStartSpeed = 5.0f;
    [SerializeField] private float slideDuration = 0.75f;
    [SerializeField] private float slideBoost = 4.0f;
    [SerializeField] private float slideFriction = 1.2f;
    [SerializeField] private float maxSlideSpeed = 12.0f;
    [SerializeField] private float slideAccel = 8.0f;
    [SerializeField] private float slideEndSpeed = 2.0f;
    [SerializeField] private float slideCameraOffset = 0.6f;
    [SerializeField] private float slideCameraDuration = 0.15f;
    private float slideTimer;
    private bool slideStarted;
    private bool slidePressed;
    private bool slideHold;
    private bool isSliding;
    private Vector3 slideDir;

    [Header("Extra")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float playerFriction = 0f;
    [SerializeField] private float groundFriction = 6f;
    [SerializeField] private float GroundDistance = 0.4f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float playerTopVelocity = 0f;
    [SerializeField] private Vector3 moveDirection, moveDirectionNorm;

    public enum MovementState
    {
        walking,
        sliding
    }

    #region Used for UI
    private Vector3 lastPos;
    private Vector3 moved;
    private Vector3 PlayerVel;
    private float ModulasSpeed;
    private float XVelocity, ZVelocity;
    #endregion

    private float inputX, inputZ;
    private bool jumpPressed;
    private Vector3 playerVelocity;
    Vector3 wishdir;
    Vector3 vec;
    Vector3 flatVel; // Temp Vector3 to measure horizontal speed
    private float wishspeed, wishspeed2;
    float addspeed;
    float accelspeed;
    float currentspeed;
    float speed, zspeed;
    float dot;
    float k;
    float accel;
    float newspeed;
    float control;
    float drop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = GetComponent<CharacterController>();
        lastPos = player.position;
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                wishJump = true;
            }
            else
            {
                JumpQueue = true;
            }
        }

        if (Input.GetKeyDown(slideKey) && pm.SliderActivated == true)
        {
            slidePressed = true;
        }

        slideHold = Input.GetKey(slideKey);

        flatVel = playerVelocity;
        flatVel.y = 0;

        if (flatVel.magnitude > playerTopVelocity)
        {
            playerTopVelocity = flatVel.magnitude;
        }
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        //Debug.Log($"isGrounded: {isGrounded} | GroundCheck Y: {GroundCheck.position.y} | GroundDistance: {GroundDistance}");

        QueueJump();

        if (isGrounded)
        {
            GroundMove(dt);
        }
        else
        {
            AirMove(dt);
        }

        if (slidePressed)
        {
            StartSlide(wishdir);
            slidePressed = false;
        }

        if (isSliding)
        {
            Slide(wishdir, dt);
        }

        cc.Move(playerVelocity * dt);

        if ((cc.collisionFlags & CollisionFlags.Above) != 0 && playerVelocity.y > 0f)
        {
            playerVelocity.y = 0f;
        }
    }

    public void QueueJump()
    {
        if (isGrounded && JumpQueue)
        {
            wishJump = true;
            JumpQueue = false;
        }
    }

    public void StartSlide(Vector3 wishDir)
    {
        if (!isGrounded)
        {
            return;
        }

        float slideSpeed = new Vector3(playerVelocity.x, 0f, playerVelocity.z).magnitude;
        if (slideSpeed < slideStartSpeed)
        {
            return;
        }

        isSliding = true;
        slideStarted = true;
        slideTimer = slideDuration;

        playerCamera.DOLocalMoveY(
            playerCamera.localPosition.y - slideCameraOffset, slideCameraDuration
        ).SetEase(Ease.OutQuart);

        Vector3 lateral = new Vector3(playerVelocity.x, 0f, playerVelocity.z);
        if (lateral.sqrMagnitude > 0.001f)
        {
            slideDir = lateral.normalized;
        }
        else if (wishDir.sqrMagnitude > 0.001f)
        {
            slideDir = wishDir;
        }
        else
        {
            slideDir = transform.forward;
        }
    }

    public void Slide(Vector3 wishDir, float dt)
    {
        if (slideStarted)
        {
            playerVelocity += slideDir * slideBoost;
            slideStarted = false;
        }

        slideTimer -= dt;

        ApplyFriction(1.0f, slideFriction, dt);

        if (wishDir.sqrMagnitude > 0.001f)
        {
            Accelerate(wishDir, maxSlideSpeed, slideAccel, dt);
            slideDir = Vector3.Slerp(slideDir, wishDir, 6f * dt).normalized;
        }

        if (playerVelocity.y < 0f)
        {
            playerVelocity.y = 0f;
        }

        float slideSpeed = new Vector3(playerVelocity.x, 0f, playerVelocity.z).magnitude;

        if (slideTimer <= 0f)
        {
            StopSlide();
        }
        else if (slideSpeed < slideEndSpeed)
        {
            StopSlide();
        }
        else if (!slideHold)
        {
            StopSlide();
        } // if you want release-to-cancel
    }

    public void StopSlide()
    {
        isSliding = false;
        slideStarted = false;
        slideTimer = 0f;

        playerCamera.DOKill();
        playerCamera.DOLocalMoveY(
           playerCamera.localPosition.y + slideCameraOffset, slideCameraDuration
       ).SetEase(Ease.OutQuart);
    }

    //Calculates wish acceleration
    public void Accelerate(Vector3 wishDir, float wishSpeed, float accel, float dt)
    {
        currentspeed = Vector3.Dot(playerVelocity, wishDir); //Tells you how much the playerVelocity direction points in the same direction as wishDir
        addspeed = wishSpeed - currentspeed;

        if (addspeed <= 0)
        {
            return;
        }
        accelspeed = accel * dt * wishspeed;

        if (accelspeed > addspeed)
        {
            accelspeed = addspeed;
        }
        playerVelocity.x += accelspeed * wishDir.x;
        playerVelocity.z += accelspeed * wishDir.z;
    }

    public void AirMove(float dt)
    {
        Vector3 input = Vector3.ClampMagnitude(new Vector3(inputX, 0, inputZ), 1f);
        wishdir = transform.TransformDirection(input).normalized;
        wishspeed = input.magnitude * maxAirSpeed;

        //Air Control
        wishspeed2 = wishspeed;
        if (Vector3.Dot(playerVelocity, wishdir) < 0)
        {
            accel = airDeacceleration;
        }
        else
        {
            accel = airAcceleration;
        }

        //If the player is ONLY strafing left or right
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            if (wishspeed > sideStrafeSpeed)
            {
                wishspeed = sideStrafeSpeed;
            }
            accel = sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel, dt);

        AirControl(wishdir, wishspeed2, dt);

        playerVelocity.y += gravity * dt;
    }

    public void AirControl(Vector3 wishDir, float wishSpeed, float dt)
    {
        if (Input.GetAxis("Horizontal") == 0 || wishSpeed == 0)
        {
            return;
        }

        zspeed = playerVelocity.y;
        playerVelocity.y = 0;

        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dot = Vector3.Dot(playerVelocity, wishDir);
        k = airControlScale;
        k *= airControl * dot * dot * dt;

        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishDir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishDir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishDir.z * k;

            playerVelocity.Normalize();
            moveDirectionNorm = playerVelocity;
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zspeed; //Note this line
        playerVelocity.z *= speed;
    }

    public void GroundMove(float dt)
    {
        if (!wishJump)
        {
            ApplyFriction(1.0f, groundFriction, dt);
        }
        else
        {
            ApplyFriction(0, groundFriction, dt);
        }

        Vector3 input = Vector3.ClampMagnitude(new Vector3(inputX, 0, inputZ), 1f);
        wishdir = transform.TransformDirection(input).normalized;
        moveDirection = wishdir;
        wishspeed = input.magnitude * maxGroundSpeed;

        Accelerate(wishdir, wishspeed, groundAcceleration, dt);

        //Reset the gravity velocity
        playerVelocity.y = 0;

        if (wishJump)
        {
            playerVelocity.y = jump;
            wishJump = false;
        }
    }

    public void ApplyFriction(float t, float f, float dt)
    {
        vec = playerVelocity;
        vec.y = 0f;
        speed = vec.magnitude;
        drop = 0f;

        if (isGrounded)
        {
            control = Mathf.Max(speed, groundDeaccleration);
            drop = control * f * dt * t;
        }

        newspeed = Mathf.Max(speed - drop, 0f);
        playerFriction = newspeed;

        if (speed > 0)
        {
            newspeed /= speed;
        }

        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;
    }

    void OnDrawGizmosSelected()
    {
        if (GroundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GroundCheck.position, GroundDistance);
    }
}

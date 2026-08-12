using UnityEngine;
using DG.Tweening;
using System.Collections;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform holdPos;

    [SerializeField] private float throwImpulse = 500f;
    [SerializeField] private float pickupRange = 5f;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(50, 50, 50);
    [SerializeField] private KeyCode lMouse = KeyCode.Mouse0;
    [SerializeField] private LayerMask layers;
    [SerializeField] private WeaponHandler weaponHandler;

    private GameObject heldObject;
    private Rigidbody objectRb;
    private bool lMouseHold;
    private bool shouldRotate;
    private LayerMask holdLayer;

    // Strong Man Perk
    [Header("Strong Man Perk")]
    [SerializeField] private bool strongManActivated = false;
    [SerializeField] private float strongManHoldDuration = 5f;   // Seconds before the object "expires"
    [SerializeField] private float strongManImpulseMultiplier = 1.5f;
    [SerializeField] private float strongManDestroyDelay = 3f;   // Seconds after throw before object destroys

    private float holdTimer = 0f;
    private bool isFlashing = false;
    private Coroutine flashCoroutine;

    [SerializeField] private PerkMachine pm;

    #region Getters
    public bool LMouseHold => lMouseHold;
    public GameObject HeldObject => heldObject;
    public bool StrongManActivated
    {
        get => strongManActivated;
        set => strongManActivated = value;
    }
    #endregion

    void Start()
    {
        holdLayer = LayerMask.NameToLayer("Hold");
        shouldRotate = false;
    }

    void Update()
    {
        ObjectRotation();
        TickStrongManHoldTimer();

        if (Input.GetKeyDown(lMouse))
        {
            if (heldObject == null && (weaponHandler == null || !weaponHandler.IsEquipped))
            {
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, pickupRange, layers))
                {
                    if (hit.transform.gameObject.tag == "Pickup")
                    {
                        PickupObject(hit.transform.gameObject);
                        ObjectRotation();
                        lMouseHold = true;
                    }
                }
            }
        }

        if (Input.GetKeyUp(lMouse))
        {
            if (heldObject != null && lMouseHold == true)
            {
                StopClipping();
                ThrowObject();
                lMouseHold = false;
                shouldRotate = false;
            }
        }
    }

    private void TickStrongManHoldTimer()
    {
        if (!strongManActivated || heldObject == null || !lMouseHold) return;

        holdTimer += Time.deltaTime;

        // Start flashing at 60% of the hold duration
        float flashThreshold = strongManHoldDuration * 0.6f;
        if (holdTimer >= flashThreshold && !isFlashing)
        {
            flashCoroutine = StartCoroutine(FlashObject());
        }

        // Auto-throw at the end of the timer (object "expires")
        if (holdTimer >= strongManHoldDuration)
        {
            StopClipping();
            ThrowObject();
            lMouseHold = false;
            shouldRotate = false;
        }
    }

    // Flashes/darkens the held object's material color using DOTween
    private IEnumerator FlashObject()
    {
        isFlashing = true;

        Renderer rend = heldObject ? heldObject.GetComponent<Renderer>() : null;
        if (rend == null) { isFlashing = false; yield break; }

        Color originalColor = rend.material.color;
        Color darkColor = Color.black; // Darken significantly
        darkColor.a = 1f;

        while (heldObject != null && isFlashing)
        {
            // Flash speed increases as the timer gets closer to expiry
            float remaining = strongManHoldDuration - holdTimer;
            float t = 1f - Mathf.Clamp01(remaining / (strongManHoldDuration * 0.4f)); // 0→1 over last 40%
            float flashSpeed = Mathf.Lerp(0.25f, 0.05f, t); // slows to very fast flicker

            rend.material.DOKill();
            rend.material.DOColor(darkColor, flashSpeed).SetEase(Ease.InQuart)
                .OnComplete(() =>
                {
                    if (rend != null && heldObject != null)
                        rend.material.DOColor(originalColor, flashSpeed).SetEase(Ease.OutQuart);
                });

            yield return new WaitForSeconds(flashSpeed * 2f);
        }

        // Restore original color when done
        if (rend != null)
            rend.material.color = darkColor;

        isFlashing = false;
    }

    public void PickupObject(GameObject pickupObject)
    {
        if (pickupObject.GetComponent<Rigidbody>())
        {
            heldObject = pickupObject;
            objectRb = pickupObject.GetComponent<Rigidbody>();
            objectRb.isKinematic = true;
            heldObject.transform.parent = holdPos.transform;

            Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), player.GetComponent<Collider>(), true);

            // Reset strong man timer each time we pick up
            holdTimer = 0f;
            isFlashing = false;

            heldObject.transform.DOLocalMove(Vector3.zero, 0.3f)
                .SetEase(Ease.OutQuart)
                .OnComplete(() => shouldRotate = true);
        }
    }

    public void ThrowObject()
    {
        if (heldObject == null) return;

        // Stop the flash coroutine cleanly
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        isFlashing = false;
        holdTimer = 0f;

        // Restore original color in case it was mid-flash
        Renderer rend = heldObject.GetComponent<Renderer>();
        if (rend != null) rend.material.DOKill();

        shouldRotate = false;

        Physics.IgnoreCollision(heldObject.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObject.transform.DOKill();
        objectRb.isKinematic = false;
        heldObject.transform.parent = null;

        // Apply impulse — multiply by 1.5 if Strong Man is active
        float finalImpulse = strongManActivated
            ? throwImpulse * strongManImpulseMultiplier
            : throwImpulse;

        objectRb.AddForce(transform.forward * finalImpulse, ForceMode.Impulse);

        // Tell ThrowableObject about the Strong Man state
        ThrowableObject throwable = heldObject.GetComponent<ThrowableObject>();
        if (throwable != null)
        {
            throwable.SetThrown(true);
            throwable.SetStrongManThrown(strongManActivated, strongManDestroyDelay);
        }

        heldObject = null;
    }

    public void ObjectRotation()
    {
        if (shouldRotate && heldObject != null)
        {
            heldObject.transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void StopClipping()
    {
        if (heldObject == null) return;

        var clipRange = Vector3.Distance(heldObject.transform.position, transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        if (hits.Length > 1)
        {
            heldObject.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }

    public void ClearHeldObject()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        isFlashing = false;
        holdTimer = 0f;
        heldObject = null;
        objectRb = null;
        lMouseHold = false;
        shouldRotate = false;
    }
}
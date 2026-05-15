using Unity.VisualScripting;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float sens = 150.0f;
    //[SerializeField] private float smoothing = 2.0f;
    [SerializeField] private float lookXLimit = 90.0f;
    [SerializeField] private float lookYLimit = 360f;
    [SerializeField] private float rotationX = 0;
    [SerializeField] private float rotationY = 0;
    //private Vector2 smoothMouseDelta;
    private bool canRotate;

    [Header("Reference")]
    [SerializeField] private GameObject player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraTransfrom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        canRotate = true;

        //init rotationX and rotationY from current transforms
        if (playerCamera != null)
        {
            rotationX = playerCamera.transform.localEulerAngles.x;
            if (rotationX > 180)
            {
                rotationX -= 360f;
            }
        }

        if (player != null)
        {
            rotationY = player.transform.eulerAngles.y;
            if (rotationY > 180)
            {
                rotationY -= 360f;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!canRotate)
        {
            return;
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens;

        AdjustRotation(mouseX, mouseY);

        // unlock the cursor by pressing Escape
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void AdjustRotation(float speedX, float speedY)
    {
        rotationX += -speedY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        rotationY += speedX;

        if (lookYLimit < 360f)
        {
            rotationY = Mathf.Clamp(rotationY, -lookYLimit, lookYLimit);
        }

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }

        if (player != null)
        {
            player.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        }
    }
}

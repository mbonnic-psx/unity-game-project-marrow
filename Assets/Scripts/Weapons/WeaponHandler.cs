using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private GameObject playerShotgun;
    [SerializeField] private float scrollCooldown = 0.3f;
    [SerializeField] private AttackSystem attackSystem;

    private bool hasShotgun = false;
    private bool isEquipped = false;
    private float lastToggleTime;

    public bool IsEquipped => isEquipped;
    public bool HasShotgun => hasShotgun;

    public void OnPickedUp()
    {
        hasShotgun = true;
        isEquipped = true;
    }

    void Update()
    {
        if (!hasShotgun) return;
        if (attackSystem != null && attackSystem.HeldObject != null) return; // can't equip while holding a throwable

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && Time.time - lastToggleTime >= scrollCooldown)
        {
            ToggleEquip();
            lastToggleTime = Time.time;
        }
    }

    private void ToggleEquip()
    {
        isEquipped = !isEquipped;
        playerShotgun.SetActive(isEquipped);
    }
}
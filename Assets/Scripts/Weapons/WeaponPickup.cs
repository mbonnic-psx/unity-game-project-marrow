using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private GameObject shotgunPickup;
    [SerializeField] private GameObject playerShotgun;
    [SerializeField] private WeaponHandler weaponHandler;

    void Start()
    {
        shotgunPickup.SetActive(true);
        playerShotgun.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            weaponHandler.OnPickedUp();   // notify FIRST
            shotgunPickup.SetActive(false);
            playerShotgun.SetActive(true);
        }
    }
}
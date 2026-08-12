using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Power : MonoBehaviour
{
    [SerializeField] private List<GameObject> powerParts;
    [SerializeField] private List<GameObject> placedParts;
    [SerializeField] private string layerName = "Power";
    [SerializeField] private AttackSystem attackSystem;
    private int layer;
    private bool[] delivered;

    [Header("POWER ON")]
    [SerializeField] private List<GameObject> powerDoors;
    [SerializeField] private List<Collider> perkColliders;
    [SerializeField] public bool powerFlag;

    void Awake()
    {
        layer = LayerMask.NameToLayer(layerName);
        delivered = new bool[powerParts.Count];

        foreach(GameObject door in powerDoors)
        door.SetActive(true);
        
        foreach(Collider perk in perkColliders)
        perk.enabled = false;

        // Make sure all placed visuals start hidden
        foreach (GameObject part in placedParts)
            part.SetActive(false);

        powerFlag = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && attackSystem.LMouseHold == true)
        {
            if (attackSystem.HeldObject.layer == layer)
            {
                int index = powerParts.IndexOf(attackSystem.HeldObject);

                if (index != -1 && !delivered[index])
                {
                    delivered[index] = true;
                    attackSystem.HeldObject.SetActive(false);  // remove from world
                    attackSystem.ClearHeldObject();                  // clear the hold in AttackSystem
                    placedParts[index].SetActive(true);        // show built piece on station

                    CheckAllPartsDelivered();
                }
            }
        }
    }

    private void CheckAllPartsDelivered()
    {
        foreach (bool part in delivered)
            if (!part) return;

        ActivatePowerStation();
    }

    private void ActivatePowerStation()
    {
        // Trigger your animation, sound, unlock door, etc. here
        Debug.Log("Power Station Activated!");

        foreach(GameObject door in powerDoors)
        door.SetActive(false);
        
        foreach(Collider perk in perkColliders)
        perk.enabled = true;

        powerFlag = true;

    }
}

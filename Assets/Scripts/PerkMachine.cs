using UnityEngine;
using UnityEngine.UI;

public class PerkMachine : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerUI points;
    [SerializeField] private PlayerStats health;
    [SerializeField] private PromptText promptText;

    [Header("Perk")]
    [SerializeField] private string perkID;
    [SerializeField] private int perkCost;
    [SerializeField] private Collider perkCollider;

    [Header("Values")]
    [SerializeField] private float armorValue = 165f;
    [SerializeField] private bool sliderActivated;
    [SerializeField] private AttackSystem attackSystem;

    [Header("UI")]
    [SerializeField] private GameObject armorPerkUI;
    [SerializeField] private GameObject smPerkUI;
    [SerializeField] private GameObject sliderPerkUI;

    public bool SliderActivated => sliderActivated;

    void Start()
    {
        //perkCollider.enabled = true;
        sliderActivated = false;
        attackSystem.StrongManActivated = false;
        armorPerkUI.SetActive(false);
        smPerkUI.SetActive(false);
        sliderPerkUI.SetActive(false);
    }

    public void BuyPerk()
    {
        switch (perkID)
        {
            case "Slider":
                if (points.KillCounter >= perkCost)
                {
                    Debug.Log("Slider Perk is Activated");
                    sliderActivated = true;
                    sliderPerkUI.SetActive(true);
                    promptText.enabled = false;
                    //perkCollider.enabled = false;
                }
                else
                {
                    return;
                }
                break;

            case "Armor":
                if (points.KillCounter >= perkCost)
                {
                    health.SetPlayerHealth(armorValue);
                    armorPerkUI.SetActive(true);
                    promptText.enabled = false;
                    //perkCollider.enabled = false;
                }
                else
                {
                    return;
                }
                break;

            case "Strong Man":
                if (points.KillCounter >= perkCost)
                {
                    Debug.Log("Strong Man Perk is Activated");
                    attackSystem.StrongManActivated = true;
                    smPerkUI.SetActive(true);
                    promptText.enabled = false;
                    //perkCollider.enabled = false;
                }
                else
                {
                    return;
                }
                break;

            case "Trickster":
                if (points.KillCounter >= perkCost)
                {
                    Debug.Log("Trickster Perk is Activated");
                    perkCollider.enabled = false;
                }
                else
                {
                    return;
                }
                break;
        }
    }
}

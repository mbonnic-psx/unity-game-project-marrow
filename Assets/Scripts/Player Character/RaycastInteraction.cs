using TMPro;
using UnityEngine;

public class RaycastInteraction : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private TextMeshProUGUI prompt;
    Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        prompt.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {

            PromptText promptText = hit.collider.GetComponent<PromptText>();
            if (promptText != null)
            {
                prompt.text = promptText.GetPrompt();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                switch (hit.collider.tag)
                {
                    case "Pickup":
                        hit.collider.GetComponent<Pickup>()?.PickedupItem();
                        break;
                    case "Dialogue":
                        hit.collider.GetComponent<InteractableDialogue>()?.TriggerDialogue();
                        break;
                    case "FrontDoor":
                        hit.collider.GetComponent<Door>()?.BuyDoor();
                        break;
                    case "BackDoor":
                        hit.collider.GetComponent<Door>()?.BuyDoor();
                        break;
                    case "Perk":
                        hit.collider.GetComponent<PerkMachine>()?.BuyPerk();
                        break;
                    case "Start":
                        hit.collider.GetComponent<SceneManager>()?.LoadGame(1);
                        break;
                    case "Quit":
                        hit.collider.GetComponent<SceneManager>()?.QuitGame();
                        break;
                }
            }
        }
        else
        {
            prompt.text = "";
        }
    }
}

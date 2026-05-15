using UnityEngine;

public class InteractableDialogue : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        Object.FindAnyObjectByType<DialogueManager>().StartDialogue(dialogue, this.gameObject);
    }
}

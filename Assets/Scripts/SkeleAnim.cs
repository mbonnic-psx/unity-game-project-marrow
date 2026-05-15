using UnityEngine;

public class SkeleAnim : MonoBehaviour
{
    [SerializeField] private string anim;
    [SerializeField] private string dialogueAnim;
    [SerializeField] private Animator animator;
    [SerializeField] private DialogueManager dialogueManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayAnimation(anim);
    }

    void Update()
    {
        if (dialogueManager.IsDialogueActive == true)
        {
            PlayAnimation(dialogueAnim);
        }
    }

    public void PlayAnimation(string animString)
    {
        animator.CrossFade(animString, 0.2f);
    }
}

using UnityEngine;

public class PromptText : MonoBehaviour
{
    [TextArea] public string prompt;

    public string GetPrompt()
    {
        return prompt;
    }
}

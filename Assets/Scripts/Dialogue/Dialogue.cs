using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(0, 10)]
    public string[] sentences;
}

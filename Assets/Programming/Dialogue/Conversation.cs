using UnityEngine;

[System.Serializable]
public struct Line
{
    public string name;
    [TextArea(1, 6)] public string text;
}

[CreateAssetMenu(fileName = "New Conversation", menuName = "Dialogue/Conversation")]
public class Conversation : ScriptableObject
{
    public Line[] lines;
    public Conversation nextConversation;
    public bool shouldNextChangeSpeakerConversation;
}

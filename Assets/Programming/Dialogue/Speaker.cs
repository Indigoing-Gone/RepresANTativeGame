using System;
using UnityEngine;

public class Speaker : MonoBehaviour
{
    [SerializeField] private Conversation conversation;

    //Signal when it is time to start a conversation
    public static event Action<Conversation> EnteringConversation;
    public event Action ConversationExited;

    public void EnterConversation()
    {
        EnteringConversation?.Invoke(conversation);

        DialogueController.ConversationEnded += OnConversationEnded;
        DialogueController.ChangeSpeakerConversation += ChangeConversation;
    }

    private void ChangeConversation(Conversation _newConversation)
    {
        conversation = _newConversation;
    }

    private void OnConversationEnded()
    {
        DialogueController.ConversationEnded -= OnConversationEnded;
        DialogueController.ChangeSpeakerConversation -= ChangeConversation;

        ConversationExited?.Invoke();
    }
}

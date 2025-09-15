using System;
using UnityEngine;

[RequireComponent(typeof(DialogueDisplay))]
public class DialogueController : MonoBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private DialogueDisplay dialogueUI;

    [SerializeField] private Conversation currentConversation;
    [SerializeField] private Conversation defaultConversation;

    private int activeLineIndex;
    private bool conversationStarted = false;
    private bool advanceDesired = true;

    //Broadcast when dialogue is started/ended
    public static event Action ConversationStarted;
    public static event Action ConversationEnded;

    //Broadcast 

    //Broadcast what the speaker's conversation should be changed to
    public static Action<Conversation> ChangeSpeakerConversation;

    private void OnEnable()
    {
        input.AdvanceEvent += HandleAdvance;
        Speaker.EnteringConversation += ChangeConversation;
    }

    private void OnDisable()
    {
        input.AdvanceEvent -= HandleAdvance;
        Speaker.EnteringConversation -= ChangeConversation;
    }

    private void Awake()
    {
        dialogueUI = GetComponent<DialogueDisplay>();
    }

    private void StartConversation()
    {
        conversationStarted = true;
        activeLineIndex = 0;
        dialogueUI.DisplayDialogue();

        ConversationStarted?.Invoke();
    }

    private void EndConversation()
    {
        if (currentConversation.shouldNextChangeSpeakerConversation) ChangeSpeakerConversation?.Invoke(currentConversation.nextConversation);

        currentConversation = defaultConversation;
        conversationStarted = false;
        dialogueUI.HideDialogue();

        ConversationEnded?.Invoke();
    }

    private void ChangeConversation(Conversation _nextConversation)
    {
        conversationStarted = false;
        currentConversation = _nextConversation;
        AdvanceLine();
    }

    private void AdvanceConversation()
    {
        if (currentConversation.nextConversation != null && !currentConversation.shouldNextChangeSpeakerConversation) ChangeConversation(currentConversation.nextConversation);
        else EndConversation();
    }

    private void AdvanceLine()
    {
        if (currentConversation == null) return;
        if (!conversationStarted) StartConversation();

        if (activeLineIndex < currentConversation.lines.Length)
        {
            dialogueUI.DisplayLine(currentConversation.lines[activeLineIndex]);
            activeLineIndex++;
        }
        else AdvanceConversation();
    }

    private void HandleAdvance(bool _state)
    {
        //Handle input to advance dialogue
        if (advanceDesired)
        {
            advanceDesired = false;
            AdvanceLine();
        }

        if (!_state) advanceDesired = true;
    }
}

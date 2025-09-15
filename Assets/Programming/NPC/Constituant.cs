using System;
using UnityEngine;

[RequireComponent(typeof(Speaker))]
public class Constituant : MonoBehaviour, IInteractable
{
    private Speaker speaker;

    private bool canStartTower;
    //Information about what blocks this ant has for the tower game

    public static event Action StartingTowerGame; //Modify to supply blocks

    private void OnEnable()
    {
        speaker.ConversationExited += StartTowerGame;
    }

    private void OnDisable()
    {
        speaker.ConversationExited -= StartTowerGame;
    }

    private void Awake()
    {
        speaker = GetComponent<Speaker>();
        canStartTower = true;
    }

    public void Interact()
    {
        speaker.EnterConversation();
    }

    private void StartTowerGame()
    {
        if (!canStartTower) return;

        StartingTowerGame?.Invoke();
        canStartTower = false;
    }
}

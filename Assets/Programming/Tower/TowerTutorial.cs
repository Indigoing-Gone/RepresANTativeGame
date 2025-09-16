using UnityEngine;

[RequireComponent(typeof(Speaker))]
public class TowerTutorial : MonoBehaviour
{
    private Speaker speaker;
    private bool tutorialCompleted;

    private void OnEnable()
    {
        StateManager.TowerStarted += StartTutorial;
    }

    private void OnDisable()
    {
        StateManager.TowerStarted -= StartTutorial;
    }

    private void Awake()
    {
        speaker = GetComponent<Speaker>();
        tutorialCompleted = false;
    }


    private void StartTutorial()
    {
        if (tutorialCompleted) return;
        speaker.EnterConversation();
        tutorialCompleted = true;
    }
}

using UnityEngine;

[RequireComponent(typeof(Speaker))]
public class IntroTutorial : MonoBehaviour
{
    private Speaker speaker;
    private bool tutorialCompleted;

    private void Start()
    {
        StartTutorial();
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

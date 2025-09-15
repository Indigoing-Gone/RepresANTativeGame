using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueDisplay : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private float textDisplaySpeed;

    private void Awake()
    {
        HideDialogue();
    }

    public void DisplayDialogue()
    {
        dialoguePanel.SetActive(true);
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    public void DisplayLine(Line _line)
    {
        if (dialogue == null) return;

        speakerName.text = _line.name;
        dialogue.text = "";

        StopAllCoroutines();
        StartCoroutine(Typewriter(_line.text));
    }

    private IEnumerator Typewriter(string _text)
    {
        foreach (char character in _text.ToCharArray())
        {
            dialogue.text += character;
            yield return new WaitForSeconds(textDisplaySpeed);
        }
    }
}

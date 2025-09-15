using UnityEngine;

public class InteractionDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popup;
    [SerializeField] private Vector3 offset;

    public void DisplayPopup(Collider2D _interactable)
    {
        popup.SetActive(true);

        Vector3 worldPos = _interactable.transform.position + offset;
        worldPos.y += _interactable.bounds.size.y;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        //screenPos.y += popup.GetComponent<RectTransform>().rect.height / 2;
        popup.transform.position = screenPos;
    }

    public void HidePopup()
    {
        popup.SetActive(false);
    }
}

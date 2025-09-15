using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Interactor : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InputReader input;
    [SerializeField] private InteractionDisplay interactionDisplay;
    private CircleCollider2D col;

    [Header("Parameters")]
    [SerializeField] private float detectRadius;
    [SerializeField] private Vector2 detectOffset;
    private List<Collider2D> interactableColliders;
    private Collider2D targetInteractable;

    private bool canInteract;

    private void OnEnable()
    {
        //Set up interaction detector
        col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = detectRadius;
        col.offset = detectOffset;

        //Subscribe to events
        input.InteractEvent += HandleInteract;

        interactableColliders = new();
        canInteract = true;
        targetInteractable = null;
    }

    private void OnDisable()
    {
        input.InteractEvent -= HandleInteract;
    }

    private void Update()
    {
        //Remove interactables from list if the interactable gets deleted while in range of the interactor
        for (int i = 0; i < interactableColliders.Count; i++)
            if (interactableColliders[i] == null)
                interactableColliders.RemoveAt(i);

        targetInteractable = interactableColliders.Count == 1 ? interactableColliders[0] : FindClosestInteractable();
        AttemptPopupDisplay();
    }

    private Collider2D FindClosestInteractable()
    {
        Collider2D _closest = null;
        float _lowestSqrDist = Mathf.Infinity;
        Vector3 _currentPos = transform.position;

        for (int i = 0; i < interactableColliders.Count; i++)
        {
            Vector3 _direction = interactableColliders[i].transform.position - _currentPos;
            float _curSqrDist = _direction.sqrMagnitude;
            if (_curSqrDist < _lowestSqrDist)
            {
                _lowestSqrDist = _curSqrDist;
                _closest = interactableColliders[i];
            }
        }

        return _closest;
    }

    private void AttemptPopupDisplay()
    {
        if (interactableColliders.Count > 0 && targetInteractable != null) interactionDisplay.DisplayPopup(targetInteractable);
        else interactionDisplay.HidePopup();
    }

    private void HandleInteract(bool _state)
    {
        //If you can interact and there is an interactable object, trigger the interactable
        if (canInteract)
        {
            canInteract = false;
            if (interactableColliders.Count == 0 || targetInteractable == null) return;
            targetInteractable.GetComponent<IInteractable>().Interact();
        }

        //Allow player to interact again when key is released
        if (!_state) canInteract = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Add interactables to list
        if (other.TryGetComponent<IInteractable>(out _)) interactableColliders.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Remove interactables from list
        if (interactableColliders.Contains(other)) interactableColliders.Remove(other);
    }
}

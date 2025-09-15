using System;
using UnityEngine;

public class PlayerGround : MonoBehaviour
{
    [Header("States")]
    private bool onGround;
    public bool OnGround { get => onGround; }

    [Header("Raycast Settings")]
    [SerializeField] private float raycastLength;
    [SerializeField] private Vector3 raycastSpacing;
    [SerializeField] private LayerMask groundLayer;

    private void Update()
    {
        onGround = CheckGrounded();
    }

    private bool CheckGrounded()
    {
        (Vector3 origin1, Vector3 origin2, Vector3 down) = GetGroundRayInfo();

        //Run a raycast pointing downwards based on parameters
        return Physics2D.Raycast(origin1, down, raycastLength, groundLayer) ||
               Physics2D.Raycast(origin2, down, raycastLength, groundLayer);
    }

    private (Vector3, Vector3, Vector3) GetGroundRayInfo()
    {
        //Get down and spacing, both relative to current rotation of player
        //Means that the raycast is always relative to the players "feet"
        Vector3 down = -transform.up;
        Vector3 rotatedSpacing = transform.rotation * raycastSpacing;

        //Set origins of the two raycasts 
        Vector3 origin1 = transform.position + rotatedSpacing;
        Vector3 origin2 = transform.position - rotatedSpacing;

        return (origin1, origin2, down);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = onGround ? Color.green : Color.red;

        (Vector3 origin1, Vector3 origin2, Vector3 down) = GetGroundRayInfo();

        Gizmos.DrawLine(origin1, origin1 + down * raycastLength);
        Gizmos.DrawLine(origin2, origin2 + down * raycastLength);
    }
}

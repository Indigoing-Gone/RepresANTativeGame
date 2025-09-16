using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{

    // Constants
    public float rotationSpeed;
    private Vector4 bounds;
    public float height;

    // Components
    private Rigidbody2D rb;
    private TargetJoint2D joint;

    // Input
    bool clicked = false;
    public bool Clicked => clicked;
    private Vector2 mousePos;

    // Collision
    public bool touchingGround;

    // Material
    public Material[] mats;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<TargetJoint2D>();
        joint.enabled = false;
        bounds = TowerManager.Instance.bounds;
        GetComponent<SpriteRenderer>().material = mats[Random.Range(0, mats.Length)];
    }

    // When block is clicked
    private void OnClick()
    {
        clicked = true;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        joint.enabled = true;
    }

    // When block is released
    private void OnRelease()
    {
        clicked = false;
        joint.enabled = false;
        rb.constraints = RigidbodyConstraints2D.None;
    }

    // Rotation
    private void Rotate(float theta)
    {
        if (clicked)
        {
            transform.eulerAngles += new Vector3(0.0f, 0.0f, theta);
        }
    }

    // Collision handling

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground")) touchingGround = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground")) touchingGround = false;
    }

    // Basic Input
    // TODO: Change to input system 
    private void OnMouseDown()
    {
        OnClick();
    }
    
    private void OnMouseUp()
    {
        OnRelease();
    }

    // Update is called once per frame
    void Update()
    {
        if (clicked)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            joint.target = mousePos;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bounds[0], bounds[1]),
                                         Mathf.Clamp(transform.position.y, bounds[2], bounds[3]));

        // TODO: REPLACE THIS
        int rotation = 0;
        if (Input.GetKey(KeyCode.A)) rotation++;
        if (Input.GetKey(KeyCode.D)) rotation--;
        if (rotation != 0) Rotate(rotation * rotationSpeed * Time.deltaTime);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Robot : MonoBehaviour
{
    // Constant
    private float jumpForce = 6f;
    private float moveSpeed = 7f;

    // Player variables
    private Rigidbody2D rb;
    private Animator anim;

    // Components and states
    private bool isDead = false;

    private bool isGrounded;
    public Transform groundCheck;
    private float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    private float horizontalInput;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            Debug.Log("Robot is dead. Cannot jump.");
        }

        // Jump only if space is pressed AND the robot is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        horizontalInput = 0f;
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            // Gauche : Q 
            if (keyboard.aKey.isPressed)
            {
                horizontalInput -= 1f;
            }

            // Droite : D 
            if (keyboard.dKey.isPressed)
            {
                horizontalInput += 1f;
            }

            // Saut : Z
            bool jumpPressed = keyboard.zKey.wasPressedThisFrame ||
                               keyboard.wKey.wasPressedThisFrame ||
                               keyboard.upArrowKey.wasPressedThisFrame;

            if (jumpPressed && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
}

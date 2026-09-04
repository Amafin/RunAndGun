using UnityEngine;
using UnityEngine.InputSystem;

public class Robot : MonoBehaviour
{
    // Movement
    private float moveSpeed = 7f;
    private float jumpForce = 6f;

    // Ground check
    public Transform groundCheck;
    private float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // Shooting
    public GameObject bulletPrefab;
    private float shootDuration = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead = false;
    private bool isGrounded;
    private float horizontalInput;
    private float verticalInput;
    private int facingDirection = 1;

    private bool isShooting = false;
    private float shootTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        // Détection du sol
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        // Déplacements ZQSD
        if (keyboard != null)
        {
            horizontalInput = 0f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontalInput += 1f;
            if (keyboard.qKey.isPressed || keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontalInput -= 1f;

            verticalInput = 0f;
            if (keyboard.zKey.isPressed || keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) verticalInput += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) verticalInput -= 1f;

            // Saut
            if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        // Déclenchement du tir
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }

        // Gestion du chrono de tir
        if (isShooting)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                isShooting = false;
            }
        }

        // Orientation du robot
        if (horizontalInput > 0 && !isShooting)
        {
            facingDirection = 1;
        }
        else if (horizontalInput < 0 && !isShooting)
        {
            facingDirection = -1;
        }

        transform.localScale = new Vector3(facingDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // Envoi des variables à l'Animator
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetBool("IsShooting", isShooting);
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Shoot()
    {
        if (bulletPrefab == null) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        // Position de départ
        Vector3 spawnPos = transform.position;

        // Position souris dans le monde
        Vector3 mouseScreenPos = mouse.position.ReadValue();
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // Direction du tir
        Vector2 shootDirection = (mouseWorldPos - spawnPos).normalized;

        // Oriente immédiatement le robot vers le clic
        facingDirection = shootDirection.x >= 0 ? 1 : -1;
        transform.localScale = new Vector3(facingDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // Active l'animation de tir
        isShooting = true;
        shootTimer = shootDuration;

        // Instanciation de la balle
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Launch(shootDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
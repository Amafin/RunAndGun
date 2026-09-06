using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Settings")]
    private float speed = 1.5f;
    private float attackCooldown = 1.2f;
    private float deathDelay = 2.5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D col;


    private bool isDead = false;
    private float lastAttackTime = -999f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Attention : Aucun GameObject avec le tag 'Player' n'a été trouvé !");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        if (isDead || player == null) return;

        float direction = player.position.x > transform.position.x ? 1f : -1f;

        if (Mathf.Abs(player.position.x - transform.position.x) > 0.8f)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;

                if (anim != null) anim.SetTrigger("isAttacking");

                Robot robot = collision.gameObject.GetComponent<Robot>();
                if (robot != null)
                {
                    robot.TakeDamage(1);
                }
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (col != null) col.enabled = false;

        if (anim != null) anim.SetBool("isDead", true);

        Destroy(gameObject, deathDelay);
    }
}
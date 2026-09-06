using UnityEngine;

public class ZombieF : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float deathDelay = 2.5f;
    private bool isAttackingAnim = false;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D col;

    private bool isDead = false;
    private float lastAttackTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void LateUpdate()
    {
        if (isDead || player == null) return;

        Vector3 scale = transform.localScale;
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
        }
    }

    void FixedUpdate()
    {
        if (isDead || player == null || isAttackingAnim) return;

        float dir = player.position.x > transform.position.x ? 1f : -1f;

        if (Mathf.Abs(player.position.x - transform.position.x) > 0.8f)
        {
            rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
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

                StartCoroutine(AttackRoutine());

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

        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (col != null) col.enabled = false;

        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isDead", true);
        }

        Destroy(gameObject, deathDelay);
    }

    private System.Collections.IEnumerator AttackRoutine()
    {
        isAttackingAnim = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // Active l'animation
        if (anim != null) anim.SetBool("isAttacking", true);

        yield return new WaitForSeconds(0.6f);

        // Éteint l'animation pour revenir à Idle
        if (anim != null) anim.SetBool("isAttacking", false);

        isAttackingAnim = false;
    }
}
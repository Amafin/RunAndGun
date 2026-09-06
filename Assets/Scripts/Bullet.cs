using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 15f;
    private float lifeTime = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 direction)
    {
        Vector2 dir = direction.normalized;
        rb.linearVelocity = dir * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        ZombieF zombie = collision.GetComponent<ZombieF>();
        if (zombie != null)
        {
            Debug.Log("Zombie touché !");
            zombie.Die();
            Destroy(gameObject);
            return;
        }

        Ninja ninja = collision.GetComponent<Ninja>();
        if (ninja != null)
        {
            ninja.Die();
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
using System.Collections;
using UnityEngine;

public class Ninja : MonoBehaviour
{
    [Header("Combat")]
    public GameObject kunaiPrefab;
    private float fireRate = 2f;
    private float shootAnimDuration = 0.5f;
    private float deathDelay = 2f;

    private Transform player;
    private Animator anim;
    private Collider2D col;

    private bool isDead = false;
    private float shootTimer = 0f;

    private SpriteRenderer sr;

    private bool IsInMainCameraView()
    {
        if (Camera.main == null) return false;

        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewPos.z > 0 && viewPos.x >= 0f && viewPos.x <= 1f && viewPos.y >= 0f && viewPos.y <= 1f;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        shootTimer = fireRate;
    }

    void LateUpdate()
    {
        if (isDead || player == null) return;

        Vector3 scale = transform.localScale;

        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
        }
    }

    void Update()
    {
        if (isDead || player == null || !IsInMainCameraView()) return;

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            shootTimer = fireRate;
            StartCoroutine(ThrowKunaiSequence());
        }
    }

    private IEnumerator ThrowKunaiSequence()
    {
        if (anim != null) anim.SetBool("isShooting", true);

        yield return new WaitForSeconds(0.2f);

        if (!isDead && player != null && kunaiPrefab != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;

            Vector3 spawnPos = transform.position + new Vector3(dir.x * 0.6f, 0.2f, 0);
            GameObject kunai = Instantiate(kunaiPrefab, spawnPos, Quaternion.identity);

            Kunai kunaiScript = kunai.GetComponent<Kunai>();
            if (kunaiScript != null)
            {
                kunaiScript.Launch(dir);
            }
        }

        yield return new WaitForSeconds(shootAnimDuration - 0.2f);
        if (anim != null) anim.SetBool("isShooting", false);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();
        if (col != null) col.enabled = false;

        if (anim != null)
        {
            anim.SetBool("isShooting", false);
            anim.SetBool("isDead", true);
        }

        Destroy(gameObject, deathDelay);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public float stunDuration = 1.5f;
    private bool isStunned = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color stunnedColor;

    private Rigidbody2D rb;

    private CinemachineImpulseSource impulseSource;

    public GameObject deathParticle;

    public Slider healthBarSlider;
    public TextMeshProUGUI healthText;

    public PlayableDirector playableDirector;

    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    private bool canAttack = true;

    private Transform player;
    private bool isPlayerInRange = false;
    private bool isPlayerTooClose = false;

    public float followRange = 10f;
    public float attackCloseRange = 1.5f;
    public float followSpeed = 2f;

    public float knockbackForce = 10f;

    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        impulseSource = GetComponent<CinemachineImpulseSource>();

        player = GameObject.FindWithTag("Player").transform;

        animator = GetComponent<Animator>();

        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = health;
            healthBarSlider.value = health;
        }

        if (healthText != null)
        {
            healthText.text = health + " / " + health;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followRange)
        {
            isPlayerInRange = true;
        }
        else
        {
            isPlayerInRange = false;
        }

        if (isPlayerInRange && !isStunned)
        {
            FollowPlayer();
        }

        if (isPlayerInRange && distanceToPlayer <= attackCloseRange && canAttack)
        {
            isPlayerTooClose = true;
            StartCoroutine(Attack());
        }
        else
        {
            isPlayerTooClose = false;
        }
    }

    public void TakeDamage(int damage)
    {
        CameraShake.instance.CameraShaking(impulseSource);

        if (!isStunned)
        {
            StartCoroutine(Stun());
        }

        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage!");

        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
        }

        if (healthText != null)
        {
            healthText.text = health + " / " + health;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator Stun()
    {
        isStunned = true;

        spriteRenderer.color = stunnedColor;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(stunDuration);

        spriteRenderer.color = originalColor;
        isStunned = false;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been defeated!");
        Instantiate(deathParticle, transform.position, Quaternion.identity);

        if (playableDirector != null)
        {
            playableDirector.Play();
        }

        Destroy(gameObject);
    }

    private IEnumerator Attack()
    {
        canAttack = false;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        Debug.Log("Enemy is attacking!");

        Vector2 knockbackDirection = (player.position - transform.position).normalized;

        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage((int)attackDamage);
            Debug.Log("Player took damage from enemy!");

            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    private void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * followSpeed, rb.velocity.y);
    }

    public IEnumerator Erasure(float duration)
    {
        if (isStunned) yield break;

        isStunned = true;

        rb.velocity = Vector2.zero;
        spriteRenderer.color = stunnedColor;

        yield return new WaitForSeconds(duration);

        isStunned = false;
        spriteRenderer.color = originalColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cinemachine;

public class Saitama : MonoBehaviour, IAbility
{
    private PlayerStats playerStats;
    public GameObject deathParticle;
    public GameObject bigHit;
    public GameObject player;

    private bool isActive = false;
    private bool isOnCooldown = false;
    private float damageMultiplier = 50f;
    private float knockbackMultiplier = 5f;
    public float punchRadius = 8f;
    private float cooldownDuration = 5f;

    private Animator animator;
    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        animator = player.GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        player = GameObject.FindWithTag("Player");
    }

    public void Activate(float duration)
    {
        if (isActive || isOnCooldown) return;

        Debug.Log("[MYTHICAL] You got Saitama's ability!");
        isActive = true;

        if (playerStats != null)
        {
            playerStats.UpdateStats(1f, damageMultiplier);
        }
    }

    public void ApplyEffect(ref int damage, ref float knockback)
    {
        if (!isActive) return;

        damage = Mathf.RoundToInt(damage * damageMultiplier);
        knockback *= knockbackMultiplier;

        Deactivate();
    }

    public void Deactivate()
    {
        if (isActive)
        {
            playerStats.ResetStats();
            isActive = false;
            Debug.Log("Saitama's ability has ended");
        }
    }

    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.K))
        {
            NormalPunch();
            Deactivate();
        }

        if (isActive && Input.GetKeyDown(KeyCode.L))
        {
            OnePunch();
            Deactivate();
        }
    }

    private bool NormalPunch()
    {
        bool enemyHit = false;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, punchRadius);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                animator.SetTrigger("Attack1");

                GameObject hitEffect = Instantiate(bigHit, enemy.transform.position, Quaternion.identity);

                hitEffect.transform.rotation = Quaternion.Euler(0, 0, 45);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

                int damage = Mathf.RoundToInt(playerStats.damage * damageMultiplier);
                enemyScript.TakeDamage(damage);
                Debug.Log("DEATH");
                enemyHit = true;

                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                float knockbackForce = 50f * knockbackMultiplier;
                enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                CameraShake.instance.CameraShaking(impulseSource);
            }
        }

        return enemyHit;
    }

    private void OnePunch()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, punchRadius);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                animator.SetTrigger("Attack1");

                GameObject hitEffect = Instantiate(bigHit, enemy.transform.position, Quaternion.identity);

                hitEffect.transform.rotation = Quaternion.Euler(0, 0, 45);

                Enemy enemyScript = enemy.GetComponent<Enemy>();

                CameraShake.instance.CameraShaking(impulseSource);
                Destroy(enemy.gameObject);
                Instantiate(deathParticle, enemy.transform.position, Quaternion.identity);
                Debug.Log("Killer Move: Serious Punch");
            }
        }
    }

    private IEnumerator StartCooldown()
    {
        isOnCooldown = true;
        Debug.Log("K Key cooldown started");

        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
        Debug.Log("K Key cooldown ended");
    }
}

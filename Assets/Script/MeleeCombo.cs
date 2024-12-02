using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCombo : MonoBehaviour
{
    public GameObject hitbox1, hitbox2, hitbox3, hitbox4;
    public GameObject hitEffectPrefab;
    public float baseComboResetTime = 1.0f;
    public float baseComboCooldown = 1.5f;
    public float attackDelay = 0.2f;

    private int currentCombo = 0;
    private float comboTimer = 0f;
    private bool isComboing = false;
    private bool isOnCooldown = false;
    private bool isAttackLocked = false;

    public float knockbackForce1 = 10f;
    public float knockbackForce2 = 13f;
    public float knockbackForce3 = 17f;
    public float knockbackForce4 = 75f;

    public AbilityManager abilityManager;

    private Rigidbody2D rb;
    public Animator animator;
    private PlayerStats playerStats;
    private Hardened hardenedScript;

    private float comboResetTime; // Adjusted reset time
    private float comboCooldown; // Adjusted cooldown

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        DisableAllHitboxes();

        playerStats = GetComponent<PlayerStats>();

        hardenedScript = GetComponent<Hardened>(); // Get reference to Hardened script

        // Initialize cooldown values
        comboResetTime = baseComboResetTime;
        comboCooldown = baseComboCooldown;
    }

    void Update()
    {
        if (isOnCooldown || isAttackLocked) return;

        AdjustCooldownForSpeedBoost();

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isComboing || currentCombo == 0)
            {
                StartCoroutine(PerformCombo());
            }
        }

        if (isComboing)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboResetTime)
            {
                ResetCombo();
            }
        }
    }

    void AdjustCooldownForSpeedBoost()
    {
        if (abilityManager != null && abilityManager.IsAbilityActive("SpeedBoost"))
        {
            comboResetTime = baseComboResetTime * 0.5f;
            comboCooldown = baseComboCooldown * 0.5f;
        }
        else
        {
            comboResetTime = baseComboResetTime;
            comboCooldown = baseComboCooldown;
        }
    }

    IEnumerator PerformCombo()
    {
        isAttackLocked = true;
        currentCombo++;
        comboTimer = 0f;
        isComboing = true;

        float knockbackMultiplier = hardenedScript != null && hardenedScript.isHealthBoosted ? 2f : 1f;

        switch (currentCombo)
        {
            case 1:
                ActivateHitbox(hitbox1, playerStats.damage, true, knockbackForce1 * knockbackMultiplier);
                animator.SetTrigger("Attack1");
                break;

            case 2:
                ActivateHitbox(hitbox2, playerStats.damage + 5, true, knockbackForce2 * knockbackMultiplier);
                animator.SetTrigger("Attack2");
                break;

            case 3:
                ActivateHitbox(hitbox3, playerStats.damage + 10, true, knockbackForce3 * knockbackMultiplier);
                animator.SetTrigger("Attack3");
                break;

            case 4:
                int damage = playerStats.damage + 20;
                float knockback = knockbackForce4 * knockbackMultiplier;

                if (abilityManager != null)
                {
                    abilityManager.ApplyFinalAttackBoost(ref damage, ref knockback);
                }

                ActivateHitbox(hitbox4, damage, true, knockback);
                animator.SetTrigger("Attack4");
                EndCombo();
                break;

            default:
                ResetCombo();
                break;
        }

        yield return new WaitForSeconds(attackDelay);
        isAttackLocked = false;
    }

    void ActivateHitbox(GameObject hitbox, int damage, bool applyKnockback = false, float knockback = 0f)
    {
        DisableAllHitboxes();
        hitbox.SetActive(true);

        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(hitbox.transform.position, hitbox.GetComponent<BoxCollider2D>().size, 0f);
        foreach (Collider2D enemy in enemiesHit)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                enemyScript?.TakeDamage(damage);

                Instantiate(hitEffectPrefab, hitbox.transform.position, Quaternion.identity);

                if (applyKnockback)
                {
                    Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                    if (enemyRb != null)
                    {
                        Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                        enemyRb.AddForce(knockbackDirection * knockback, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }


    void DisableAllHitboxes()
    {
        hitbox1.SetActive(false);
        hitbox2.SetActive(false);
        hitbox3.SetActive(false);
        hitbox4.SetActive(false);
    }

    void ResetCombo()
    {
        currentCombo = 0;
        isComboing = false;
        DisableAllHitboxes();
        StartCoroutine(StartComboCooldown());
    }

    void EndCombo()
    {
        isComboing = false;
        currentCombo = 0;
        DisableAllHitboxes();
        StartCoroutine(StartComboCooldown());
    }

    private IEnumerator StartComboCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(comboCooldown);
        isOnCooldown = false;
    }
}

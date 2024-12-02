using System.Collections;
using UnityEngine;

public class Hardened : MonoBehaviour, IAbility
{
    private PlayerStats playerStats;
    public GameObject player;

    public bool isActive = false;
    public bool isHealthBoosted = false;
    private bool isGroundSlamCooldown = false;

    [SerializeField] private SpriteRenderer target;
    public Color hardenedColor = Color.gray;
    private Color origColor;

    public GameObject groundslamEffect;
    public float groundSlamRadius = 5f;
    public float groundSlamDamage = 20f;
    public float groundSlamCooldown = 3f;

    private float unbreakabledura = 20f;
    public GameObject unbreakableEffect;

    private Animator anim;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        player = GameObject.FindWithTag("Player");
        anim = player.GetComponent<Animator>();

        origColor = target.color;
    }

    public void Activate(float duration)
    {
        if (isActive) return;

        Debug.Log("[UNCOMMON] You got Kirishima's ability!");
        isActive = true;

        playerStats.UpdateStats(2f, 2f);

        StartCoroutine(HardenedEffect(duration));
    }

    public void Deactivate()
    {
        if (!isActive) return;

        Debug.Log("Kirishima's ability has ended");
        
        playerStats.ResetStats();

        target.color = origColor;

        isActive = false;
        isHealthBoosted = false;
    }

    public void ApplyEffect(ref int damage, ref float knockback)
    {
        // leave it blank so that the script works
    }

    private IEnumerator HardenedEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        Deactivate();
    }

    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.L) && !isHealthBoosted)
        {
            Unbreakable();
        }

        if (isActive && Input.GetKeyDown(KeyCode.K) && !isGroundSlamCooldown)
        {
            PerformGroundSlam();
        }
    }

    private void Unbreakable()
    {
        if (playerStats != null && !isHealthBoosted)
        {
            Debug.Log("RED RIOT: UNBREAKABLE!");
            Instantiate(unbreakableEffect, transform.position, Quaternion.identity);
            playerStats.health *= 2;
            isHealthBoosted = true;

            if (target != null)
            {
                target.color = hardenedColor;
            }

            StartCoroutine(UnbreakableDuration());
        }
    }

    private IEnumerator UnbreakableDuration()
    {
        yield return new WaitForSeconds(unbreakabledura);

        playerStats.health /= 2;

        target.color = origColor;

        isHealthBoosted = false;
        Debug.Log("Unbreakable has ended");
    }

    private void PerformGroundSlam()
    {
        anim.SetTrigger("GroundSlam");

        Debug.Log("Ground Slam Activated!");

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, groundSlamRadius);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

                Instantiate(groundslamEffect, transform.position, Quaternion.identity);

                int damage = Mathf.RoundToInt(playerStats.damage + groundSlamDamage);
                enemyScript.TakeDamage(damage);

                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                float knockbackForce = 10f;
                enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }

        StartCoroutine(GroundSlamCooldown());
    }

    private IEnumerator GroundSlamCooldown()
    {
        isGroundSlamCooldown = true;
        yield return new WaitForSeconds(groundSlamCooldown);
        isGroundSlamCooldown = false;
        Debug.Log("Ground Slam is ready!");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erasure : MonoBehaviour, IAbility
{
    private bool isActive = false;
    private bool isStunned = false;

    [SerializeField] private SpriteRenderer target;
    public Color stunnedColor;
    private Color origColor;

    private float stunDuration = 10f;
    public GameObject erasureEffect;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        origColor = target.color;
    }

    public void Activate(float duration)
    {
        if (isActive) return;

        Debug.Log("[EPIC] You got Aizawa's ability!");
        isActive = true;

        StartCoroutine(ErasureCountDown(duration));

        StartCoroutine(StunEffect());
    }

    public void Deactivate()
    {
        if (!isActive) return;

        Debug.Log("Aizawa's ability has ended");
        isActive = false;
        isStunned = false;

        if (target != null)
        {
            target.color = origColor;
        }
    }

    public void ApplyEffect(ref int damage, ref float knockback)
    {
        //leave it blank so that the script works
    }

    private IEnumerator ErasureCountDown(float duration)
    {
        yield return new WaitForSeconds(duration);
        Deactivate();
    }

    private IEnumerator StunEffect()
    {
        while (isActive)
        {
            if (Input.GetKeyDown(KeyCode.K) && !isStunned)
            {
                Debug.Log("ERASURE!");
                Vector3 offset = new Vector3(0, 3, 0);
                Instantiate(erasureEffect, transform.position + offset, Quaternion.identity);

                isStunned = true;

                if (target != null)
                {
                    target.color = stunnedColor;
                }

                StunNearbyEnemies();

                yield return new WaitForSeconds(stunDuration);

                if (target != null)
                {
                    target.color = origColor;
                }

                isStunned = false;
            }
            yield return null;
        }
    }

    private void StunNearbyEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 20f);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    StartCoroutine(enemyScript.Erasure(stunDuration));
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;
    public int damage = 10;
    public Slider slider;

    private float healthRegenerationRate = 2f;
    private int healthRegenerationAmount = 5;

    public void Start()
    {
        health = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = health;

        StartCoroutine(HealthRegeneration());
    }

    public void UpdateStats(float healthMultiplier, float damageMultiplier)
    {
        health *= (int)healthMultiplier;
        damage *= (int)damageMultiplier;
    }

    public void ResetStats()
    {
        health = 100;
        damage = 10;
    }

    public void ModifyDamage(float amount)
    {
        damage += (int)amount;
    }

    public void TakeDamage(int amount)
    {
        health -= (int)amount;
        slider.value = health;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator HealthRegeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(healthRegenerationRate);

            if (health < maxHealth)
            {
                health += healthRegenerationAmount;
                if (health > maxHealth)
                {
                    health = maxHealth;
                }

                slider.value = health;
            }
        }
    }
}

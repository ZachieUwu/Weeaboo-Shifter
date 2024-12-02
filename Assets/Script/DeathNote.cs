using System.Collections;
using UnityEngine;
using Cinemachine;

public class InstantDestroy : MonoBehaviour, IAbility
{
    private bool isActive = false;

    public GameObject deathParticle;
    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.K))
        {
            if (DestroyNearbyEnemies())
            {
                CameraShake.instance.CameraShaking(impulseSource);
                Deactivate();
            }
            else
            {
                Deactivate();
                Debug.Log("No enemies nearby, ability ended.");
            }
        }
    }

    public void Activate(float duration)
    {
        if (isActive) return;

        Debug.Log("[MYTHICAL] You got Light Yagami's ability!");
        isActive = true;
    }

    public void Deactivate()
    {
        if (!isActive) return;

        Debug.Log("Light Yagami's ability has ended");
        isActive = false;
    }

    public void ApplyEffect(ref int damage, ref float knockback)
    {
        //leave it blank so that the script works
    }

    private bool DestroyNearbyEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f);

        bool anyDestroyed = false;

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Instantiate(deathParticle, enemy.transform.position, Quaternion.identity);
                Destroy(enemy.gameObject);
                Debug.Log("When you die, the one who'll write your name down in a notebook will be me");
                anyDestroyed = true;
            }
        }

        return anyDestroyed;
    }

    private IEnumerator DeactivateEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        Deactivate();
    }
}

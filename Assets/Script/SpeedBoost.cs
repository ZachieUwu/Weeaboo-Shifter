using System.Collections;
using UnityEngine;

public class SpeedBoost : MonoBehaviour, IAbility
{
    private MovementControl movementControl;
    private bool isActive = false;

    [SerializeField] private TrailRenderer handOne;
    [SerializeField] private TrailRenderer handTwo;

    void Start()
    {
        movementControl = FindObjectOfType<MovementControl>();
    }

    public void Activate(float duration)
    {
        if (isActive) return;

        Debug.Log("[LEGENDARY] You got Garou's ability!");
        isActive = true;

        handOne.emitting = true;
        handTwo.emitting = true;

        if (movementControl != null)
        {
            movementControl.speed *= 2;
        }

        StartCoroutine(SpeedBoostEffect(duration));
    }

    public void Deactivate()
    {
        if (!isActive) return;

        Debug.Log("Garou's ability has ended");
        if (movementControl != null)
        {
            movementControl.speed /= 2;
        }

        isActive = false;

        handOne.emitting = false;
        handTwo.emitting = false;
    }

    private IEnumerator SpeedBoostEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        Deactivate();
    }

    public void ApplyEffect(ref int damage, ref float knockback)
    {
        //leave it blank so that the script works
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityManager : MonoBehaviour
{
    public float abilityDuration = 10f;

    public IAbility currentAbility;
    private bool isAbilityActive = false;
    private bool isRngBoxCollected = false;

    private SpriteRenderer spriteRenderer;

    [System.Serializable]
    public class AbilityChance
    {
        public MonoBehaviour ability;
        public float chance;
        public string abilityName;
        public Color rarityColor;
    }

    public List<AbilityChance> abilitiesWithChances;

    public GameObject rngBoxObject;
    public Image rngBoxImage;
    public Color activeColor;
    private Color defaultColor;

    public TextMeshProUGUI abilityNameText;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rngBoxImage != null)
        {
            defaultColor = rngBoxImage.color;
            rngBoxImage.color = Color.black;
        }

        if (abilityNameText != null)
        {
            abilityNameText.text = "Ability:";
            abilityNameText.color = Color.white;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && isRngBoxCollected && !isAbilityActive)
        {
            AssignRandomAbility();
        }
    }

    public void CollectRngBox()
    {
        if (!isRngBoxCollected && rngBoxObject != null)
        {
            rngBoxObject.SetActive(false);

            rngBoxImage.color = defaultColor;

            isRngBoxCollected = true;
        }
    }

    void AssignRandomAbility()
    {
        if (currentAbility != null)
        {
            currentAbility.Deactivate();
            currentAbility = null;
        }

        float totalChance = 0f;
        foreach (var abilityChance in abilitiesWithChances)
        {
            totalChance += abilityChance.chance;
        }

        float randomRoll = Random.Range(0f, totalChance);

        float cumulativeChance = 0f;
        foreach (var abilityChance in abilitiesWithChances)
        {
            cumulativeChance += abilityChance.chance;
            if (randomRoll <= cumulativeChance)
            {
                currentAbility = (IAbility)abilityChance.ability;

                UpdateAbilityNameText(abilityChance.abilityName, abilityChance.rarityColor);
                break;
            }
        }

        if (currentAbility != null)
        {
            currentAbility.Activate(abilityDuration);
            isAbilityActive = true;

            UpdateRngBoxColor(true);

            StartCoroutine(ResetAbilityStatus(abilityDuration));
        }
    }

    private IEnumerator ResetAbilityStatus(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (currentAbility != null)
        {
            currentAbility.Deactivate();
            currentAbility = null;
        }

        isAbilityActive = false;

        UpdateRngBoxColor(false);

        UpdateAbilityNameText("", Color.white);
    }

    private void UpdateRngBoxColor(bool isActive)
    {
        if (rngBoxImage != null)
        {
            rngBoxImage.color = isActive ? activeColor : defaultColor;
        }
    }

    private void UpdateAbilityNameText(string abilityName, Color rarityColor)
    {
        if (abilityNameText != null)
        {
            abilityNameText.text = "Ability: " + abilityName;
            abilityNameText.color = rarityColor;
        }
    }

    public bool IsAbilityActive(string abilityName)
    {
        return currentAbility != null && currentAbility.GetType().Name == abilityName;
    }

    public void ApplyFinalAttackBoost(ref int damage, ref float knockback)
    {
        if (currentAbility != null)
        {
            currentAbility.ApplyEffect(ref damage, ref knockback);
        }
    }
}

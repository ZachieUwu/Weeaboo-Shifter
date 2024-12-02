using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    void Activate(float duration);
    void Deactivate();
    void ApplyEffect(ref int damage, ref float knockback);
}

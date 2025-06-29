using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeaponData
{
    public WeaponSO weapon;
    [Range(0f, 1f)] public float trust;

    public List<CombatActionSO> GetBasics() => weapon.GetAvailableBasics(trust);
    public List<CombatActionSO> GetSkills() => weapon.GetAvailableSkills(trust);
    public CombatActionSO GetBurst() => weapon.GetBurst(trust);
}

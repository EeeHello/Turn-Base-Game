using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WeaponType { Melee, Ranged, Magic }

[CreateAssetMenu(menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public WeaponType type;

    public List<UnlockableAction> basicActions;
    public List<UnlockableAction> skillActions;
    public CombatActionSO baseBurst;
    public CombatActionSO maxTrustBurst;

    public List<CombatActionSO> GetAvailableBasics(float trust) =>
        basicActions.Where(a => trust >= a.requiredTrust).Select(a => a.action).ToList();

    public List<CombatActionSO> GetAvailableSkills(float trust) =>
        skillActions.Where(a => trust >= a.requiredTrust).Select(a => a.action).ToList();

    public CombatActionSO GetBurst(float trust) =>
        trust >= 1f ? maxTrustBurst : baseBurst;
}

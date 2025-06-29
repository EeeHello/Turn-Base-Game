using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Actions/Damage Action")]
public class DamageActionSO : CombatActionSO
{
    public override void Execute(GameObject user, GameObject target)
    {
        Debug.Log($"{user.name} used {actionName} on {target.name}, dealing {damage} damage!");
        // Add damage logic here
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerTemplate", menuName = "Player/Template")]
public class PlayerTemplateSO : ScriptableObject
{
    public Stats baseStats;

    public Ability[] startingAbilities;
    public Effect[] startingEffects;

    public List<WeaponSO> startingWeapons;
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerCombatBuild
{
    public List<CombatActionSO> chosenBasics = new(3);
    public List<CombatActionSO> chosenSkills = new(3);
    public CombatActionSO chosenBurst;
}

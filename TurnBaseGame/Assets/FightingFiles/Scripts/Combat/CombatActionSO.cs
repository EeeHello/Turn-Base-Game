using UnityEngine;

public enum ActionType { Basic, Skill, Burst }

public abstract class CombatActionSO : ScriptableObject
{
    public string actionName;
    public Sprite icon;
    public ActionType actionType;
    public int damage;
    public int spCost;

    public abstract void Execute(GameObject user, GameObject target);
}
using System;
using UnityEngine;

[Serializable]
public class UnlockableAction
{
    public CombatActionSO action;
    [Range(0f, 1f)] public float requiredTrust;
}

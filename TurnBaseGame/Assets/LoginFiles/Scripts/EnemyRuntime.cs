using System.Collections.Generic;
using UnityEngine;

public class EnemyRuntime : MonoBehaviour
{
    public Stats stats;
    public List<EnemyRuntime> extraEnemies = new List<EnemyRuntime>();

    public void Initialize(Stats enemyStats)
    {
        stats = enemyStats;
        Debug.Log($"Initialized Enemy | Level: {stats.Level} | Health: {stats.Health}");
    }
}
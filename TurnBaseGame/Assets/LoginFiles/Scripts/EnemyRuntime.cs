using UnityEngine;

public class EnemyRuntime : MonoBehaviour
{
    public Stats stats;

    public void Initialize(Stats enemyStats)
    {
        stats = enemyStats;
        Debug.Log($"Initialized Enemy | Level: {stats.Level} | Health: {stats.Health}");
    }
}
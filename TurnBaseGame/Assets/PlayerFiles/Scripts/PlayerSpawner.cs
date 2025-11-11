using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject camPrefab;
    public GameObject enemyPrefab;

    [Header("FightScene Positions")]
    public Transform[] playerPositions;
    public Transform[] enemiesPositions;

    public GameObject[] enemies;   // Filled dynamically before scene loads
    public bool isInFightingScene;

    private void Start()
    {
        if (PlayerDataCarrier.Instance == null || PlayerDataCarrier.Instance.LoadedPlayerData == null)
        {
            Debug.LogError("No player data found to initialize.");
            return;
        }

        // Spawn the camera first
        GameObject camObj = Instantiate(camPrefab);

        GameObject playerObj;

        // Now spawn the player
        if (isInFightingScene)
        {
            playerObj = Instantiate(playerPrefab, playerPositions[0]);
        }
        else
        {
            playerObj = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }

        FindAnyObjectByType<CinemachineCamera>().Target.TrackingTarget = playerObj.transform;

        // Initialize the player with its data
        PlayerRuntime runtime = playerObj.GetComponent<PlayerRuntime>();
        if (runtime != null)
        {
            runtime.Initialize(PlayerDataCarrier.Instance.LoadedPlayerData);
        }

        if (isInFightingScene && EnemyDataCarrier.Instance != null && EnemyDataCarrier.Instance.LoadedEnemyStatsList != null)
        {
            // Copy the initial list
            List<Stats> enemiesToSpawn = new List<Stats>(EnemyDataCarrier.Instance.LoadedEnemyStatsList);

            for (int i = 0; i < enemiesToSpawn.Count; i++)
            {
                if (i >= enemiesPositions.Length)
                {
                    Debug.LogWarning("Not enough enemy positions defined!");
                    break;
                }

                // Spawn the main enemy
                GameObject enemyObj = Instantiate(enemyPrefab, enemiesPositions[i]);
                EnemyRuntime enemyRuntime = enemyObj.GetComponent<EnemyRuntime>();

                if (enemyRuntime != null)
                {
                    enemyRuntime.Initialize(enemiesToSpawn[i]);

                    // Collect extra enemies to spawn later
                    foreach (var enemy in enemyRuntime.extraEnemies)
                    {
                        enemiesToSpawn.Add(enemy.stats);
                    }
                }
                else
                {
                    Debug.LogWarning("No EnemyRuntime found on spawned enemy prefab.");
                }
            }
        }

    }
}

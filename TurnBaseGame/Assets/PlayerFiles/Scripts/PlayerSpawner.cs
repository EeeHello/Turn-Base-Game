using NUnit.Framework;
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
    private bool isInFightingScene;
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

        //Now spawn the player
        if (isInFightingScene)
        {
            playerObj = Instantiate(playerPrefab, playerPositions[0]);
        }
        else playerObj = Instantiate(playerPrefab, transform.position, Quaternion.identity);


        FindAnyObjectByType<CinemachineCamera>().Target.TrackingTarget = playerObj.transform;

        // Initialize the player with its data
        PlayerRuntime runtime = playerObj.GetComponent<PlayerRuntime>();
        if (runtime != null)
        {
            runtime.Initialize(PlayerDataCarrier.Instance.LoadedPlayerData);
        }

        if (isInFightingScene && EnemyDataCarrier.Instance != null && EnemyDataCarrier.Instance.LoadedEnemyStatsList != null)
        {
            for (int i = 0; i < EnemyDataCarrier.Instance.LoadedEnemyStatsList.Count; i++)
            {
                GameObject enemyObj = Instantiate(enemyPrefab, enemiesPositions[i]);
                EnemyRuntime enemyRuntime = enemyObj.GetComponent<EnemyRuntime>();

                if (enemyRuntime != null)
                {
                    enemyRuntime.Initialize(EnemyDataCarrier.Instance.LoadedEnemyStatsList[i]);
                }
                else
                {
                    Debug.LogWarning("No EnemyRuntime found on spawned enemy prefab.");
                }
            }
        }
    }
}

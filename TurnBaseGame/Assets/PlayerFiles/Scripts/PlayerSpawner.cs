using NUnit.Framework;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject camPrefab;
    public bool isInFightingScene;

    public Transform[] playerPositions;
    public Transform[] enemiesPositions;
    public GameObject[] enemies;

    private void Start()
    {
        if (PlayerDataCarrier.Instance == null || PlayerDataCarrier.Instance.LoadedPlayerData == null)
        {
            Debug.LogError("No player data found to initialize.");
            return;
        }

        // Spawn the camera first
        GameObject camObj = Instantiate(camPrefab);
        Camera cam = camObj.GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("Camera prefab does not contain a Camera component!");
            return;
        }

        GameObject playerObj;

        //Now spawn the player
        if (isInFightingScene)
        {
            playerObj = Instantiate(playerPrefab, playerPositions[0]);
        }
        else playerObj = Instantiate(playerPrefab, new Vector3(0, 10, 0), Quaternion.identity);

        // Assign the camera to the SideScrollerCamera script AFTER it's been instantiated
        SideScrollerCamera scCam = playerObj.GetComponent<SideScrollerCamera>();
        if (scCam != null)
        {
            scCam.cam = cam;
        }

        // Initialize the player with its data
        PlayerRuntime runtime = playerObj.GetComponent<PlayerRuntime>();
        if (runtime != null)
        {
            runtime.Initialize(PlayerDataCarrier.Instance.LoadedPlayerData);
        }

        if (isInFightingScene && EnemyDataCarrier.Instance != null && EnemyDataCarrier.Instance.LoadedEnemyStats != null)
        {
            GameObject enemyObj = Instantiate(enemies[0], enemiesPositions[0].position, Quaternion.identity);
            EnemyRuntime enemyRuntime = enemyObj.GetComponent<EnemyRuntime>();

            if (enemyRuntime != null)
            {
                enemyRuntime.Initialize(EnemyDataCarrier.Instance.LoadedEnemyStats);
            }
            else
            {
                Debug.LogWarning("No EnemyRuntime found on spawned enemy prefab.");
            }
        }
    }
}

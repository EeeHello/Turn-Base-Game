using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject camPrefab;
    public bool isInFightingScene;

    public Transform[] playerPositions;
    public Transform[] enemiesPositions;
    public GameObject enemyPrefab; // Assign this in the Inspector
    public GameObject[] enemies;   // Filled dynamically before scene loads

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
        else playerObj = Instantiate(playerPrefab, transform.position, Quaternion.identity);

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

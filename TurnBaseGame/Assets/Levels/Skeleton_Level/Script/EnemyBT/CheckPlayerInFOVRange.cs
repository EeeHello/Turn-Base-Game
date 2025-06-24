using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPlayerInFOVRange : Node
{
    private Transform transform;
    private Animator animator;

    public CheckPlayerInFOVRange(Transform transform)
    {
        this.transform = transform;
        animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Player with tag 'Player' not found!");
            state = NodeState.FAILURE;
            Debug.Log($"[BT] InvestigatePosition: {state}");
            return state;
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);

        //Debug.Log(distance);

        if (distance <= ZombieBT.fovRange)
        {
            parent.parent.SetData("lastKnownPosition", player.transform.position);
            animator.SetBool("Walking", true);
            state = NodeState.SUCCESS;
            HandleCollisionWithPlayer(distance, player);
            Debug.Log($"[BT] InvestigatePosition: {state}");
            return state;
        }


        state = NodeState.FAILURE;
        Debug.Log($"[BT] InvestigatePosition: {state}");
        ((ZombieBT)transform.GetComponent<ZombieBT>()).currentState = "Chasing";
        return state;
    }

    private void HandleCollisionWithPlayer(float distance, GameObject player)
    {
        if (distance <= 0.5f)
        {
            if (PlayerDataCarrier.Instance == null)
            {
                Debug.LogError("PlayerDataCarrier not found!");
                return;
            }

            if (EnemyDataCarrier.Instance == null)
            {
                Debug.LogError("EnemyDataCarrier not found!");
                return;
            }

            GameObject spawnerObj = GameObject.FindObjectOfType<PlayerSpawner>()?.gameObject;
            if (spawnerObj == null)
            {
                Debug.LogError("PlayerSpawner not found in the scene!");
                return;
            }

            PlayerSpawner spawner = spawnerObj.GetComponent<PlayerSpawner>();
            if (spawner == null || spawner.enemyPrefab == null)
            {
                Debug.LogError("PlayerSpawner missing or enemyPrefab not assigned.");
                return;
            }

            float searchRadius = 5f;
            Collider[] nearbyColliders = Physics.OverlapSphere(player.transform.position, searchRadius);

            List<Stats> selectedEnemyStats = new List<Stats>();
            List<GameObject> selectedEnemyPrefabs = new List<GameObject>();

            EnemyRuntime thisEnemy = transform.GetComponent<EnemyRuntime>();
            if (thisEnemy != null)
            {
                selectedEnemyStats.Add(thisEnemy.stats);

                GameObject enemyClone = GameObject.Instantiate(spawner.enemyPrefab);
                enemyClone.GetComponent<EnemyRuntime>()?.Initialize(thisEnemy.stats);
                selectedEnemyPrefabs.Add(enemyClone);
            }
            else
            {
                Debug.LogError("EnemyRuntime missing on the colliding enemy.");
                return;
            }

            foreach (var col in nearbyColliders)
            {
                if (selectedEnemyStats.Count >= 3) break;

                if (col.gameObject != transform.gameObject)
                {
                    EnemyRuntime nearbyEnemy = col.GetComponent<EnemyRuntime>();
                    if (nearbyEnemy != null)
                    {
                        selectedEnemyStats.Add(nearbyEnemy.stats);

                        GameObject enemyClone = GameObject.Instantiate(spawner.enemyPrefab);
                        enemyClone.GetComponent<EnemyRuntime>()?.Initialize(nearbyEnemy.stats);
                        selectedEnemyPrefabs.Add(enemyClone);
                    }
                }
            }

            EnemyDataCarrier.Instance.LoadedEnemyStatsList = selectedEnemyStats;
            spawner.enemies = selectedEnemyPrefabs.ToArray();

            SceneManager.LoadScene("FightingTestScene");
        }
    }
}

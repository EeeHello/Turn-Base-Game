using BehaviorTree;
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
            return state;
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);

        //Debug.Log(distance);

        if (distance <= ZombieBT.fovRange)
        {
            parent.parent.SetData("target", player.transform);
            animator.SetBool("Walking", true);
            state = NodeState.SUCCESS;
            HandleCollisionWithPlayer(distance, player);
            return state;
        }


        state = NodeState.FAILURE;
        return state;
    }

    private void HandleCollisionWithPlayer(float distance, GameObject player)
    {
        if (distance <= 0.5f)
        {
            if (PlayerDataCarrier.Instance == null)
            {
                Debug.LogError("PlayerDataCarrier not found in scene!");
                return;
            }

            //PlayerDataCarrier.Instance.LoadedPlayerData = player;

            // Load the gameplay scene
            SceneManager.LoadScene("FightingTestScene");
        }
    }
}

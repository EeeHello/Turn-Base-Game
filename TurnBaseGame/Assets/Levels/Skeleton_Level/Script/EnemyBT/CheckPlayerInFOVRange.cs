using BehaviorTree;
using UnityEngine;

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
        Debug.Log($"Player detected at distance: {distance}");

        if (distance <= ZombieBT.fovRange)
        {
            parent.parent.SetData("target", player.transform);
            animator.SetBool("Walking", true);
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}

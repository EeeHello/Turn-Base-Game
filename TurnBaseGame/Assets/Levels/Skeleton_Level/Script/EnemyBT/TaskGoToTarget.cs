using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
public class TaskGoToTarget : Node
{
    private Transform transform;

    public TaskGoToTarget(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (target == null)
        {
            Debug.LogWarning("Target is null in TaskGoToTarget — did CheckPlayerInFOVRange run first?");
            state = NodeState.FAILURE;
            return state;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        // Check for obstacle and adjust direction if needed
        if (ObstacleDetector.IsObstacleForward(transform))
        {
            //direction = ObstacleDetector.GetAvoidanceDirection(transform);
            direction = ObstacleDetector.GetAvoidanceDirection(transform);
            Debug.Log($"[TaskGoToTarget] Avoiding obstacle, new direction: {direction}");
        }

        // Movement
        transform.position += direction * ZombieBT.speed * Time.deltaTime;

        // Rotation
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 360f * Time.deltaTime);
        }

        state = NodeState.RUNNING;
        return state;
    }
}

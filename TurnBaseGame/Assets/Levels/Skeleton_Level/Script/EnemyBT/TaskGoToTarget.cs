using System.Collections;
using UnityEngine;
using BehaviorTree;

public class TaskGoToTarget : Node
{
    private ZombieBT zombie;
    private readonly Transform transform;
    private readonly System.Func<Transform> getTarget;

    public TaskGoToTarget(Transform transform, System.Func<Transform> getTarget)
    {
        this.transform = transform;
        this.getTarget = getTarget;
    }

    public override NodeState Evaluate()
    {
        Transform target = getTarget?.Invoke();
        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        if (ObstacleDetector.IsObstacleForward(transform))
        {
            direction = ObstacleDetector.GetAvoidanceDirection(transform);
        }

        ZombieBT zombie = transform.GetComponent<ZombieBT>();
        transform.position += direction * zombie.Speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 360f * Time.deltaTime);
        }

        state = NodeState.RUNNING;
        return state;
    }
}

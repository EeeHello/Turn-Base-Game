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

        if (Vector3.Distance(transform.position, target.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, ZombieBT.speed * Time.deltaTime);
            //transform. LookAt(target.position);

            Vector3 lookDirection = target.position - transform.position;
            lookDirection.y = 0f; ;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}

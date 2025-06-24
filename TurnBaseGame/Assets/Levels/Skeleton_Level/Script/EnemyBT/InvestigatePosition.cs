using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class InvestigatePosition : Node
{
    private Transform transform;

    private float investigationTime = 10f;
    private float timer = 0f;
    private bool arrived = false;

    public InvestigatePosition(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        object lastPosObj = GetData("lastKnownPosition");
        if (lastPosObj == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (!(GetData("lastKnownPosition") is Vector3 lastKnownPosition))
        {
            state = NodeState.FAILURE;
            Debug.Log($"[BT] InvestigatePosition: {state}");
            return state;
        }

        if (state != NodeState.RUNNING)
        {
            timer = 0f;
            arrived = false;
        }

        if(!arrived)
        {
            if (Vector3.Distance(transform.position, lastKnownPosition) > 1.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, lastKnownPosition, ZombieBT.speed * Time.deltaTime);

                Vector3 direction = lastKnownPosition - transform.position;
                direction.y = 0f;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
                else
                {
                    arrived = true;
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= investigationTime)
                {
                    timer += 0f;
                    arrived = false;
                    ClearData("target");
                    ClearData("lastKnownPosition");

                    state = NodeState.SUCCESS;
                    Debug.Log($"[BT] InvestigatePosition: {state}");
                    Debug.Log($"[BT] InvestigatePosition: {state}");
                    ((ZombieBT)transform.GetComponent<ZombieBT>()).currentState = "Investigating";
                    return state;
                }
            }
        }

        state = NodeState.RUNNING;
        Debug.Log($"[BT] InvestigatePosition: {state}");
        return state;
    }
}

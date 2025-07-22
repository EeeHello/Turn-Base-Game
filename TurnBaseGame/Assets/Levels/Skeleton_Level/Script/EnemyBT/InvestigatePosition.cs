using System.Collections;
using UnityEngine;
using BehaviorTree;

public class InvestigatePosition : Node
{
    private readonly Transform transform;
    private readonly System.Func<Vector3?> getLastKnownPosition;
    private readonly System.Action onInvestigationComplete;

    private float timer = 0f;
    private bool arrived = false;
    private const float investigationTime = 4f;

    public InvestigatePosition(Transform transform, System.Func<Vector3?> getLastKnownPosition, System.Action onInvestigationComplete)
    {
        this.transform = transform;
        this.getLastKnownPosition = getLastKnownPosition;
        this.onInvestigationComplete = onInvestigationComplete;
    }

    public override NodeState Evaluate()
    {
        Vector3? lastKnown = getLastKnownPosition?.Invoke();
        if (lastKnown == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        Vector3 targetPos = lastKnown.Value;

        if (!arrived)
        {
            if (Vector3.Distance(transform.position, targetPos) > 0.4f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, ZombieBT.speed * Time.deltaTime);

                Vector3 direction = targetPos - transform.position;
                direction.y = 0f;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
            else
            {
                arrived = true;
                timer = 0f;
            }
        }

        if (arrived)
        {
            timer += Time.deltaTime;
            if (timer >= investigationTime)
            {
                onInvestigationComplete?.Invoke();
                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}

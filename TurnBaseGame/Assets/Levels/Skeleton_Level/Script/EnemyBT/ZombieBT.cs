using BehaviorTree;
using System.Collections.Generic;

public class ZombieBT : Tree
{
    public UnityEngine.Transform[] waypoints;

    public static float speed = 2f;
    public static float fovRange = 6f;
    public string currentState;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInFOVRange(transform),
                new TaskGoToTarget(transform),
            }),
            new InvestigatePosition(transform),
            new PatrolAI(transform, waypoints)
        });
        return root;
    }
}

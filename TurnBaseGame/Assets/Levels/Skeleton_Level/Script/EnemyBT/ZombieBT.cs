using BehaviorTree;
using System.Collections.Generic;

public class ZombieBT : Tree
{
    public UnityEngine.Transform[] waypoints;

    public static float speed = 5f;
    public static float fovRange = 6f;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInFOVRange(transform),
                new TaskGoToTarget(transform),
            }),
            new PatrolAI(transform, waypoints)
        });
        return root;
    }
}

using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBT : BehaviorTree.Tree
{
    public Transform[] waypoints;

    public static float speed = 2f;
    public static float fovRange = 6f;
    public string currentState;

    private Transform playerTransform;
    private Vector3? lastKnownPosition;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInFOVRange(transform, (pos) =>
                {
                    lastKnownPosition = pos;
                    playerTransform = GameObject.FindWithTag("Player")?.transform;
                }),
                new TaskGoToTarget(transform, () => playerTransform),
            }),

            new Sequence(new List<Node>
            {
                new RandomChanceNode(0.4f),
                new InvestigatePosition(transform,
                    () => lastKnownPosition,
                    () =>
                    {
                        lastKnownPosition = null;
                        playerTransform = null;
                        currentState = "Investigating";
                    }),
            }),
            new PatrolAI(transform, waypoints)
        });

        return root;
    }
}

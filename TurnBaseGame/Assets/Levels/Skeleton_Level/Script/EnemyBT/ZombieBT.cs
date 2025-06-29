using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBT : BehaviorTree.Tree
{
    public UnityEngine.Transform[] waypoints;

    public static float speed = 2f;
    public static float fovRange = 6f;
    public string currentState;

    public float investigationChance;
    public float agressionLevel = 0.75f;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInFOVRange(transform),
                new TaskGoToTarget(transform),
            }),

            new Sequence(new List<Node>
            {
                new RandomChanceNode(investigationChance),
                new InvestigatePosition(transform),
            }),

            new PatrolAI(transform, waypoints)
        });
        return root;
    }
}

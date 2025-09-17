using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBT : BehaviorTree.Tree
{
    [SerializeField] private Transform[] waypoints; 
    [SerializeField] private string currentState;
    [SerializeField] private float speed = 2f;

    private Transform playerTransform;
    private Vector3? lastKnownPosition;

    public static float fovRange = 6f;

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
                    currentState = "Player in range";
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

    public float Speed
    {
        get => speed;
        set => speed = Mathf.Max(0, value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class ChanceSelectorNode : Node
{
    private RandomChanceNode chanceNode;
    private Node chaseBehaviorNode;

    public ChanceSelectorNode(RandomChanceNode chanceNode, Node chaseBehaviorNode)
    {
        this.chanceNode = chanceNode;
        this.chaseBehaviorNode = chaseBehaviorNode;

        chanceNode.parent = this;
        chaseBehaviorNode.parent = this;
    }

    public override NodeState Evaluate()
    {
        var chanceResult = chanceNode.Evaluate();
        Debug.Log($"[ChanceSelectorNode] Chance result: {chanceResult}");

        if (chanceResult == NodeState.SUCCESS)
        {
            var result = chaseBehaviorNode.Evaluate();
            Debug.Log($"[ChanceSelectorNode] Behavior node returned: {result}");

            // Resetter when behavior finishes
            if (result == NodeState.SUCCESS  || result == NodeState.FAILURE)
            {
                chanceNode.Reset();
            }

            state = result;
            return state;
        }

        chanceNode.Reset();
        state = NodeState.FAILURE;
        return state;
    }
}

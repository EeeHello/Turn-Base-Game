using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomChanceNode : Node
{
    private float chance;
    private bool evaluated = false;
    private bool result;

    public RandomChanceNode(float chance)
    {
        this.chance = Mathf.Clamp01(chance);
    }

    public override NodeState Evaluate()
    {
        if (!evaluated)
        {
            result = Random.value < chance;
            evaluated = true;
            Debug.Log($"[RandomChanceNode] Evaluated: {(result ? "SUCCESS" : "FAILURE")}");
        }

        state = result ? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }

    public void Reset()
    {
        evaluated = false;
        Debug.Log("[RandomChanceNode] Reset for next round.");
    }

}

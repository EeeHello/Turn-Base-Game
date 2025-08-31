using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TagAIController : MonoBehaviour
{
    public enum States {
        MoveTowards,
        MoveAway, 
        StandStill
    }

    public States currentState;
    public Transform opponent;
    public bool isChaser = true;

    private ANN net;
    private Rigidbody rb;
    public float moveSpeed = 5f;

    void Start()
    {

        net = new ANN(3, 4, 3);
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (opponent == null) return;

        // Inputs 
        Vector3 toOpponent = opponent.position - transform.position;
        toOpponent.y = 0f;

        float distance = Mathf.Clamp01(toOpponent.magnitude / 20f); 
        float angle = Vector3.Dot(transform.forward.normalized, toOpponent.normalized); 
        float role = isChaser ? 1f : 0f;

        List<float> inputs = new List<float> { distance, angle, role };

        // ANN Decision 
        List<float> outputs = net.Forward(inputs);
        int decision = GetBestAction(outputs);
        //int decision = outputs.IndexOf(Mathf.Max(outputs.ToArray()));

        Debug.Log($"Output count: {outputs.Count}");
        Debug.Log($"Outputs: {string.Join(", ", outputs.Select(p => p.ToString("F2")))} Decision: {decision}");

        // --- Rule-based fallback layer ---
        if (isChaser)
        {
            // Chaser should always move towards if close
            if (distance < 0.4f) decision = (int)States.MoveTowards;
        }
        else 
        {
            // Runner should not stand still if chaser is near
            if (distance < 0.4f && decision == (int)States.StandStill)
                decision = (int)States.MoveAway;
        }

        currentState = (States)decision;

        Debug.Log($"Outputs: {string.Join(", ", outputs.Select(p => p.ToString("F2")))} Final Decision: {currentState}");

        Vector3 move = Vector3.zero;
        switch (currentState)
        {
            case States.MoveTowards:
                move = toOpponent.normalized;
                Debug.Log("MoveTowards");
                break;
            case States.MoveAway:
                move = -toOpponent.normalized;
                Debug.Log("MoveAway");
                break;
            case States.StandStill:
                move = Vector3.zero;
                Debug.Log("StandStill");
                break;
        }

        rb.linearVelocity = move * moveSpeed;
    }

    private int GetBestAction(List<float> outputs)
    {
        int bestIndex = 0;
        float bestValue = outputs[0];
        for (int i = 1; i < outputs.Count; i++)
        {
            if (outputs[i] > bestValue)
            {
                bestValue = outputs[i];
                bestIndex = i;
            }
        }
        return bestIndex;
    }
}

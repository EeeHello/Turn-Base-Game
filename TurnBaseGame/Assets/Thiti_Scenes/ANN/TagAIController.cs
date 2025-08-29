using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TagAIController : MonoBehaviour
{
    private enum States {
        MoveTowards,
        MoveAway, 
        StandStill
    }
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
        float distance = Mathf.Clamp01(toOpponent.magnitude / 20f); 
        float angle = Vector3.Dot(transform.forward.normalized, toOpponent.normalized); 
        float role = isChaser ? 1f : 0f;

        List<float> inputs = new List<float> { distance, angle, role };

        // ANN Decision 
        List<float> outputs = net.Forward(inputs);
        int decision = outputs.IndexOf(Mathf.Max(outputs.ToArray()));

        Vector3 move = Vector3.zero;
        switch (decision)
        {
            case (int)States.MoveTowards:
                move = toOpponent.normalized;
                break;
            case (int)States.MoveAway: 
                move = -toOpponent.normalized;
                break;
            case (int)States.StandStill:
                move = Vector3.zero;
                break;
        }

        rb.linearVelocity = move * moveSpeed;
    }
}

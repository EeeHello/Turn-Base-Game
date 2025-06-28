using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class PatrolAI : Node
{
    private Transform transform;
    private Transform[] waypoints;

    private int currentWaypointIndex = 0;

    private float waitTime = 0f;
    private float waitCounter = 0f;
    private bool waiting = false;

    private bool isIdling = false;
    private string currentIdle = " ";

    private bool pauseMidPatrol = false;
    private float pauseTimer = 0f;
    private float pauseDuration = 0f;

    private bool isRotating = false;
    private float rotationDirection = 1f;
    private float rotationSpeed = 45f;
    public PatrolAI(Transform transform, Transform[] waypoints)
    {
        this.transform = transform;
        this.waypoints = waypoints;
    }

    public override NodeState Evaluate()
    {
        if (waiting)
        {
            HandleIdleRotation();

            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                ResetIdleState();
                waiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                Debug.Log($"[PatrolAI] {transform.name} finished idling and is resuming patrol.");
            }

            state = NodeState.RUNNING;
            return state;
        }

        if (pauseMidPatrol)
        {
            HandleIdleRotation();

            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                ResetIdleState();
                pauseMidPatrol = false;
                Debug.Log($"[PatrolAI] {transform.name} resumed walking toward waypoint.");
            }

            state = NodeState.RUNNING;
            return state;
        }

        Transform wp = waypoints[currentWaypointIndex];
        float distanceToWP = Vector3.Distance(transform.position, wp.position);

        // Trigger mid-walk pause randomly (20% chance/sec)
        if (!pauseMidPatrol && !waiting && !isIdling && Random.value < 0.05f * Time.deltaTime)
        {
            pauseMidPatrol = true;
            pauseDuration = Random.Range(1.5f, 3f);
            pauseTimer = 0f;
            PickRandomIdle();
            Debug.Log($"[PatrolAI] {transform.name} randomly paused mid-patrol.");
            state = NodeState.RUNNING;
            return state;
        }

        if (distanceToWP < 0.01f)
        {
            transform.position = wp.position;
            waitCounter = 0f;
            waitTime = Random.Range(2f, 4f);
            PickRandomIdle();
            waiting = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, wp.position, ZombieBT.speed * Time.deltaTime);
            Vector3 lookDirection = wp.position - transform.position;
            lookDirection.y = 0f;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        state = NodeState.RUNNING;
        return state;
    }

    private void PickRandomIdle()
    {
        isIdling = true;
        int idleIndex = Random.Range(0, 3);

        switch (idleIndex)
        {
            case 0:
                currentIdle = "Looking around...";
                isRotating = true;
                rotationDirection = Random.value < 0.5f ? 1f : -1f;
                break;
            case 1:
                currentIdle = "Scratching head...";
                isRotating = false;
                break;
            case 2:
                currentIdle = "Stretching...";
                isRotating = false;
                break;
        }

        Debug.Log($"[PatrolAI] {transform.name} is idling: {currentIdle}" + (isRotating ? $" (Rotating {(rotationDirection > 0 ? "right" : "left")})" : ""));
    }

    private void HandleIdleRotation()
    {
        if (isRotating)
        {
            transform.Rotate(Vector3.up, rotationSpeed * rotationDirection * Time.deltaTime);
        }
    }

    private void ResetIdleState()
    {
        isIdling = false;
        isRotating = false;
        currentIdle = "";
    }
}


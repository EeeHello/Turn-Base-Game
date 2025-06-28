using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class PatrolAI : Node
{
    private Transform transform;
    private Transform[] waypoints;

    private int currentWaypointIndex = 0;

    private float waitTime = Random.Range(2f, 4f);
    private float waitCounter = 0f;
    private bool waiting = false;

    private float idleTimer = 0f;
    private float idleDurantion = 2f;
    private bool isIdling = false;
    private string currentIdle = " ";

    public PatrolAI(Transform transform, Transform[] waypoints)
    {
        this.transform = transform;
        this.waypoints = waypoints;
    }
    public override NodeState Evaluate()
    {
        if (waiting)
        {
            if (!isIdling)
            {
                int idleIndex = Random.Range(0, 3);
                switch (idleIndex)
                {
                    case 0: currentIdle = "Looking around...";
                        break;

                    case 1: currentIdle = "Scratching head...";
                        break;

                    case 2: currentIdle = "Stretching...";
                        break;
                }
                Debug.Log($"[PatrolAI] {transform.name} is idling: {currentIdle}");
                isIdling = true;
            }

            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
                waiting = false;
            }
        }
        else
        {
            Transform wp = waypoints[currentWaypointIndex];
            if (Vector3.Distance(transform.position, wp.position) < 0.01f)
            {
                transform.position = wp.position;
                waitCounter = 0f;
                waiting = true;

                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, wp.position, ZombieBT.speed * Time.deltaTime);
                //transform. LookAt(wp.position);

                Vector3 lookDirection = wp.position - transform.position;
                lookDirection.y = 0f; ;
                if (lookDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection);
                }
            }
        }

        state = NodeState.RUNNING;
        Debug.Log($"[BT] InvestigatePosition: {state}");
        return state;
    }
}


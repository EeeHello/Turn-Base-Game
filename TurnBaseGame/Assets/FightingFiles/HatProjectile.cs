using UnityEngine;
using System.Collections;

public class HatProjectile : MonoBehaviour
{
    public enum HatState { BasicAction, SkillAction };

    private Vector3 currentTarget;
    private float currentSpeed = 0f;
    private float hitDistance = 0.2f;

    private HatState currentState;

    public float inHandTime = 0.3f; // How long it travels at 5 speed
    private float timeSinceLaunch = 0f;

    private bool isActive = false;

    void Update()
    {
        if (isActive && currentTarget != null && currentState==HatState.BasicAction)
        {
            timeSinceLaunch += Time.deltaTime;

            currentSpeed = (timeSinceLaunch < inHandTime) ? 5f : 15f;

            transform.position = Vector3.MoveTowards(transform.position, currentTarget, currentSpeed * Time.deltaTime);

            float distance = Vector3.Distance(transform.position, currentTarget);
            if (distance < hitDistance)
            {
                isActive = false;

                
                Destroy(gameObject);
            }
        }
    }
    public void BasicAction(Vector3 target)
    {
        currentState = HatState.BasicAction;
        currentTarget = target;
        currentSpeed = 5f;
        timeSinceLaunch = 0f;
        isActive = true;
    }

    public void SkillAction(Vector3 target)
    {
        currentState = HatState.SkillAction;
        currentTarget = target;
        // Implement SkillAction behavior here
    }
}

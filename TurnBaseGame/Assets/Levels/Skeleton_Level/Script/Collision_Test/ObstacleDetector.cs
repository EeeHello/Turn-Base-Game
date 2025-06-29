using UnityEngine;

public static class ObstacleDetector
{
    public static bool IsObstacleInDirection(Transform origin, Vector3 direction, float checkDistance, string tag = "Building")
    {
        RaycastHit hit;
        Vector3 rayOrigin = origin.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, direction.normalized, out hit, checkDistance))
        {
            Debug.DrawRay(rayOrigin, direction.normalized * checkDistance, Color.red);
            return hit.collider.CompareTag(tag);
        }

        Debug.DrawRay(rayOrigin, direction * checkDistance, Color.green);
        return false;
    }

    public static bool IsObstacleForward(Transform origin, float checkDistance = 1.0f) => IsObstacleInDirection(origin, origin.forward, checkDistance);
    public static bool IsObstacleLeft(Transform origin, float checkDistance = 1.0f) => IsObstacleInDirection(origin, -origin.right, checkDistance);
    public static bool IsObstacleRight(Transform origin, float checkDistance = 1.0f) => IsObstacleInDirection(origin, origin.right, checkDistance);

     public static Vector3 GetAvoidanceDirection(Transform origin, float checkDistance = 1.0f)
    {
        bool forwardBlocked = IsObstacleForward(origin, checkDistance);
        bool leftBlocked = IsObstacleLeft(origin, checkDistance);
        bool rightBlocked = IsObstacleRight(origin, checkDistance);

        if (!leftBlocked && rightBlocked) return -origin.right;
        if (!rightBlocked && leftBlocked) return origin.right;
        if (!forwardBlocked && !leftBlocked && !rightBlocked) return origin.forward;
        if (!forwardBlocked) return origin.forward;

        // All directions blocked, step back
        return -origin.forward;
    }
}

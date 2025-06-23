using Unity.VisualScripting;
using UnityEngine;

public class SideScrollerCamera : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    public Vector3 offset = new Vector3(-5f, 3f, 0f);
    public float smoothSpeed = 5f;
    public float collisionCheckRadius = 0.3f;
    public float maxCameraPullBack = 5f;
    public float pullBackStep = 0.1f;
    public Vector3 minBounds;
    public Vector3 maxBounds;

    private Transform camTransform;

    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Camera not assigned in SideScrollerCamera script.");
            enabled = false;
            return;
        }

        camTransform = cam.transform;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Start with default offset position
        Vector3 basePosition = target.position + offset;

        // Lock Z and Y immediately to target
        float fixedY = basePosition.y;
        float fixedZ = basePosition.z;

        // We'll only adjust X if there's obstruction
        Vector3 adjustedPosition = new Vector3(basePosition.x, fixedY, fixedZ);
        Vector3 directionToCamera = offset.normalized;

        int maxAttempts = Mathf.CeilToInt(maxCameraPullBack / pullBackStep);
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            bool isBlocked = false;
            Collider[] hits = Physics.OverlapSphere(adjustedPosition, collisionCheckRadius);

            foreach (var hit in hits)
            {
                if (hit.transform != target && hit.transform != camTransform)
                {
                    isBlocked = true;
                    break;
                }
            }

            bool viewObstructed = IsViewObstructed(adjustedPosition, target.position);

            if (!isBlocked && !viewObstructed)
                break;

            adjustedPosition.x += -directionToCamera.x * pullBackStep; // Only adjust X
            attempts++;
        }

        // Clamp position within bounds
        adjustedPosition.x = Mathf.Clamp(adjustedPosition.x, minBounds.x, maxBounds.x);

        // Force Y and Z to stay locked to target
        adjustedPosition.y = fixedY;
        adjustedPosition.z = fixedZ;

        // Smooth X movement only
        Vector3 currentPos = camTransform.position;
        float smoothedX = Mathf.Lerp(currentPos.x, adjustedPosition.x, smoothSpeed * Time.deltaTime);

        camTransform.position = new Vector3(smoothedX, adjustedPosition.y, adjustedPosition.z);
    }

    private bool IsViewObstructed(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        float distance = dir.magnitude;

        if (Physics.Raycast(from, dir.normalized, out RaycastHit hit, distance))
        {
            if (hit.transform != target && hit.transform != camTransform)
            {
                Collider col = hit.collider;

                // Sample check: use bounds to estimate "thickness" along ray direction
                float thickness = Vector3.Dot(col.bounds.size, dir.normalized);

                if (thickness < 0.3f) // Thin object threshold
                    return false;

                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(target.position + offset, collisionCheckRadius);
    }
}

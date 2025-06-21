using UnityEngine;

public class SideScrollerCamera : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed;
    public float collisionCheckRadius;
    private Transform camTransform;
    public Vector3 minBounds;
    public Vector3 maxBounds;

    private void Start()
    {
        camTransform = cam.transform;
    }
    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 correctedPosition = desiredPosition;

        // Check if desiredPosition collides with anything
        Collider[] hits = Physics.OverlapSphere(desiredPosition, collisionCheckRadius);

        bool isColliding = false;

        foreach (Collider hit in hits)
        {
            if (hit.transform != target && hit.transform != camTransform)
            {
                isColliding = true;
                break;
            }
        }

        // If colliding, move camera away from player until it's clear
        if (isColliding)
        {
            float step = -0.05f;
            int maxAttempts = 50;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                correctedPosition += Vector3.right * step;

                bool stillColliding = false;
                hits = Physics.OverlapSphere(correctedPosition, collisionCheckRadius);
                foreach (Collider hit in hits)
                {
                    if (hit.transform != target && hit.transform != camTransform)
                    {
                        stillColliding = true;
                        break;
                    }
                }

                if (!stillColliding && !IsViewObstructed(camTransform.position,target.position))
                    break;

                attempts++;
            }
        }

        // Smoothly follow correctedPosition 
        correctedPosition.x = Mathf.Clamp(correctedPosition.x, minBounds.x, maxBounds.x); //map bounds
        correctedPosition.z = Mathf.Clamp(correctedPosition.z, minBounds.z, maxBounds.z); //map bounds
        Vector3 smoothedPosition = Vector3.Lerp(camTransform.position, correctedPosition, smoothSpeed * Time.deltaTime);
        smoothedPosition.z = correctedPosition.z;
        camTransform.position = smoothedPosition;
    }
    private bool IsViewObstructed(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        Ray ray = new Ray(from, dir);
        float distance = dir.magnitude;

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            if (hit.transform != target && hit.transform != camTransform)
                return true;
        }

        return false;
    }
}

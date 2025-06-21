using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public bool isDeepCollision = false;

    private void OnTriggerStay(Collider other)
    {
        // Ensure the other has a collider and isn't a trigger
        if (!other.isTrigger)
        {
            Collider thisCollider = GetComponent<Collider>();
            Vector3 direction;
            float distance;

            // Try to compute penetration depth
            if (Physics.ComputePenetration(
                thisCollider, transform.position, transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out direction, out distance))
            {
                if (distance >= GetComponent<SphereCollider>().radius * 0.5f)
                {
                    isDeepCollision = true;
                }
                else
                {
                    isDeepCollision = false;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isDeepCollision = false;
    }
}

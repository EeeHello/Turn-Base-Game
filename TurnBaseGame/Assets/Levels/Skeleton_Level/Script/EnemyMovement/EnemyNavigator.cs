//using UnityEngine;

//public class EnemyNavigator : MonoBehaviour
//{
//    public float moveSpeed = 2f;
//    public float obstacleAvoidDistance = 1.5f;
//    public LayerMask obstacleMask;
//    public float rotationSpeed = 360f;

//    private Vector3 currentDestination;
//    private bool hasDestination = false;

//    private void Update()
//    {
//        if (!hasDestination)
//            return;

//        Vector3 direction = (currentDestination - transform.position).normalized;
//        direction.y = 0f;

//        // Apply obstacle avoidance
//        direction = ObstacleAvoidance(direction);

//        // Move manually
//        transform.position += direction * moveSpeed * Time.deltaTime;

//        // Rotate toward direction
//        if (direction != Vector3.zero)
//        {
//            Quaternion lookRot = Quaternion.LookRotation(direction);
//            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
//        }

//        // Reached destination
//        if (Vector3.Distance(transform.position, currentDestination) <= 0.2f)
//        {
//            Stop();
//        }
//    }

//    public void SetDestination(Vector3 destination)
//    {
//        currentDestination = destination;
//        hasDestination = true;
//    }

//    public void Stop()
//    {
//        hasDestination = false;
//    }

//    public bool AtDestination()
//    {
//        return !hasDestination;
//    }

//    private Vector3 ObstacleAvoidance(Vector3 originalDir)
//    {
//        Vector3 origin = transform.position + Vector3.up * 0.5f;
//        Vector3 avoidDir = originalDir;

//        if (Physics.Raycast(origin, transform.forward, obstacleAvoidDistance, obstacleMask))
//        {
//            Debug.DrawRay(origin, transform.forward * obstacleAvoidDistance, Color.red);

//            Vector3 right = transform.right;
//            Vector3 left = -transform.right;

//            bool rightClear = !Physics.Raycast(origin, right, obstacleAvoidDistance, obstacleMask);
//            bool leftClear = !Physics.Raycast(origin, left, obstacleAvoidDistance, obstacleMask);

//            if (rightClear && !leftClear)
//                avoidDir += right;
//            else if (leftClear && !rightClear)
//                avoidDir += left;
//            else if (rightClear && leftClear)
//                avoidDir += Random.value < 0.5f ? left : right;
//            else
//                avoidDir = -originalDir; // Turn around if boxed in
//        }

//        return avoidDir.normalized;
//    }
//}

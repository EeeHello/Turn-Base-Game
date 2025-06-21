using UnityEngine;

public class SideScrollerCamera : MonoBehaviour
{   
    public Camera Camera;
    public Transform target;
    public Vector3 offset; 
    public float smoothSpeed;

    SphereCollider cameraCollider;

    private void Start()
    {
        cameraCollider=Camera.GetComponent<SphereCollider>();
    }

    private void Update()
    {
        CameraTrigger trigger = Camera.GetComponent<CameraTrigger>();

        if (trigger != null && trigger.isDeepCollision)
        {
            Debug.Log("Deep collision detected");

        }
        else
        {
        }
    }





    void LateUpdate()
    {
        if (target == null)
            return;

        
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, target.position.z) + offset ;
        Vector3 smoothedPosition = Vector3.Lerp(Camera.transform.position, desiredPosition, smoothSpeed); //find way too smooth out (maybe with time)

        Camera.transform.position = smoothedPosition;
    }
}
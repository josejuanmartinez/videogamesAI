using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    public Transform target;  // The object to follow
    public float followSpeed = 5f;  // The speed at which the camera follows the object

    private Vector3 offset;  // The initial offset between the camera and the object

    private void Start()
    {
        // Calculate the initial offset between the camera and the object
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the target position by adding the offset to the object's position
            Vector3 targetPosition = target.position + offset;

            // Move the camera towards the target position using linear interpolation
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Make the camera always look at the object
            //transform.LookAt(target);
        }
    }
}
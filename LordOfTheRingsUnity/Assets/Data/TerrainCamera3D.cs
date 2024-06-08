using UnityEngine;

[RequireComponent (typeof(Camera))]
public class TerrainCamera3D : MonoBehaviour
{
    private Transform target; // The object to look at
    public float speed = 15f; // Speed at which the camera moves
    public float stopDistance = 0.1f; // Distance at which the camera stops moving

    private Camera cam;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        if (target != null)
        {
            // Calculate the distance to the target
            float distance = Vector3.Distance(transform.position, target.position);

            // Check if the camera has reached the target
            if (distance > stopDistance)
            {
                // Move the camera towards the target
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

                // Rotate the camera to look at the target
                transform.LookAt(target);
            }
            else
            {
                target = null;
            }
        }
    }

    public void LookAt(Transform t)
    {
        Debug.Log("Looking at " + t.name);
        target = t;
    }
}

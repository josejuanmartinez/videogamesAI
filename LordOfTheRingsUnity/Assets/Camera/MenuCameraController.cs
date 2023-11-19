using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    public Canvas regionsCanvas;
    public Canvas charactersCanvas;
    public float moveSpeed = 5.0f; // the speed of the camera movement

    private Transform target;
    private Vector3 targetPosition;

    void Start()
    {
        target = transform;
        // Set the initial position and rotation of the camera to match the target
        targetPosition = target.position;
        transform.position = targetPosition;
    }

    void Update()
    {
        // Calculate the new position and rotation of the camera
        targetPosition = target.position;

        Vector3 v3targetPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        Vector3 newPosition = Vector3.Lerp(transform.position, v3targetPosition, Time.deltaTime * moveSpeed);

        // Move the camera to the new position and rotation
        transform.position = newPosition;
    }

    public void LookTo(Transform trTargetPosition)
    {
        // Calculate the new position and rotation of the camera
        target = trTargetPosition;
    }
    public void LookToCharacters()
    {
        LookTo(charactersCanvas.transform);
    }

    public void LookToRegions()
    {
        LookTo(regionsCanvas.transform);
    }
}

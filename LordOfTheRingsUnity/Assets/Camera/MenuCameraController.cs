using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    public Canvas regionsCanvas;
    public Canvas charactersCanvas;
    public float moveSpeed = 5.0f; // the speed of the camera movement

    private Transform target;
    private Vector3 targetPosition;
    private AudioManager audioManager;
    private AudioRepo audioRepo;

    void Awake()
    {

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
    }

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

        Vector3 v3targetPosition = new (targetPosition.x, targetPosition.y, transform.position.z);
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
        audioManager.PlaySound(audioRepo.GetAudio("cards"));
        LookTo(charactersCanvas.transform);
    }

    public void LookToRegions()
    {
        audioManager.PlaySound(audioRepo.GetAudio("cards"));
        LookTo(regionsCanvas.transform);
    }
}

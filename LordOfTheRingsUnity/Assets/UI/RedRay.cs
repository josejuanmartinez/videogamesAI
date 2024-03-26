using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class RedRay : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float rayLength = 10f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + transform.forward * rayLength;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
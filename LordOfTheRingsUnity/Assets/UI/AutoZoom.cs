using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animation))]
public class AutoZoom : MonoBehaviour, IPointerEnterHandler
{
    
    Animation autozoom;

    // Start is called before the first frame update
    void Start()
    {
        autozoom = GetComponent<Animation>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!autozoom.isPlaying)
            autozoom.Play();
    }

}

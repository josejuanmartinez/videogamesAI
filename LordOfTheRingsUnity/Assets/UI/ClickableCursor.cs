using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Mouse mouse;
    
    public void Awake()
    {
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse.Clickable();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse.RemoveCursor();
    }

}

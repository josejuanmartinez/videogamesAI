using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public TextMeshProUGUI prowess;
    
    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public void Initialize(int prowess)
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        gridLayout = transform.parent.GetComponent<GridLayoutGroup>();
        this.prowess.text = prowess.ToString();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(gridLayout != null)
            gridLayout.enabled = false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (gridLayout != null)
            gridLayout.enabled = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        rectTransform.position = Input.mousePosition;
    }
}

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapSelector : MonoBehaviour
{
    public GameObject minimap;

    private TilemapSelector tilemapSelector;
    private EventSystem eventSystem;

    void Awake()
    {
        tilemapSelector = GameObject.Find("TilemapSelector").GetComponent<TilemapSelector>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tilemapSelector.IsOverUI())
        {
            if (IsOverMinimap() && Input.GetMouseButtonUp(0))
            {
                Vector2 clickPosition = GetRaycastResultByTag("Minimap")[0].screenPosition;
                Debug.Log(clickPosition);
                Vector3[] corners = new Vector3[4];
                minimap.GetComponent<RectTransform>().GetWorldCorners(corners);
                for(int i=0; i<corners.Length; i++)
                {
                    Debug.Log(corners[i]);
                }
            }
        }
    }

    public bool IsOverMinimap()
    {
        PointerEventData pointerEventData = new(eventSystem) { position = Input.mousePosition };

        List<RaycastResult> results = new();

        eventSystem.RaycastAll(pointerEventData, results);

        if (results.Count > 0)
            foreach (RaycastResult r in results)
                if (r.gameObject.CompareTag("Minimap"))
                    return true;
        return false;
    }

    public List<RaycastResult> GetRaycastResultByTag(string tag)
    {
        PointerEventData pointerEventData = new(eventSystem) { position = Input.mousePosition };

        List<RaycastResult> results = new();

        eventSystem.RaycastAll(pointerEventData, results);

        if (results.Count > 0)
            return results.FindAll(x => x.gameObject.CompareTag(tag));

        return new List<RaycastResult>();
    }
}

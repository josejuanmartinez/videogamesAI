using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MinimapSelector : MonoBehaviour
{
    public GameObject minimap;
    public Tilemap worldTilemap;
    public Vector2 decayPercentage;

    private TilemapSelector tilemapSelector;
    private EventSystem eventSystem;
    private CameraController cameraController;

    private Vector3[] corners;
    private Vector3 firstCornerWorldPosition;
    private Vector3 tilemapSize;
    private Vector2 minimapSpriteSize;

    

    private bool isAwaken = false;
    void Awake()
    {
        tilemapSelector = GameObject.Find("TilemapSelector").GetComponent<TilemapSelector>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        corners = new Vector3[4];
        minimap.GetComponent<RectTransform>().GetWorldCorners(corners);
        //for (int i = 0; i < corners.Length; i++)
        //    Debug.Log(corners[i]);
        
        BoundsInt worldTilemapBounds = worldTilemap.cellBounds;
        //Debug.Log(string.Format("Tilemap bounds: {0}", worldTilemapBounds));
        firstCornerWorldPosition = worldTilemap.CellToWorld(worldTilemapBounds.position);
        Vector3 lastCornerWorldPosition = worldTilemap.CellToWorld(worldTilemapBounds.size);
        tilemapSize = lastCornerWorldPosition - firstCornerWorldPosition;
        //Debug.Log(string.Format("World bounds : from {0} to {1}", firstCornerWorldPosition, lastCornerWorldPosition));
        //Debug.Log(string.Format("Total tilemap size: {0}", tilemapSize));
        minimapSpriteSize = minimap.GetComponent<RectTransform>().rect.size;
        //Debug.Log(string.Format("Total image sprite size: {0}", minimapSpriteSize));

        isAwaken = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (tilemapSelector.IsOverUI())
        {
            if (IsOverMinimap() && Input.GetMouseButton(0))
            {
                if (!isAwaken)
                    Awake();

                Vector2 clickPosition = GetRaycastResultByTag("Minimap")[0].screenPosition;
                Vector3 clickPosition3D = new (clickPosition.x, clickPosition.y, 0);
                //Debug.Log(clickPosition3D);

                Vector3 distanceToCorner0 = new (clickPosition3D.x - corners[0].x, clickPosition3D.y - corners[0].y, 0);

                //Debug.Log(string.Format("Distance to corner in sprite is {0}", distanceToCorner0));
                Vector2 relativeDistanceToCorner0 = new Vector2(
                    distanceToCorner0.x / minimapSpriteSize.x,
                    distanceToCorner0.y / minimapSpriteSize.y);
                //Debug.Log(string.Format("That means a percentage offset of {0}", relativeDistanceToCorner0));

                Vector2 pixelsInWorld = new (
                    relativeDistanceToCorner0.x * tilemapSize.x,
                    relativeDistanceToCorner0.y * tilemapSize.y);

                Vector2 target = new(
                    firstCornerWorldPosition.x + pixelsInWorld.x,
                    firstCornerWorldPosition.y + pixelsInWorld.y);

                target.x += distanceToCorner0.x * decayPercentage.x;
                target.y += distanceToCorner0.y * decayPercentage.y;

                cameraController.LookToImmediate(target);

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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MinimapSelector : MonoBehaviour
{
    private TilemapSelector tilemapSelector;
    private EventSystem eventSystem;
    private CameraController cameraController;

    public Vector3 tilemapSize;
    
    public Vector2 minimapSpriteSize;

    public Vector3 relativeDistanceToCorner0;

    public Vector3[] corners;
    public int minX, minY, maxX, maxY, sizeX, sizeY;

    public Vector3 lastClickedPoint;
    public Vector3 distanceToCorner0;

    public Vector3 targetCellDelta;
    public Vector3Int targetCell;

    /*public float correctionX = 1f;
    public float correctionY = 1f;*/



    private bool isAwaken = false;
    void Awake()
    {
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();

        Tilemap tilemap = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        tilemapSize = tilemap.size;

        tilemapSelector = GameObject.Find("TilemapSelector").GetComponent<TilemapSelector>();

        minimapSpriteSize = GetComponent<RectTransform>().sizeDelta;

        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);
     
        minX = tilemap.cellBounds.xMin;
        minY = tilemap.cellBounds.yMin;
        maxX = tilemap.cellBounds.xMax;
        maxY = tilemap.cellBounds.yMax;

        sizeX = tilemap.cellBounds.size.x;
        sizeY = tilemap.cellBounds.size.y;
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
                lastClickedPoint = new (clickPosition.x, clickPosition.y, 0);

                distanceToCorner0 = new (lastClickedPoint.x - corners[0].x, lastClickedPoint.y - corners[0].y, 0);

                relativeDistanceToCorner0 = new (
                    (distanceToCorner0.x / minimapSpriteSize.x),
                    (distanceToCorner0.y / minimapSpriteSize.y),
                    0f);

                targetCellDelta = new Vector3(relativeDistanceToCorner0.x * sizeX, relativeDistanceToCorner0.y * sizeY, 0);

                /*Vector2 pixelsInWorld = new (
                    relativeDistanceToCorner0.x * tilemapSize.x,
                    relativeDistanceToCorner0.y * tilemapSize.y);

                Vector2 target = new(
                    firstCornerWorldPosition.x + pixelsInWorld.x,
                    firstCornerWorldPosition.y + pixelsInWorld.y);*/

                //target.x += distanceToCorner0.x * decayPercentage.x;
                //target.y += distanceToCorner0.y * decayPercentage.y;

                //target.x += distanceToCorner0.x;
                //target.y += distanceToCorner0.y;

                //cameraController.LookToImmediate(target);

                targetCell = new Vector3Int(minX + (int)targetCellDelta.x, minY + (int)targetCellDelta.y, 0);
                cameraController.LookToCell(targetCell);
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

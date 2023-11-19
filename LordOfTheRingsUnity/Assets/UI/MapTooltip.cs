using TMPro;
using UnityEngine;

public class MapTooltip : MonoBehaviour
{
    public TextMeshPro hex;
    public SpriteRenderer terrainImage;
    public TextMeshPro movementCost;

    private TerrainManager terrainManager;
    private SpritesRepo spritesRepo;
    void Awake()
    {
        terrainManager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
    }

    public void Initialize(Vector3Int tilePos)
    {
        TileAndMovementCost info = terrainManager.GetTileAndMovementCost(new Vector2Int(tilePos.x, tilePos.y));
        if (info.cardInfo == null)
            return;

        movementCost.text = info.movementCost.ToString();        
        terrainImage.sprite = spritesRepo.GetSprite(info.cardInfo.cardType.ToString());

        hex.text = string.Format("({0},{1})", info.cellPosition.x, info.cellPosition.y);
    }
}

using TMPro;
using UnityEngine;

public enum MapTooltipEnum
{
    DEFAULT,
    CITY_ATTACK,
    CITY_ENTER
}

public class MapTooltip : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer tooltipImage;
    [SerializeField]
    private TextMeshPro hex;
    [SerializeField]
    private SpriteRenderer terrainImage;
    [SerializeField]
    private TextMeshPro movementCost;

    [SerializeField]
    private Sprite defaultTooltip;
    [SerializeField]
    private Sprite cityAttackTooltip;
    [SerializeField]
    private Sprite cityEnterTooltip;

    private TerrainManager terrainManager;
    private SpritesRepo spritesRepo;
    void Awake()
    {
        terrainManager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
    }

    public void Initialize(Vector3Int tilePos, MapTooltipEnum mapTooltipType)
    {
        TileAndMovementCost info = terrainManager.GetTileAndMovementCost(new Vector2Int(tilePos.x, tilePos.y));
        if (info.cardInfo == null)
            return;

        tooltipImage.sprite = mapTooltipType switch
        {
            MapTooltipEnum.CITY_ATTACK => cityAttackTooltip,
            MapTooltipEnum.CITY_ENTER => cityEnterTooltip,
            _ => defaultTooltip,
        };
        movementCost.text = info.movementCost.ToString();        
        terrainImage.sprite = spritesRepo.GetSprite(info.cardInfo.cardType.ToString());

        hex.text = string.Format("({0},{1})", info.cellPosition.x, info.cellPosition.y);
    }
}

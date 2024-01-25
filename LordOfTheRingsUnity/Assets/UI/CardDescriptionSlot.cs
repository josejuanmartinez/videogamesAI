using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDescriptionSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image imageIcon;
    public SimpleTooltip tooltip;

    private Game game;
    private SpritesRepo spritesRepo;
    private TooltipRepo tooltipRepo;

    private string leftTooltipInfo;
    private string rightTooltipInfo;

    private bool awaken = false;
    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        tooltipRepo = GameObject.Find("TooltipRepo").GetComponent<TooltipRepo>();
        awaken = true;
    }
    public bool Initialize(string title, string stringId, Sprite fallbackIcon = null)
    {
        if (!awaken)
            Awake();
        if (string.IsNullOrEmpty(stringId))
            return false;

        if (fallbackIcon == null)
            fallbackIcon = spritesRepo.GetSprite("default");
                
        imageIcon.sprite = spritesRepo.ExistsSprite(stringId) ? spritesRepo.GetSprite(stringId) : fallbackIcon;
        tooltip.simpleTooltipStyle = tooltipRepo.tooltipStyle;
        leftTooltipInfo = string.Format("<b>{0}</b> ", GameObject.Find("Localization").GetComponent<Localization>().Localize(title));
        rightTooltipInfo = GameObject.Find("Localization").GetComponent<Localization>().LocalizeTooltipRight(stringId);
        return true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (game.IsPopup())
            return;

        if (tooltip != null)
            tooltip.ShowTooltip(leftTooltipInfo, rightTooltipInfo);
    }    
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.HideTooltip();
    }
}

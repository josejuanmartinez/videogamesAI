using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDescriptionSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image imageIcon;
    public SimpleTooltip tooltip;

    private SpritesRepo spritesRepo;
    private TooltipRepo tooltipRepo;
    
    private bool awaken = false;
    void Awake()
    {
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
        tooltip.infoRight = GameObject.Find("Localization").GetComponent<Localization>().LocalizeTooltipRight(stringId);
        tooltip.infoLeft = string.Format("<b>{0}</b> ", GameObject.Find("Localization").GetComponent<Localization>().Localize(title));
        
        return true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.HideTooltip();
    }
}

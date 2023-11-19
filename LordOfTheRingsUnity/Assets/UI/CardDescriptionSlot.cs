using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDescriptionSlot : MonoBehaviour
{
    private SpritesRepo spritesRepo;
    private TooltipRepo tooltipRepo;
    private Image imageIcon;
    private SimpleTooltip tooltip;
    private bool awaken = false;
    void Awake()
    {
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        tooltipRepo = GameObject.Find("TooltipRepo").GetComponent<TooltipRepo>();
        if(GetComponent<Image>() == null)
            imageIcon = gameObject.AddComponent<Image>();
        if(GetComponent<SimpleTooltip>() == null)
            tooltip = gameObject.AddComponent<SimpleTooltip>();
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
    
}

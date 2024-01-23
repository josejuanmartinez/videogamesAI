using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckCardUIRequirement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public TextMeshProUGUI text;
    public SimpleTooltip tooltip;
    public string requirementName;
    
    private SpritesRepo spritesRepo;

    private string leftTooltipInfo;
    private string rightTooltipInfo;

    void Awake()
    {
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
    }
    public void Initialize(string stringId, int value)
    {
        requirementName = stringId;
        image.sprite = spritesRepo.GetSprite(stringId);
        text.text = value > 1 ? value.ToString() : "";
        leftTooltipInfo = GameObject.Find("Localization").GetComponent<Localization>().Localize(stringId);
        rightTooltipInfo = text.text;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowTooltip(leftTooltipInfo, rightTooltipInfo);
    }
}

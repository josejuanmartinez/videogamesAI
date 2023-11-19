using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text), typeof(SimpleTooltip))]
public class TextTooptip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SimpleTooltipStyle style;
    private TextMeshProUGUI text;
    private SimpleTooltip simpleTooltip;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        simpleTooltip = GetComponent<SimpleTooltip>();
        simpleTooltip.enabled = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);  // If you are not in a Canvas using Screen Overlay, put your camera instead of null
        if (linkIndex != -1)
        {
            simpleTooltip.enabled = true;
            simpleTooltip.ShowTooltip();
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            Debug.Log(linkInfo);
            simpleTooltip.simpleTooltipStyle = style;
            simpleTooltip.infoLeft = GameObject.Find("Localization").GetComponent<Localization>().LocalizeTooltipRight(linkInfo.GetLinkID());
            simpleTooltip.infoRight = GameObject.Find("Localization").GetComponent<Localization>().Localize(linkInfo.GetLinkID());
            
        }
        else
        {
            simpleTooltip.HideTooltip();
            simpleTooltip.enabled = false;
        }        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        simpleTooltip.HideTooltip();
        simpleTooltip.enabled = false;
    }
}
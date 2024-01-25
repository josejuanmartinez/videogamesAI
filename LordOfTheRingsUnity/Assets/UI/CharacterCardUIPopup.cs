
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardUIPopup : CharacterCardUI
{
    [Header("Character Card UI Popup")]
    public TextMeshProUGUI prowessText;
    public TextMeshProUGUI defenceText;
    public GameObject targetedPrefab;
    public override bool Initialize(string cardId, NationsEnum owner, bool refresh = false)
    {
        if (!base.Initialize(cardId, owner, refresh))
            return false;

        initialized = false;

        prowessText.text = GetTotalProwess().ToString();
        prowessText.color = GetTotalProwessColor();
        defenceText.text = GetTotalDefence().ToString();
        defenceText.color = GetTotalDefenceColor();
        UndrawTargetted();

        initialized = true;

        return initialized;
    }

    public void UndrawTargetted()
    {
        targetedPrefab.SetActive(false);
    }
    public void DrawTargetted()
    {
        targetedPrefab.SetActive(true);
    }
}

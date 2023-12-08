
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardUIPopup : CharacterCardUI
{
    [Header("Character Card UI Popup")]
    public GridLayoutGroup targettedGrid;
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
        int children = targettedGrid.transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(targettedGrid.transform.GetChild(0).gameObject);
    }
    public void DrawTargetted()
    {
        Instantiate(targetedPrefab, targettedGrid.transform);
    }
}

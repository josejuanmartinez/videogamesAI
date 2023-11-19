using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyCardUIPopup : AllyCardUI
{
    [Header("Character Card UI Popup")]
    public GridLayoutGroup targettedGrid;
    public TextMeshProUGUI prowessText;
    public TextMeshProUGUI defenceText;
    public override bool Initialize(string cardId, NationsEnum owner)
    {
        if (!base.Initialize(cardId, owner))
            return false;

        prowessText.text = GetTotalProwess().ToString();
        prowessText.color = GetTotalProwessColor();
        defenceText.text = GetTotalDefence().ToString();
        defenceText.color = GetTotalDefenceColor();
        UndrawTargetted();

        initialized = true;

        return true;
    }

    public void UndrawTargetted()
    {
        int children = targettedGrid.transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(targettedGrid.transform.GetChild(0).gameObject);
    }
    public void DrawTargetted()
    {
        GameObject go = new("strike");
        go.transform.SetParent(targettedGrid.transform);
        Image img = go.AddComponent<Image>();
        img.transform.localScale = Vector3.one;
        img.sprite = spritesRepo.GetSprite("target");
    }
}

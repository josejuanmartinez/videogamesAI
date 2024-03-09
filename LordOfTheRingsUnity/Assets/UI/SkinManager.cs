using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public List<NationsEnum> nations;
    public List<Sprite> nationSkinSprite;

    public Image topBackground;
    public Image bottomBackground;

    public Image topBackgroundBar;
    public Image bottomBackgroundBar;

    public Image rightTrack;
    public Image mapFrame;
    public Image cardFrame;

    public Image nationIconOrnament;

    void Awake()
    {
        ColorManager colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        Settings settings = GameObject.Find("Settings").GetComponent<Settings>();
        NationsEnum player = settings.GetHumanPlayer();

        Sprite playerSkin = GetSkinImageByNation(player);
        Color playerColor = colorManager.GetNationColor(player);

        topBackground.sprite = playerSkin;
        bottomBackground.sprite = playerSkin;

        topBackgroundBar.color = playerColor;
        bottomBackgroundBar.color = playerColor;

        rightTrack.color = playerColor;
        mapFrame.color = playerColor;
        cardFrame.color = playerColor;

        nationIconOrnament.color = playerColor;
    }

    public Sprite GetSkinImageByNation(NationsEnum nation)
    {
        return nationSkinSprite[nations.IndexOf(nation)];
    }
}

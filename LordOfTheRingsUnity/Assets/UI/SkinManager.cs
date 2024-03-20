using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : MonoBehaviour
{
    public List<NationsEnum> nations;
    public List<Sprite> nationSkinSprite;

    public Image topBackground;
    public Image bottomBackground;

    public Image rightTrack;
    public Image nextTurn;

    ColorManager colorManager;
    Settings settings;
    Sprite playerSkin;
    Color playerColor;

    void Awake()
    {
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        settings = GameObject.Find("Settings").GetComponent<Settings>();
        
        playerSkin = GetSkinImageByNation(settings.GetHumanPlayer());
        playerColor = colorManager.GetNationColor(settings.GetHumanPlayer());
        StartCoroutine(ApplySkin());
    }

    public Sprite GetSkinImageByNation(NationsEnum nation)
    {
        return nationSkinSprite[nations.IndexOf(nation)];
    }

    IEnumerator ApplySkin()
    {
        const short MAX_TRIES = 5;
        short tries = MAX_TRIES;
        while (tries > 0)
        {
            if (tries < MAX_TRIES)
                yield return new WaitForSeconds(1);

            try
            {
                topBackground.sprite = playerSkin;
                bottomBackground.sprite = playerSkin;

                rightTrack.color = playerColor;
                nextTurn.color = playerColor;
                break;
            }
            catch
            {
                tries--;
            }
        }
        yield return null;
    }
}

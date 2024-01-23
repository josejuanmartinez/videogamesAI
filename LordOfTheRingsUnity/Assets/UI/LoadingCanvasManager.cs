using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvasManager : MonoBehaviour
{
    public Sprite[] loadingBackgroundSprites;
    public Image loadingBackgroundImage;
    public CanvasGroup loadingBackgroundCanvasGroup;

    // Start is called before the first frame update
    public void Hide()
    {
        loadingBackgroundCanvasGroup.alpha = 0;
    }
    public void Show()
    {
        loadingBackgroundImage.sprite = loadingBackgroundSprites[Random.Range(0, loadingBackgroundSprites.Length)];
        loadingBackgroundCanvasGroup.alpha = 1;
    }
}

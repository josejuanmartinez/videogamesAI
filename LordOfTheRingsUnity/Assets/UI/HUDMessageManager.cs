using System.Collections;
using TMPro;
using UnityEngine;

public class HUDMessageManager : MonoBehaviour
{
    public float delayToClose;

    private ColorManager colorManager;

    private void Awake()
    {
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
    }

    public void ShowMessage(CardUI card, string message, bool success)
    {
        card.AddMessage(message, delayToClose, success ? colorManager.GetColor("success"): colorManager.GetColor("failure"));
    }
    public void ShowMessage(CardUI card, string message, Color color)
    {
        card.AddMessage(message, delayToClose, color);
    }

}

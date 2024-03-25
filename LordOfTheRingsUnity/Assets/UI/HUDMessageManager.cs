using UnityEngine;

public class HUDMessageManager : MonoBehaviour
{
    [SerializeField]
    private float delayToClose = 1f;
    [SerializeField]
    private bool bDebug = false;
    
    private HudGlobal hudGlobal;
    private ColorManager colorManager;

    private void Awake()
    {
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        hudGlobal = GameObject.Find("HUDGlobal").GetComponent<HudGlobal>();
    }

    public void ShowMessage(CardUI card, string message, bool success)
    {
        card.AddMessage(message, delayToClose, success ? colorManager.GetColor("success"): colorManager.GetColor("failure"));
    }
    public void ShowMessage(CityUI city, string message, bool success)
    {
        city.AddMessage(message, delayToClose, success ? colorManager.GetColor("success") : colorManager.GetColor("failure"));
    }

    public void ShowMessage(CardUI card, string message, Color color)
    {
        card.AddMessage(message, delayToClose, color);
    }

    public void ShowGlobalHUDMessage(string message, Sprite pre, Sprite post)
    {
        hudGlobal.Initialize(message, pre, post);
    }
    public void ShowGlobalHUDMessage(string message, string preImageString, string postImageString)
    {
        hudGlobal.Initialize(message, preImageString, postImageString);
    }
    public void ShowGlobalHUDMessage(string message, Sprite pre)
    {
        hudGlobal.Initialize(message, pre);
    }
    public void ShowGlobalHUDMessage(string message, string preImageString)
    {
        hudGlobal.Initialize(message, preImageString);
    }
    public void ShowGlobalHUDMessage(string message)
    {
        hudGlobal.Initialize(message);
    }

    void Update()
    {
        if(bDebug)
        {
            ShowGlobalHUDMessage("-");
            bDebug = false;
        }         
    }
}

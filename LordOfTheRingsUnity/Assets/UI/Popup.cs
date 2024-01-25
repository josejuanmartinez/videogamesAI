using UnityEngine;

public class Popup : MonoBehaviour
{
    public GameObject popup;

    private Game game;
    private bool isInitialized = false;
    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        isInitialized = true;
    }

    public virtual void ShowPopup()
    {   
        if (!isInitialized)
            Awake();
        popup.SetActive(true);
        game.SetIsPopup(true);
    }

    public virtual void HidePopup()
    {
        if (!isInitialized)
            Awake();
        popup.SetActive(false);
        game.SetIsPopup(false);
    }

    public bool IsShown()
    {
        return popup.activeSelf;
    }
}

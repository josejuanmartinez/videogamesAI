using UnityEngine;

public class Popup : MonoBehaviour
{
    public GameObject popup;
    [SerializeField]
    private string openSound = "popup";
    [SerializeField]
    private string closeSound = "buttonCancel";

    private Game game;
    private AudioManager audioManager;
    private AudioRepo audioRepo;
    private bool isInitialized = false;
    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
        isInitialized = true;
    }

    public virtual void ShowPopup()
    {   
        if (!isInitialized)
            Awake();
        popup.SetActive(true);
        game.SetIsPopup(true);
        audioManager.PlaySound(audioRepo.GetAudio(openSound));
    }

    public virtual void HidePopup()
    {
        if (!isInitialized)
            Awake();
        popup.SetActive(false);
        game.SetIsPopup(false);
        audioManager.PlaySound(audioRepo.GetAudio(closeSound));
    }

    public bool IsShown()
    {
        return popup.activeSelf;
    }
}

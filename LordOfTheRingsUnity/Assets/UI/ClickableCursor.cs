using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string sound = "clickable";
    [SerializeField]
    private string cursor = "clickable";

    private Mouse mouse;
    private AudioManager audioManager;
    private AudioRepo audioRepo;


    public void Awake()
    {
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse.ChangeCursor(cursor);
        audioManager.PlaySound(audioRepo.GetAudio(sound));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse.RemoveCursor();
    }

}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct OkOption
{
    public string text;
    public UnityAction cardBoolFunc;
}

public class InputPopupManager : Popup
{
    public GameObject buttonsLayout;
    public GameObject okButton;
    public GameObject cancelButton;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image imageLeft;

    public void Initialize(string title, string description, Sprite imageLeft, List<OkOption> options, UnityAction cancel)
    {
        ShowPopup(false);
        this.description.text = description;
        this.title.text = title;

        this.imageLeft.sprite = imageLeft;
        this.imageLeft.color = imageLeft != null ? Color.white : Color.clear;

        int children = buttonsLayout.transform.childCount;
        for (int i=0; i<children; i++)
            DestroyImmediate(buttonsLayout.transform.GetChild(0).gameObject);

        for(int i=0; i< options.Count; i++)
        {
            GameObject goButton = Instantiate(okButton, buttonsLayout.transform);
            TextMeshProUGUI text = goButton.GetComponentInChildren<TextMeshProUGUI>();
            text.text = options[i].text;
            Button button = goButton.GetComponentInChildren<Button>();
            button.onClick.AddListener(Ok);
            button.onClick.AddListener(options[i].cardBoolFunc);
        
        }

        GameObject goCancelButton = Instantiate(cancelButton, buttonsLayout.transform);
        goCancelButton.GetComponentInChildren<Button>().onClick.AddListener(cancel);
    }

    public void Initialize(string title, string description, Sprite imageLeft, List<OkOption> options)
    {
        Initialize(title, description, imageLeft, options, Cancel);
    }

    public void Ok()
    {
        audioManager.PlaySound(audioRepo.GetAudio("buttonOk"));
    }
    public void Cancel()
    {
        HidePopup();
        audioManager.PlaySound(audioRepo.GetAudio("buttonCancel"));
    }
}

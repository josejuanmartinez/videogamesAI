using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardQuoteSlot : MonoBehaviour
{
    public TextMeshProUGUI description;
    private bool awaken = false;
    void Awake()
    {
        description.text = "";
        awaken = true;
    }
    
    public bool Initialize(string stringId)
    {
        if (!awaken)
            Awake();
        if (string.IsNullOrEmpty(stringId))
            return false;

        description.text = GameObject.Find("Localization").GetComponent<Localization>().LocalizeQuote(stringId);

        return true;
    }
}

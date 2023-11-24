using TMPro;
using UnityEngine;

[RequireComponent (typeof(TextMeshProUGUI))]
public class AutoLocalize : MonoBehaviour
{
    bool initialized = false;
    Localization localization;
    bool localize = false;
    // Start is called before the first frame update
    void Initialize()
    {
        localization = GameObject.Find("Localization").GetComponent<Localization>();    
        initialized = localization.IsInitialized();
        localize = !string.IsNullOrEmpty(GetComponent<TextMeshProUGUI>().text);
        if(initialized && localize)
            GetComponent<TextMeshProUGUI>().text = localization.Localize(GetComponent<TextMeshProUGUI>().text);
    }

    void Update()
    {
        if(!initialized)
            Initialize();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class RegionSelectionBehaviour : MonoBehaviour
{
    private bool isCharactersShown;
    private GalleryLevelSelectionManager regionSelector;
    private GalleryLevelSelectionManager characterSelector;
    private MenuCameraController caRegionsCamera;
    private Button button;
    private bool isInitialized;

    void Awake()
    {
        regionSelector = GameObject.Find("RegionSelection").GetComponent<GalleryLevelSelectionManager>();
        caRegionsCamera = Camera.main.GetComponent<MenuCameraController>();
        button = GetComponent<Button>();
        button.onClick.AddListener(Toggle);
        isInitialized = false;
        isCharactersShown = false;
    }

    void Update()
    {
        if (!isInitialized)
            return;
        if (isCharactersShown && Input.GetKeyUp(KeyCode.Escape))
            Toggle();
    }

    public void Initialize(GalleryLevelSelectionManager gallery)
    {
        characterSelector = gallery;
        isInitialized = true;
    }

    public void Toggle() {
        if (!isInitialized)
            return;
        isCharactersShown = !isCharactersShown;
        characterSelector.gameObject.SetActive(isCharactersShown);

        if (!isCharactersShown)
        {
            SimpleTooltip st = characterSelector.transform.GetComponentInChildren<SimpleTooltip>();
            if (st)
                st.HideTooltip();

            caRegionsCamera.LookToRegions();
        } 
        else
            caRegionsCamera.LookToCharacters();

        characterSelector.gameObject.SetActive(isCharactersShown);
        characterSelector.Start();

        regionSelector.enabled = !isCharactersShown;
    }

}

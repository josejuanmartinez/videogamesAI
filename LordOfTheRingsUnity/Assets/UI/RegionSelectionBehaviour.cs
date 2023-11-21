using UnityEngine;
using UnityEngine.UI;

public class RegionSelectionBehaviour : MonoBehaviour
{
    public NationRegionsEnum region;
    public GameObject characterSelector;
    public bool isCharactersShown;

    private GalleryLevelSelectionManager galRegionSelector;
    private MenuCameraController caRegionsCamera;
    private Button button;

    void Awake()
    {
        galRegionSelector = GameObject.Find("RegionSelection").GetComponent<GalleryLevelSelectionManager>();
        caRegionsCamera = Camera.main.GetComponent<MenuCameraController>();
        button = GetComponent<Button>();
        button.onClick.AddListener(Toggle);
    }

    void Update()
    {
        if (isCharactersShown && Input.GetKeyUp(KeyCode.Escape))
            Toggle();
    }

    public void Toggle() {
        isCharactersShown = !isCharactersShown;

        if (!isCharactersShown)
        {
            SimpleTooltip st = characterSelector.transform.GetComponentInChildren<SimpleTooltip>();
            if (st)
                st.HideTooltip();

            caRegionsCamera.LookToRegions();
        } 
        else
            caRegionsCamera.LookToCharacters();

        characterSelector.SetActive(isCharactersShown);
        galRegionSelector.enabled = !isCharactersShown;
    }

}

using UnityEngine;

public class RegionSelectionBehaviour : MonoBehaviour
{
    public GameObject characterSelector;
    public bool isCharactersShown;

    private GalleryLevelSelectionManager galRegionSelector;
    private MenuCameraController caRegionsCamera;

    private void Awake()
    {
        galRegionSelector = GameObject.Find("RegionSelection").GetComponent<GalleryLevelSelectionManager>();
        caRegionsCamera = Camera.main.GetComponent<MenuCameraController>();
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
        } else
        {
            caRegionsCamera.LookToCharacters();
        }

        characterSelector.SetActive(isCharactersShown);
        galRegionSelector.enabled = !isCharactersShown;
    }

}

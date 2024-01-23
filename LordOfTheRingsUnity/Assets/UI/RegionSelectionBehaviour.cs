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

    private float lastClick = 0f;

    void Awake()
    {
        regionSelector = GameObject.Find("RegionSelection").GetComponent<GalleryLevelSelectionManager>();
        caRegionsCamera = Camera.main.GetComponent<MenuCameraController>();
        button = GetComponent<Button>();
        button.onClick.AddListener(Click);
        isInitialized = false;
        isCharactersShown = false;
    }

    void Update()
    {
        if (!isInitialized)
            return;
        if (isCharactersShown && Input.GetKeyUp(KeyCode.Escape))
            Unselect();
    }

    public void Initialize(GalleryLevelSelectionManager gallery)
    {
        characterSelector = gallery;
        isInitialized = true;
    }

    public void Unselect()
    {
        if (!isInitialized)
            return;
        isCharactersShown = false;
        characterSelector.gameObject.SetActive(isCharactersShown);

        SimpleTooltip st = characterSelector.transform.GetComponentInChildren<SimpleTooltip>();
        if (st)
            st.HideTooltip();

        caRegionsCamera.LookToRegions();
    }

    public void Click() {
        if (!isInitialized)
            return;

        bool doubleClick = Time.time - lastClick < 0.5f;
        lastClick = Time.time;

        if(doubleClick)
        {
            isCharactersShown = true;
            characterSelector.gameObject.SetActive(isCharactersShown);

            caRegionsCamera.LookToCharacters();
            characterSelector.gameObject.SetActive(isCharactersShown);
            characterSelector.Start();
        }

        regionSelector.enabled = !isCharactersShown;
    }

}

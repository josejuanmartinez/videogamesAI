using UnityEngine;
using UnityEngine.UI;

public class RegionCharacterSelectionBehaviour : MonoBehaviour
{
    private NationsEnum nation;
    private Button button;

    private Settings settings;
    private StartManager startManager;

    private float lastClick = 0f;

    void Awake()
    {
        settings = GameObject.Find("Settings").GetComponent<Settings>();
        startManager = GameObject.Find("StartManager").GetComponent<StartManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(Click);
    }
    public void Initialize(NationsEnum nation)
    {
        this.nation = nation;
    }

    public void Click()
    {
        bool doubleClick = Time.time - lastClick < 0.5f;
        lastClick = Time.time;

        if (doubleClick)
        {
            settings.SetHumanPlayer(nation);
            Debug.Log(string.Format("Starting the game with {0}", nation.ToString()));
            startManager.StartGame();
        }
    }
}

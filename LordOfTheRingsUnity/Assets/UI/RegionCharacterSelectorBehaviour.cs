using UnityEngine;
using UnityEngine.UI;

public class RegionCharacterSelectionBehaviour : MonoBehaviour
{
    private NationsEnum nation;
    private Button button;

    private Settings settings;
    private StartManager startManager;

    void Awake()
    {
        settings = GameObject.Find("Settings").GetComponent<Settings>();
        startManager = GameObject.Find("StartManager").GetComponent<StartManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(Toggle);
    }
    public void Initialize(NationsEnum nation)
    {
        this.nation = nation;
    }

    public void Toggle()
    {
        settings.SetHumanPlayer(nation);
        Debug.Log(string.Format("Starting the game with {0}", nation.ToString()));
        startManager.StartGame();
    }

}

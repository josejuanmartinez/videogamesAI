using UnityEngine;
using UnityEngine.UI;

public class RegionCharacterSelectionBehaviour : MonoBehaviour
{
    private NationsEnum nation;
    private Button button;

    private Settings settings;
    private StartGameManager startGameManager;

    private float lastClick = 0f;

    void Awake()
    {
        settings = GameObject.Find("Settings").GetComponent<Settings>();
        startGameManager = GameObject.Find("StartGameManager").GetComponent<StartGameManager>();
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
            startGameManager.StartGame();
        }
    }
}

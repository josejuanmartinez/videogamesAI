using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Attacker : MonoBehaviour
{
    public float secondsToResult = 1.5f;
    public Image attacker;
    public Button button;
    public Image result;
    public CanvasGroup canvasGroup;

    public TextMeshProUGUI prowessText;
    public TextMeshProUGUI defenceText;

    protected int attackerNum;
    protected bool resolved = false;
    protected bool initialized = false;
    protected bool isAwaken = false;

    protected DiceManager diceManager;
    protected SpritesRepo spritesRepo;
    protected CardDetailsRepo cardRepo;
    protected PlaceDeck placeDeck;
    protected Game game;

    protected Type targetType;
    protected Dictionary<string, CardUI> company;
    protected HazardCreatureCardDetails attackerDetails;


    void Awake()
    {
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        cardRepo = GameObject.Find("CardDetailsRepo").GetComponent<CardDetailsRepo>();
        placeDeck = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>();
        game = GameObject.Find("Game").GetComponent<Game>();
        isAwaken = true;
    }

    public virtual bool Initialize(string cardId, Dictionary<string, CardUI> company, int attackerNum, NationsEnum owner)
    {
        if (!isAwaken)
            Awake();

        this.company = new Dictionary<string, CardUI>(company);

        this.attackerNum = attackerNum;
        result.enabled = false;
        canvasGroup.alpha = 1;

        CardDetails details = cardRepo.GetCardDetails(cardId, owner);
        if (details == null)
        {
            Debug.LogError(string.Format("{0} not found in cardRepo, but was trying to spawn it as attacker", cardId));
            return false;
        }

        attackerDetails = details as HazardCreatureCardDetails;
        if (attackerDetails == null)
        {
            Debug.LogError(string.Format("{0} should be a hazard creature to spawn as attacker", cardId));
            return false;
        }

        attacker.sprite = attackerDetails.cardSprite;
        prowessText.text = attackerDetails.prowess.ToString();
        defenceText.text = attackerDetails.defence.ToString();

        initialized = true;

        return true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (!initialized)
            return;
        StartCoroutine(diceManager.RollForCombat(attackerNum));
    }
}

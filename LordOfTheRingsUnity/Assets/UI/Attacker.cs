using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Attacker : MonoBehaviour
{
    public float secondsToResult = 1.5f;
    public Image attacker;
    public Button button;
    public Image result;
    public CanvasGroup canvasGroup;

    public TextMeshProUGUI prowessText;
    public TextMeshProUGUI defenceText;
    
    public float waitForAnimationBase = 2f; 
    public float moveSpeed = 2f;

    protected float waitForAnimation;

    protected CardUI target;
    protected NationsEnum attackerNation;
    protected int attackerNum;

    protected bool resolved = false;
    protected bool initialized = false;
    protected bool isAwaken = false;
    protected bool isAnimating = false;
    protected RacesEnum race;

    protected DiceManager diceManager;
    protected SpritesRepo spritesRepo;
    protected CardDetailsRepo cardRepo;
    protected PlaceDeck placeDeck;
    protected Game game;

    protected Type targetType;
    protected Dictionary<string, CardUI> company;
    protected HazardCreatureCardDetails attackerDetails;

    private Vector3 NONE = Vector3.one * int.MinValue;
    private Vector3 moveTo = Vector3.one * int.MinValue;
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
        int target_num = UnityEngine.Random.Range(0, this.company.Count);
        target = this.company[this.company.Keys.ToList()[target_num]];
        attackerNation = owner;

        this.attackerNum = attackerNum;

        //  waitForAnimation = waitForAnimationBase * (attackerNum + 1);
        waitForAnimation = waitForAnimationBase;

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
        race = attackerDetails.race;

        initialized = true;

        return true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator AttackAnimation()
    {
        isAnimating = true;
        yield return new WaitForSecondsRealtime(waitForAnimation);
        moveTo = target.gameObject.transform.position;
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    void Update()
    {
        if (moveTo != NONE)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, moveTo, Time.deltaTime * moveSpeed);
            transform.position = newPosition;
            float distanceToDestination = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToDestination < 2f)
            {
                transform.position = moveTo;
                moveTo = NONE;
                isAnimating = false;
            }   
        }
    }
}

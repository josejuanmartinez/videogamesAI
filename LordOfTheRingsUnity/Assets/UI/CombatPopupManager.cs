using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CombatPopupType
{
    AutomaticAttack,
    MovementAttack,
    CreatureAttack
}

public class CombatPopupManager : Popup
{
    public short maxAttacks = 4;
    public short secondsToResult = 3;
    public CompanyManager combatCompanyManager;

    public Image citySprite;
    public TextMeshProUGUI title;
    public GameObject attackersLayout;
    public GameObject attackerToCompany;
    public GameObject attackerToCreature;

    private CardDetails leaderCardDetails;
    private CardUI leader;
    private CityUI city;
    private List<CardDetails> company;

    private HUDMessageManager hudMessageManager;
    private SelectedItems selectedItems;
    private DeckManager deckManager;
    private Turn turn;
    private SpritesRepo spritesRepo;
    private Board board;

    private int processedAttacks;
    private int attackersNum;
    private bool noHurts;
    private bool isAwaken = false;

    private CombatPopupType combatPopupType;

    void Awake()
    {
        hudMessageManager = GameObject.Find("HUDMessageManager").GetComponent<HUDMessageManager>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        board = GameObject.Find("Board").GetComponent<Board>();

        processedAttacks = 0;
        attackersNum = 0;
        noHurts = true;
        isAwaken = true;
}
    public bool Initialize(CardUI leader, CityUI city)
    {
        if (!isAwaken)
            Awake();

        if (leader.GetDetails() == null)
            return false;

        this.leader = leader;        
        leaderCardDetails = leader.GetDetails();
        combatPopupType = leaderCardDetails.IsClassOf(CardClass.Character) ? CombatPopupType.AutomaticAttack : CombatPopupType.CreatureAttack;

        if (city == null)
            return false;
        this.city = board.GetCityManager().GetCityUI(city.GetCityId());
        if (!leaderCardDetails.IsClassOf(CardClass.Character) && !leaderCardDetails.IsClassOf(CardClass.HazardCreature))
            return false;

        noHurts = true;
        processedAttacks = 0;

        if(!combatCompanyManager.Initialize(leader.GetOwner()))
            return false;

        company = combatCompanyManager.GetCompany();
        if (company == null || company.Count < 1)
            return false;

        title.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(city.GetCityId());
        citySprite.sprite = city.GetSprite();

        List<string> attacks = board.GetCityManager().GetCityUI(city.GetCityId()).GetAutomaticAttacks(leader.GetOwner());
        if(attacks.Count < 1)
            return false;

        attackersNum = attacks.Count;

        NationsEnum ownerOfCity = board.GetCityManager().GetCityOwner(city.GetCityId());

        short counter = 0;
        foreach (string attack in attacks)
        {
            if(leaderCardDetails.IsClassOf(CardClass.Character))
            {
                GameObject goAttacker = Instantiate(
                attackerToCompany,
                attackersLayout.transform);
                goAttacker.GetComponent<AttackerToCompany>().Initialize(
                    attack,
                    combatCompanyManager.GetAllCombatCards(),
                    counter,
                    ownerOfCity
                );
            }
            else if (leaderCardDetails.IsClassOf(CardClass.HazardCreature))
            {
                GameObject goAttacker = Instantiate(
                attackerToCreature,
                attackersLayout.transform);
                goAttacker.GetComponent<AttackerToCreature>().Initialize(
                    attack,
                    combatCompanyManager.GetAllCombatCards(),
                    counter,
                    ownerOfCity
                );
            }
            
            counter++;
            if (counter >= maxAttacks)
                break;
        }

        ShowPopup();

        return true;
    }
    public bool Initialize(CardUI leader, List<Tuple<string, NationsEnum>> attackingCards, string placeId)
    {
        /* MOVEMENT ATTACK TO CHARACTERS OR CREATURES */

        if (!isAwaken)
            Awake();

        combatPopupType = CombatPopupType.MovementAttack;

        if (leader.GetDetails() == null)
            return false;

        this.leader = leader;
        leaderCardDetails = leader.GetDetails();
        if (attackingCards == null)
            return false;
        if (attackingCards.Count < 1)
            return false;
        if (!leaderCardDetails.IsClassOf(CardClass.Character))
            return false;

        noHurts = true;
        processedAttacks = 0;        

        if (!combatCompanyManager.Initialize(leader.GetOwner()))
            return false;

        company = combatCompanyManager.GetCompany();
        if (company == null || company.Count < 1)
            return false;

        title.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(placeId);
        citySprite.sprite = spritesRepo.GetSprite(placeId);

        attackersNum = Math.Min(attackingCards.Count, maxAttacks);

        short counter = 0;
        foreach (Tuple<string, NationsEnum> attackTuple in attackingCards)
        {
            string attack = attackTuple.Item1;
            NationsEnum attackOwner = attackTuple.Item2;

            GameObject goAttacker = Instantiate(
                leaderCardDetails.IsClassOf(CardClass.Character) ? attackerToCompany : attackerToCreature,
                attackersLayout.transform);
            
            goAttacker.GetComponent<Attacker>().Initialize(
                attack, 
                combatCompanyManager.GetAllCombatCards(),
                counter,
                attackOwner
                );
            counter++;
            if (counter >= maxAttacks)
                break;
        }
        ShowPopup();

        return true;
    }

    public void GatherDiceResults(int diceValue, int attackerNum)
    {
        if (diceValue == 0)
            diceValue = DiceManager.D10;
        GameObject child = attackersLayout.transform.GetChild(attackerNum).gameObject;        
        if (child.GetComponent<AttackerToCompany>() != null)
            noHurts &= child.GetComponent<AttackerToCompany>().GatherDiceResults(diceValue);
        else if(child.GetComponent<AttackerToCreature>() != null)
            noHurts &= child.GetComponent<AttackerToCreature>().GatherDiceResults(diceValue);
        
        processedAttacks++;

        if(processedAttacks == attackersNum)
        {
            switch (combatPopupType)
            {
                case CombatPopupType.AutomaticAttack:
                    StartCoroutine(ApplyResultsAutomaticAttack());
                    break;
                case CombatPopupType.MovementAttack:
                    break;
                case CombatPopupType.CreatureAttack:
                    StartCoroutine(ApplyResultsCreatureAttack());
                    break;
            }
        }
    }
    IEnumerator ApplyResultsAutomaticAttack()
    {
        /* RESULTS ON COMPANY RESOLVING AUTOMATIC ATTACKS TO GET OBJECTS */

        CardDetails item = selectedItems.GetSelectedCardDetails();
        if (item == null)
            yield return null;

        yield return new WaitForSeconds(secondsToResult);

        hudMessageManager.ShowMessage(
            leader,
            noHurts ? GameObject.Find("Localization").GetComponent<Localization>().Localize(item.cardId) : GameObject.Find("Localization").GetComponent<Localization>().Localize("withdraw"),
            noHurts
        );

        // PLAY OBJECT
        HidePopup();

        if (noHurts)
        {
            CardUI cardUI = selectedItems.GetSelectedMovableCardUI();
            if (cardUI != null)
            {
                CharacterCardUIBoard original = cardUI as CharacterCardUIBoard;
                if (original != null)
                    original.AddObject(item);
                deckManager.DiscardAndDraw(turn.GetCurrentPlayer(), item, false);
            }
        }

        yield return null;
    }

    IEnumerator ApplyResultsCreatureAttack()
    {
        /* RESULTS ON CREATURES ATTACKING CITIES */

        if (city == null)
            yield return null;
        
        yield return new WaitForSeconds(secondsToResult);

        hudMessageManager.ShowMessage(
            leader,
            noHurts ? GameObject.Find("Localization").GetComponent<Localization>().Localize("city_damaged") : GameObject.Find("Localization").GetComponent<Localization>().Localize("city_defended"),
            noHurts
        );

        // PLAY OBJECT
        HidePopup();

        HazardCreatureCardUI hazardCreatureCardUI = leader as HazardCreatureCardUI;
        if (hazardCreatureCardUI != null)
        {
            int prowess = hazardCreatureCardUI.GetTotalProwess();
            if (noHurts)
                city.Damage(prowess);
            else
            {
                if (hazardCreatureCardUI.GetHazardCreatureDetails() != null)
                    if (hazardCreatureCardUI.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.Sieges))
                        city.Damage(prowess);
            }
        }
        yield return null;
    }

    public override void HidePopup()
    {
        int children = combatCompanyManager.transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(combatCompanyManager.transform.GetChild(0).gameObject);

        children = attackersLayout.transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(attackersLayout.transform.GetChild(0).gameObject);

        base.HidePopup();
    }
}

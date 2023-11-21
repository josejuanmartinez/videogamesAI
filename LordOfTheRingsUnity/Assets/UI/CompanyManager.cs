using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyManager : MonoBehaviour
{
    [Header("Company Manager")]
    [Header("Layouts")]
    public GameObject companyLayout;
    public GameObject cardGroup;
    public TextAnchor verticalLayoutAnchor;
    [Header("Prefabs")]
    public GameObject characterCardPrefab;
    public GameObject hazardCreatureCardPrefab;
    public GameObject objectCardPrefab;
    public GameObject allyCardPrefab;

    /*** From Selected Items ***/
    protected SelectedItems selectedItems;
    protected List<CardDetails> company;
    protected NationsEnum owner;
    private Dictionary<string, CardUI> combat;
    /***************************/

    bool isAwaken = false;

    // Start is called before the first frame update
    void Awake()
    {
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        isAwaken = true;
        combat = new();
    }

    public virtual bool Initialize(NationsEnum owner)
    {
        if (!isAwaken)
            Awake();
        this.owner = owner;
        combat.Clear();

        Show();
        //1. I clean all except leader (child==0)
        int children = companyLayout.transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(companyLayout.transform.GetChild(0).gameObject);

        if (selectedItems == null)
            return false;

        if (selectedItems.GetCompany() == null)
            return false;

        company = selectedItems.GetCompany();

        foreach (CardDetails companion in company)
            InstantiateGroup(companion);

        return true;
    }

    public void InstantiateGroup(CardDetails companion)
    {
        if (companion.IsClassOf(CardClass.Character))
            InstantiateCharGroup(companion);
        else if (companion.IsClassOf(CardClass.HazardCreature))
            InstantiateHazardCreatureGroupAndCard(companion);
        else
            return;
    }

    public void InstantiateCharGroup(CardDetails companion)
    {
        CharacterCardDetails characterDetails = (CharacterCardDetails)companion;
        if (characterDetails == null)
            return;

        GameObject cardGroupObject = Instantiate(cardGroup, companyLayout.transform);
        cardGroupObject.GetComponent<VerticalLayoutGroup>().childAlignment = verticalLayoutAnchor;
        GameObject characterObject = Instantiate(characterCardPrefab, cardGroupObject.transform);
        CharacterCardUI cardUI = characterObject.GetComponent<CharacterCardUI>();
        if (cardUI as CharacterCardUILayout != null)
            (cardUI as CharacterCardUILayout).Initialize(companion.cardId, owner);
        else if (cardUI as CharacterCardUIPopup != null)
            (cardUI as CharacterCardUIPopup).Initialize(companion.cardId, owner);
        else
            return;

        combat.Add(companion.cardId, cardUI);

        foreach (CardDetails objectCardDetails in cardUI.GetObjects())
            InstantiateObject(objectCardDetails, cardGroupObject.transform);
        foreach (CardDetails allyCardDetails in cardUI.GetAllies())
            InstantiateAlly(allyCardDetails, cardGroupObject.transform);

    }
    public void InstantiateObject(CardDetails objectCardDetails, Transform characterLayout)
    {
        if (!objectCardDetails.IsClassOf(CardClass.Object))
            return;

        GameObject goCard = Instantiate(objectCardPrefab, characterLayout);
        CardUI cardUI = goCard.GetComponent<CardUI>();
        if(cardUI != null)
            cardUI.Initialize(objectCardDetails.cardId, owner);
        return;
    }
    public void InstantiateAlly(CardDetails companion, Transform characterLayout)
    {
        if (!companion.IsClassOf(CardClass.Ally))
            return;

        GameObject allyObject = Instantiate(allyCardPrefab, characterLayout);
        AllyCardUI cardUI = allyObject.GetComponent<AllyCardUI>();
        if (cardUI as AllyCardUILayout != null)
            (cardUI as AllyCardUILayout).Initialize(companion.cardId, owner);
        else if (cardUI as AllyCardUIPopup != null)
            (cardUI as AllyCardUIPopup).Initialize(companion.cardId, owner);
        else
            return;

        combat.Add(companion.cardId, cardUI);
            
        return;
    }

    public void InstantiateHazardCreatureGroupAndCard(CardDetails companion)
    {
        HazardCreatureCardDetails hazardDetails = (HazardCreatureCardDetails)companion;
        if (hazardDetails == null)
            return;

        GameObject cardGroupObject = Instantiate(cardGroup, companyLayout.transform);
        cardGroupObject.GetComponent<VerticalLayoutGroup>().childAlignment = verticalLayoutAnchor;
        GameObject hazardObject = Instantiate(hazardCreatureCardPrefab, cardGroupObject.transform);
        HazardCreatureCardUI cardUI = hazardObject.GetComponent<HazardCreatureCardUI>();
        if (cardUI as HazardCreatureCardUILayout != null)
            (cardUI as HazardCreatureCardUILayout).Initialize(companion.cardId, owner);
        else if (cardUI as HazardCreatureCardUIPopup != null)
            (cardUI as HazardCreatureCardUIPopup).Initialize(companion.cardId, owner);
        else
            return;

        combat.Add(hazardDetails.cardId, cardUI);
        return;
    }

    public void Hide()
    {
        companyLayout.SetActive(false);
    }
    public void Show()
    {
        companyLayout.SetActive(true);
    }

    public List<CardDetails> GetCompany()
    {
        return company;
    }

    public CardUI GetCardUI(string cardId)
    {
        if (combat.ContainsKey(cardId))
            return combat[cardId];
        else
            return null;
    }

    public Dictionary<string, CardUI> GetAllCombatCards()
    {
        return combat;
    }
}

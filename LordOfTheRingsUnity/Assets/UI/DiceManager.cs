using System.Collections;
using UnityEngine;

public enum DiceRollEnum
{
    CharacterRoll,
    HazardCreatureRoll,
    ObjectRoll
}

public class DiceManager : MonoBehaviour
{
    public static short D10 = 10;
    public short maxTime = 4;

    public GameObject dice;
    public GameObject diceCanvas;

    private GameObject instantiatedDice;
    private int diceValue;

    private PlaceDeck placeDeckManager;
    private CombatPopupManager combatPopupManager;
    private ColorManager colorManager;
    private AudioManager audioManager;
    private AudioRepo audioRepo;
    private bool dicing = false;

    void Awake()
    {
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>(); 
        combatPopupManager = GameObject.Find("CombatPopupManager").GetComponent<CombatPopupManager>();
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
    }

    public IEnumerator RollToSpawnCard(SpawnCardLocation spawnCardLocation, CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(cardDetails.cardClass));
        yield return StartCoroutine(CheckResultRollToSpawnCard(spawnCardLocation, cardDetails));
    }
    public IEnumerator RollForEvents(CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(cardDetails.cardClass));
        yield return StartCoroutine(CheckResultToPlayEvent(cardDetails));
    }
    public IEnumerator RollForFactions(CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(cardDetails.cardClass));
        yield return StartCoroutine(CheckResultToInfluenceFaction(cardDetails));
    }
    public IEnumerator RollForAllies(CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(cardDetails.cardClass));
        yield return StartCoroutine(CheckResultToRecruitAlly(cardDetails));
    }
    public IEnumerator RollForGoldRing(CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(cardDetails.cardClass));
        yield return StartCoroutine(CheckResultToAnalyzeGoldRing(cardDetails));
    }
    public IEnumerator RollForCombat(int attackerNum)
    {
        yield return StartCoroutine(RollDice(CardClass.HazardCreature));
        yield return StartCoroutine(CheckResultRollToCombat(attackerNum));
    }

    public IEnumerator RollDice(CardClass cardClass)
    {
        dicing = true;
        diceCanvas.SetActive(true);
        instantiatedDice = Instantiate(dice, diceCanvas.transform);
        
        diceValue = 0;
        Die_d10 d10 = instantiatedDice.GetComponentInChildren<Die_d10>();
        
        d10.GetComponent<MeshRenderer>().material.color = colorManager.GetColor(cardClass.ToString());

        audioManager.PlaySound(audioRepo.GetAudio("dices"));
        for (int i = 0; i < maxTime; i++)
        {
            yield return new WaitForSeconds(1);
            if (d10.value != 0)
            {
                diceValue = d10.value;
                break;
            }
        }

        if (diceValue == 0)
            diceValue = Random.Range(1, 10);

        dicing = false;
        StopAllCoroutines();
    }

    public IEnumerator CheckResultRollToSpawnCard(SpawnCardLocation spawnCardLocation, CardDetails card)
    {
        yield return new WaitUntil(() => diceValue != 0);
        DestroyImmediate(instantiatedDice);
        diceCanvas.SetActive(false);
        placeDeckManager.GatherDiceResultsForSpawningCards(diceValue, spawnCardLocation, card);
    }

    public IEnumerator CheckResultToPlayEvent(CardDetails card)
    {
        yield return new WaitUntil(() => diceValue != 0);
        DestroyImmediate(instantiatedDice);
        diceCanvas.SetActive(false);
        placeDeckManager.GatherDiceResultsForPlayingEvents(diceValue, card);
    }   
    public IEnumerator CheckResultToInfluenceFaction(CardDetails card)
    {
        yield return new WaitUntil(() => diceValue != 0);
        DestroyImmediate(instantiatedDice);
        diceCanvas.SetActive(false);
        placeDeckManager.GatherDiceResultsForInfluencingFactions(diceValue, card);
    }
    public IEnumerator CheckResultToRecruitAlly(CardDetails card)
    {
        yield return new WaitUntil(() => diceValue != 0);
        DestroyImmediate(instantiatedDice);
        diceCanvas.SetActive(false);
        placeDeckManager.GatherDiceResultsForRecruitingAllies(diceValue, card);
    }
    public IEnumerator CheckResultToAnalyzeGoldRing(CardDetails card)
    {
        yield return new WaitUntil(() => diceValue != 0);
        DestroyImmediate(instantiatedDice);
        diceCanvas.SetActive(false);
        placeDeckManager.GatherDiceResultsForAnalyzingGoldRing(diceValue, card);
    }
    public IEnumerator CheckResultRollToCombat(int attackerNum)
    {
        yield return new WaitUntil(() => diceValue != 0);
        DestroyImmediate(instantiatedDice);
        diceCanvas.SetActive(false);
        combatPopupManager.GatherDiceResults(diceValue, attackerNum);
    }
    public bool IsDicing()
    {
        return dicing;
    }
}

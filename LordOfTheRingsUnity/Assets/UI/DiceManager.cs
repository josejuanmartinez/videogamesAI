using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceRollEnum
{
    CharacterRoll,
    HazardCreatureRoll,
    ObjectRoll
}

public class DiceManager : MonoBehaviour
{
    [SerializeField]
    public static int D10 = 10;
    [SerializeField]
    private short maxTime = 4;
    [SerializeField]
    private DiceInstantiator diceInstantiator;
    [SerializeField]
    private GameObject diceCanvas;

    private PlaceDeck placeDeckManager;
    private CombatPopupManager combatPopupManager;
    private AudioManager audioManager;
    private AudioRepo audioRepo;
    private List<int> dicesResults;
    private bool dicing = false;

    void Awake()
    {
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>(); 
        combatPopupManager = GameObject.Find("CombatPopupManager").GetComponent<CombatPopupManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
    }

    public IEnumerator RollToSpawnCard(SpawnCardLocation spawnCardLocation, CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(1));
        yield return new WaitUntil(() => !dicing);
        CheckResultRollToSpawnCard(spawnCardLocation, cardDetails);
    }
    public IEnumerator RollForEvents(CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(1));
        yield return new WaitUntil(() => !dicing);
        CheckResultToPlayEvent(cardDetails);
    }
    public IEnumerator RollForFactions(CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(1));
        yield return new WaitUntil(() => !dicing);
        CheckResultToInfluenceFaction(cardDetails);
    }
    public IEnumerator RollForAllies(CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(1));
        yield return new WaitUntil(() => !dicing);
        CheckResultToRecruitAlly(cardDetails);
    }
    public IEnumerator RollForGoldRing(CardDetails cardDetails)
    {
        yield return StartCoroutine(RollDice(1));
        yield return new WaitUntil(() => !dicing);
        CheckResultToAnalyzeGoldRing(cardDetails);
    }
    public IEnumerator RollForCombatCoroutine(int attackers)
    {
        yield return StartCoroutine(RollDice(attackers));
        yield return new WaitUntil(() => !dicing);
        CheckResultRollToCombat();
    }

    public void RollForCombat(int attackers)
    {
        StartCoroutine(RollForCombatCoroutine(attackers));
    }

    public IEnumerator RollDice(int numOfDices)
    {
        dicing = true;
        diceCanvas.SetActive(true);

        diceInstantiator.Initialize(numOfDices);

        audioManager.PlaySound(audioRepo.GetAudio("dices"));
        yield return new WaitUntil(()=>!diceInstantiator.IsDicing());
        dicesResults = diceInstantiator.GetDicesResults();
        diceCanvas.SetActive(false);
        dicing = false;
    }

    public void CheckResultRollToSpawnCard(SpawnCardLocation spawnCardLocation, CardDetails card)
    {
        placeDeckManager.GatherDiceResultsForSpawningCards(dicesResults[0], spawnCardLocation, card);
    }

    public void CheckResultToPlayEvent(CardDetails card)
    {
        placeDeckManager.GatherDiceResultsForPlayingEvents(dicesResults[0], card);
    }   
    public void CheckResultToInfluenceFaction(CardDetails card)
    {
        placeDeckManager.GatherDiceResultsForInfluencingFactions(dicesResults[0], card);
    }
    public void CheckResultToRecruitAlly(CardDetails card)
    {
        placeDeckManager.GatherDiceResultsForRecruitingAllies(dicesResults[0], card);
    }
    public void CheckResultToAnalyzeGoldRing(CardDetails card)
    {
        placeDeckManager.GatherDiceResultsForAnalyzingGoldRing(dicesResults[0], card);
    }
    public void CheckResultRollToCombat()
    {
        combatPopupManager.GatherDiceResults(dicesResults);
    }
    public bool IsDicing()
    {
        return dicing;
    }
}

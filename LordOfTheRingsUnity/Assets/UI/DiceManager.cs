using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
    private float showDiceResultsTime = 3f;
    [SerializeField]
    private DiceInstantiator diceInstantiator;
    [SerializeField]
    private GameObject diceCanvas;
    
    public static int D10 = 10;
    private PlaceDeck placeDeckManager;
    private CombatPopupManager combatPopupManager;
    private AudioManager audioManager;
    private AudioRepo audioRepo;
    private List<int> dicesResults;
    private bool dicing;

    void Awake()
    {
        placeDeckManager = GameObject.Find("PlaceDeckManager").GetComponent<PlaceDeck>(); 
        combatPopupManager = GameObject.Find("CombatPopupManager").GetComponent<CombatPopupManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
        dicing = false;
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

    public IEnumerator RollDice(int numOfDices)
    {
        dicing = true;
        diceCanvas.SetActive(true);

        bool results = false;
        int tries = 2;
        while(!results)
        {
            diceInstantiator.Initialize(numOfDices);

            audioManager.PlaySound(audioRepo.GetAudio("dices"));
            yield return new WaitUntil(() => !diceInstantiator.IsDicing());
            yield return new WaitForSecondsRealtime(showDiceResultsTime);
            dicesResults = diceInstantiator.GetDicesResults();
            results = (dicesResults.Count > 0);
            if (!results)
            {
                tries--;
                if(tries < 0)
                {
                    dicesResults = new List<int>();
                    for(int i = 0; i < numOfDices; i++)
                        dicesResults.Add(Random.Range(1, D10));
                }
            }
        }
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

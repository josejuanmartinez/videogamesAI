#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

public class PrefabLoader : MonoBehaviour
{
    // The folder path where your prefabs are stored
    public string folderPath = "Assets/_GameObjects/_Cards/Decks/";
    public bool refresh = false;

    private GameObject cardDetailsRepo;
    private bool initialized = false;

    void Awake()
    {
        cardDetailsRepo = GameObject.Find("CardDetailsRepo");
        initialized = false;
        Initialize(folderPath);        
    }

    void Initialize(string path)
    {
        if(refresh)
        {
            foreach (NationsEnum candidate in Enum.GetValues(typeof(NationsEnum)))
            {
                Transform t = cardDetailsRepo.transform.Find("cards_" + candidate.ToString().ToLower());
                if(t.GetComponent<InitialDeck>() != null)
                    t.GetComponent<InitialDeck>().cards = new();
            }
                
            string[] files = AssetDatabase.FindAssets("t:prefab", new string[] { folderPath });

            foreach (string file in files)
            {
                // Load prefab
                string guidPath = AssetDatabase.GUIDToAssetPath(file);
                string player = guidPath.Replace(path, "").Split('/')[0];
                foreach (NationsEnum candidate in Enum.GetValues(typeof(NationsEnum)))
                {
                    if (candidate.ToString().ToLower() == player.ToLower())
                    {
                        if (cardDetailsRepo.transform.Find("cards_" + player.ToLower()) != null)
                        {
                            GameObject cardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(guidPath);
                            CardDetails cardDetails = cardPrefab.GetComponent<CardDetails>();
                            switch (cardDetails.cardClass)
                            {
                                case CardClass.Place:
                                    break;
                                case CardClass.Character:
                                    (cardDetails as CharacterCardDetails).Initialize();
                                    break;
                                case CardClass.Object:
                                    (cardDetails as ObjectCardDetails).Initialize();
                                    break;
                                case CardClass.Faction:
                                    (cardDetails as FactionCardDetails).Initialize();
                                    break;
                                case CardClass.Event:
                                    (cardDetails as EventCardDetails).Initialize();
                                    break;
                                case CardClass.HazardEvent:
                                    (cardDetails as HazardEventCardDetails).Initialize();
                                    break;
                                case CardClass.HazardCreature:
                                    (cardDetails as HazardCreatureCardDetails).Initialize();
                                    break;
                                case CardClass.Ally:
                                    (cardDetails as AllyCardDetails).Initialize();
                                    break;
                                case CardClass.GoldRing:
                                    (cardDetails as GoldRingDetails).Initialize();
                                    break;
                                case CardClass.Ring:
                                    (cardDetails as RingCardDetails).Initialize();
                                    break;
                                case CardClass.NONE:
                                    break;
                            }
                            //PrefabUtility.SavePrefabAsset(cardPrefab);
                            cardDetailsRepo.transform.Find("cards_" + player.ToLower()).GetComponent<InitialDeck>().AddCard(cardPrefab);
                        }
                            
                    }
                        
                    
                }
                    
            
                
            }
            //Debug.Log("PrefabLoader finished loading from disk prefabs at " + Time.realtimeSinceStartup);
        }
        //else
            //Debug.Log("PrefabLoader skipped as refresh=`false` at " + Time.realtimeSinceStartup);
        
        initialized = true;        
    }

    public bool IsInitialized()
    {
        return initialized;
    }
}
#endif
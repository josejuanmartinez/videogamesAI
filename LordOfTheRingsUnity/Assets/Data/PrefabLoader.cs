#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System.IO;
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
            string[] files = AssetDatabase.FindAssets("t:prefab", new string[] { folderPath });

            foreach (string file in files)
            {
                // Load prefab
                string guidPath = AssetDatabase.GUIDToAssetPath(file);
                string player = guidPath.Replace(path, "").Split('/')[0];
                foreach (NationsEnum candidate in Enum.GetValues(typeof(NationsEnum)))
                    if (candidate.ToString().ToLower() == player.ToLower())
                        if (cardDetailsRepo.transform.Find(player.ToLower()) != null)
                            cardDetailsRepo.transform.Find(player.ToLower()).GetComponent<InitialDeck>().AddCard(AssetDatabase.LoadAssetAtPath<GameObject>(guidPath));
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
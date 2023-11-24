using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public static string CHARACTERS_SUFFIX = "_characters";

    public GameObject regionPrefab;
    public GameObject characterSelectionPrefab;
    public GameObject regionCharacterPrefab;
    public Transform regionTransform;
    public Transform characterSelectionTransform;

    private GalleryLevelSelectionManager regionGallery;
    private bool initialized;

    void Awake()
    {
        initialized = false;
        regionGallery = GameObject.Find("RegionSelection").GetComponent<GalleryLevelSelectionManager>();
    }

    void Initialize()
    {
        if (!GameObject.Find("Localization").GetComponent<Localization>().IsInitialized())
            return;

        List<GalleryLevelView> regionCards = new ();
        foreach(NationRegionsEnum region in Enum.GetValues(typeof(NationRegionsEnum)))
        {
            // I create a selection of chars for that region
            GameObject regionCharactersGalleryGameObject = Instantiate(characterSelectionPrefab, characterSelectionTransform);
            regionCharactersGalleryGameObject.name = string.Format("{0}_{1}", region.ToString(), CHARACTERS_SUFFIX);
            GalleryLevelSelectionManager regionCharactersGallery = regionCharactersGalleryGameObject.GetComponentInChildren<GalleryLevelSelectionManager>();

            // I create the region card prefab
            GameObject regionCard = Instantiate(regionPrefab, regionTransform);
            regionCard.name = region.ToString();
            regionCard.GetComponent<RegionCardUI>().Initialize(region, regionCharactersGallery);
            regionCards.Add(regionCard.GetComponent<GalleryLevelView>());
                   

            List<GalleryLevelView> regionCharactersCards = new();            
      
            List<NationsEnum> nationsOfRegion = Nations.regions.Where(x => x.Value.Contains(region)).Select(x => x.Key).ToList();
            foreach (NationsEnum nation in nationsOfRegion)
            {
                GameObject characterRegionCard = Instantiate(regionCharacterPrefab, regionCharactersGallery.itemsContainer);
                characterRegionCard.name = nation.ToString();
                characterRegionCard.GetComponent<RegionCharacterCardUI>().Initialize(regionCharactersGallery, nation);
                regionCharactersCards.Add(characterRegionCard.GetComponent<GalleryLevelView>());
            }

            regionCharactersGallery.items = regionCharactersCards.ToArray();
            regionCharactersGalleryGameObject.SetActive(false);
            //regionCharactersGallery.Start();
        }
        regionGallery.items = regionCards.ToArray();
        regionGallery.Start();
        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            Initialize();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

}

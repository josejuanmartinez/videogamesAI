using System.Collections.Generic;
using UnityEngine;

public class RegionCardUI : MonoBehaviour
{
    protected SpritesRepo spritesRepo;

    private GalleryLevelSelectionManager gallery;
    private RegionSelectionBehaviour selectionBehaviour;    
    protected bool isAwaken = false;

    protected virtual void Awake()
    {
        gallery = GameObject.Find("RegionSelection").GetComponent<GalleryLevelSelectionManager>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        selectionBehaviour = GetComponentInChildren<RegionSelectionBehaviour>();
        isAwaken = true;
    }
    public void Initialize(NationRegionsEnum region, GalleryLevelSelectionManager gallery)
    {
        if (!isAwaken)
            Awake();
        int index = spritesRepo.nationRegions.IndexOf(region);
        if (index == -1)
        {
            DestroyImmediate(gameObject);
            return;
        }

        GalleryLevelView galleryView = GetComponent<GalleryLevelView>();
        galleryView.image.sprite = spritesRepo.nationRegionsSprites[index];
        galleryView.text.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(region.ToString());
        galleryView.levelName = "region";
        galleryView.manager = gallery;
        List<GalleryLevelView> listOfRegions = new (galleryView.manager.items)
        {
            galleryView
        };
        galleryView.manager.items = listOfRegions.ToArray();
        selectionBehaviour.Initialize(gallery);
    }
}

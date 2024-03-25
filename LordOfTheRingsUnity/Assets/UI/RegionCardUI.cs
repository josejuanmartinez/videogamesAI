using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RegionCardUI : MonoBehaviour, IPointerEnterHandler
{
    protected SpritesRepo spritesRepo;

    private RegionSelectionBehaviour selectionBehaviour;
    protected AudioManager audioManager;
    protected AudioRepo audioRepo;
    protected bool isAwaken = false;

    protected virtual void Awake()
    {
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        selectionBehaviour = GetComponentInChildren<RegionSelectionBehaviour>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioManager.PlaySound(audioRepo.GetAudio("card"));
    }
}

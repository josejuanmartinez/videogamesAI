using System.Collections.Generic;
using UnityEngine;

public class RegionCharacterCardUI : RegionCardUI
{
    public void Initialize(GalleryLevelSelectionManager gallery, NationsEnum nation)
    {
        if (!isAwaken)
            Awake();

        int index = spritesRepo.avatars.IndexOf(nation);
        if (index == -1)
        {
            DestroyImmediate(gameObject);
            return;
        }
            
        GalleryLevelView galleryView = GetComponent<GalleryLevelView>();
        galleryView.image.sprite = spritesRepo.avatarsSprites[index];
        galleryView.text.text = GameObject.Find("Localization").GetComponent<Localization>().Localize(nation.ToString());
        galleryView.levelName = "character";
        galleryView.manager = gallery;
        List<GalleryLevelView> listOfCharacters = new(galleryView.manager.items)
        {
            galleryView
        };
        galleryView.manager.items = listOfCharacters.ToArray();

        GetComponentInChildren<RegionCharacterSelectionBehaviour>().Initialize(nation);
    }
}

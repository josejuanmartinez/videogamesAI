using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpritesRepo : MonoBehaviour
{
    public List<Sprite> sprites;
    public List<string> spriteStrings;

    public List<NationRegionsEnum> nationRegions;
    public List<Sprite> nationRegionsSprites;

    public List<NationsEnum> avatars;
    public List<Sprite> avatarsSprites;

    public TMP_SpriteAsset spriteAsset;

    void Awake()
    {
        Assert.AreEqual(sprites.Count, spriteStrings.Count);        
    }

    public Sprite GetSprite(string id)
    {
        Sprite res;

        int index = spriteStrings.IndexOf(id.ToLower());
        if (index != -1)
            res = sprites[index];
        else
        {
            Debug.Log(string.Format("{0} not found in sprites. Consider adding it.", id.ToLower()));
            res = sprites[spriteStrings.IndexOf("default")];
        }
            

        return res;
    }

    public bool ExistsSprite(string id)
    {
        bool found = spriteStrings.IndexOf(id.ToLower()) != -1;
        return found;
    }

    public bool ExistsSpriteInSpriteAsset(string spriteName)
    {
        if (spriteAsset.GetSpriteIndexFromName(spriteName) != -1)
            return true;
        return false;
    }

    public Sprite GetNationSprite(NationsEnum nation)
    {
        Sprite def = GetSprite("default");

        int index = avatars.IndexOf(nation);
        if (index != -1)
            def = avatarsSprites[index];

        return def;
    }
}

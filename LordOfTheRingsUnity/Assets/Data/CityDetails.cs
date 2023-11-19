using System;
using System.Collections.Generic;
using UnityEngine;

public class CityDetails: MonoBehaviour
{
    [Header("City Details")]
    [Header("Initialization")]
    [PreviewSprite]
    public Sprite sprite;
    public string descForAIGeneration;

    public CitySizesEnum size;

    public bool isHidden = false;
    public bool hasPort = false;
    public bool hasHoard = false;
    public bool isUnderground = false;

    public string cityId;
    public string regionId;
    public bool isHaven;

    /** AUTOMATICALLY GENERATED */
    [Header("Automatically Generated")]
    [SerializeField]
    private List<ObjectType> playableObjects;
    [SerializeField]
    private List<RingType> playableRings;
    [SerializeField]
    private TerrainsEnum terrain;
    [SerializeField]
    private CardTypesEnum cardType;
    private Resources production;

    public void Initialize(TerrainsEnum terrain, CardTypesEnum cardType)
    {
        this.terrain = terrain;
        this.cardType = cardType;
        GenerateProduction();
        GeneratePlayableObjects();
        GeneratePlayableRings();
    }

    public Resources GetCityProduction()
    {
        return production;
    }

    public bool IsHaven()
    {
        return isHaven;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    public void GenerateProduction()
    {
        System.Random rd = new();
        int food = rd.Next(TerrainBonuses.minBonuses[Terrains.foodBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.foodBonuses[terrain]]);
        int gold = rd.Next(TerrainBonuses.minBonuses[Terrains.goldBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.goldBonuses[terrain]]);
        int clothes = rd.Next(TerrainBonuses.minBonuses[Terrains.clothesBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.clothesBonuses[terrain]]);
        int wood = rd.Next(TerrainBonuses.minBonuses[Terrains.woodBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.woodBonuses[terrain]]);
        int metal = rd.Next(TerrainBonuses.minBonuses[Terrains.metalBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.metalBonuses[terrain]]);
        int gems = rd.Next(TerrainBonuses.minBonuses[Terrains.gemsBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.gemsBonuses[terrain]]);
        int horses = rd.Next(TerrainBonuses.minBonuses[Terrains.horsesBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.horsesBonuses[terrain]]);
        int leather = rd.Next(TerrainBonuses.minBonuses[Terrains.leatherBonuses[terrain]], TerrainBonuses.maxBonuses[Terrains.leatherBonuses[terrain]]);

        switch (size)
        {
            case CitySizesEnum.VERY_BIG:
                gold += (food + clothes + wood + metal + gems + horses + leather);
                break;
            case CitySizesEnum.BIG:
                metal += (int)Math.Round((decimal)(food + clothes + wood + gems + horses + leather) / 2);
                gems += (int)Math.Round((decimal)(food + clothes + wood + metal + horses + leather) / 2);
                break;
            case CitySizesEnum.MEDIUM:
                leather += (int)Math.Round((decimal)(food + clothes + wood + gems + horses + metal) / 2);
                clothes += (int)Math.Round((decimal)(food + metal + wood + gems + horses + leather) / 2);
                break;
            case CitySizesEnum.SMALL:
                wood += (int)Math.Round((decimal)(food + metal + clothes + gems + horses + leather) / 2);
                horses += (int)Math.Round((decimal)(food + metal + wood + gems + clothes + leather) / 2);
                break;
            case CitySizesEnum.VERY_SMALL:
                food += (metal + clothes + wood + gems + horses + leather);
                break;
        }

        production = new Resources(food, gold, clothes, wood, metal, horses, gems, leather);
    }

    public void GeneratePlayableRings()
    {
        playableRings = new();
        foreach (RingType ringType in Enum.GetValues(typeof(RingType)))
        {
            switch (ringType)
            {
                case RingType.MindRing:
                    if (cardType == CardTypesEnum.LAIR || cardType == CardTypesEnum.DARK_BASTION)
                        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                            playableRings.Add(ringType);
                    break;
                case RingType.DwarvenRing:
                    if (terrain == TerrainsEnum.MOUNTAIN || terrain == TerrainsEnum.OTHER_HILLS_MOUNTAIN || terrain == TerrainsEnum.SNOWHILLS)
                        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                            playableRings.Add(ringType);
                    break;
                case RingType.MagicRing:
                    if (cardType == CardTypesEnum.WILDERNESS || cardType == CardTypesEnum.FREE_BASTION)
                        if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                            playableRings.Add(ringType);
                    break;
                case RingType.LesserRing:
                    if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                        playableRings.Add(ringType);
                    break;
                case RingType.TheOneRing:
                    if (cardType == CardTypesEnum.LAIR || terrain == TerrainsEnum.SWAMP)
                    {
                        if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                            playableRings.Add(ringType);
                    }
                    break;
            }
        }
    }

    public void GeneratePlayableObjects()
    {
        playableObjects = new();
        
        if (terrain == TerrainsEnum.SWAMP || 
            terrain == TerrainsEnum.MOUNTAIN ||
            terrain == TerrainsEnum.OTHER_HILLS_MOUNTAIN ||
            terrain == TerrainsEnum.SNOWHILLS ||
            terrain == TerrainsEnum.ICE ||
            terrain == TerrainsEnum.COAST ||
            terrain == TerrainsEnum.SEA
           )
        {

            if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                playableObjects.Add(ObjectType.Palantir);

            if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                playableObjects.Add(ObjectType.Jewelry);
        }

        switch (size)
        {
            case CitySizesEnum.VERY_BIG:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Jewelry);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.OtherHand);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.MainHand);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Armor);
                break;
            case CitySizesEnum.BIG:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Armor);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.MainHand);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.OtherHand);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Head);
                break;
            case CitySizesEnum.MEDIUM:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Head);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Gloves);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Boots);
                break;
            case CitySizesEnum.SMALL:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Gloves);
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Boots);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Cloak);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                    playableObjects.Add(ObjectType.Belt);
                break;
            case CitySizesEnum.VERY_SMALL:
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Cloak);
                if (UnityEngine.Random.Range(0f, 1f) > 0.75f)
                    playableObjects.Add(ObjectType.Belt);
                playableObjects.Add(ObjectType.Consumable);
                playableObjects.Add(ObjectType.Mount);
                break;
        }
    }

    public List<ObjectType> GetPlayableObjects()
    {
        return playableObjects;
    }

    public List<RingType> GetPlayableRings()
    {
        return playableRings;
    }
}
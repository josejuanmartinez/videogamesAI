using System.Collections.Generic;

public enum TerrainsEnum
{
    SEA,
    COAST,
    WASTE,
    PLAINS,
    GRASS,
    ASHES,
    MOUNTAIN,
    HILLS,
    SNOWHILLS,
    ICE,
    DESERT,
    OTHER_HILLS_MOUNTAIN,
    FOREST,
    SWAMP
}

public static class Terrains
{
    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> goldBonuses = new()
    {
        { TerrainsEnum.SEA, TerrainBonusesEnum.LOW },
        { TerrainsEnum.COAST, TerrainBonusesEnum.MID },
        { TerrainsEnum.WASTE, TerrainBonusesEnum.MID },
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.MID },
        { TerrainsEnum.GRASS, TerrainBonusesEnum.MID },
        { TerrainsEnum.ASHES, TerrainBonusesEnum.HIGH },
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.HIGH },
        { TerrainsEnum.HILLS, TerrainBonusesEnum.MID },
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.VERY_HIGH },
        { TerrainsEnum.ICE, TerrainBonusesEnum.LOW },
        { TerrainsEnum.DESERT, TerrainBonusesEnum.MID },
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.HIGH },
        { TerrainsEnum.FOREST, TerrainBonusesEnum.LOW },
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.LOW },
    };

    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> clothesBonuses = new()
    {
        { TerrainsEnum.SEA, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.COAST, TerrainBonusesEnum.VERY_LOW },
        { TerrainsEnum.WASTE, TerrainBonusesEnum.MID },
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.VERY_HIGH },
        { TerrainsEnum.GRASS, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.ASHES, TerrainBonusesEnum.VERY_LOW },
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.LOW },
        { TerrainsEnum.HILLS, TerrainBonusesEnum.MID },
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.LOW },
        { TerrainsEnum.ICE, TerrainBonusesEnum.VERY_LOW },
        { TerrainsEnum.DESERT, TerrainBonusesEnum.LOW },
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.MID },
        { TerrainsEnum.FOREST, TerrainBonusesEnum.HIGH },
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.MID},
    };

    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> foodBonuses = new()
    {
                                                             // GOLD PROD FOOD
        { TerrainsEnum.SEA, TerrainBonusesEnum.HIGH},   // LOW MID HIGH (9)
        { TerrainsEnum.COAST, TerrainBonusesEnum.MID }, // MID MID MID (9)
        { TerrainsEnum.WASTE, TerrainBonusesEnum.LOW }, // MID HIGH LOW (9)
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.MID }, // MID MID MID (9)
        { TerrainsEnum.GRASS, TerrainBonusesEnum.HIGH}, // MID LOW HIGH (9)
        { TerrainsEnum.ASHES, TerrainBonusesEnum.VERY_LOW }, // LOW VERY_HIGH LOW (9)
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.VERY_LOW }, // HIGH HIGH VERY_LOW (9)
        { TerrainsEnum.HILLS, TerrainBonusesEnum.LOW }, //MID HIGH LOW (9)
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.VERY_LOW }, //VERY_HIGH MID VERY_LOW (9)
        { TerrainsEnum.ICE, TerrainBonusesEnum.MID }, // LOW HIGH MID (9)
        { TerrainsEnum.DESERT, TerrainBonusesEnum.VERY_LOW }, // MID VERY_HIGH VERY_LOW (9)
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.VERY_LOW }, // HIGH HIGH VERY_LOW (9)
        { TerrainsEnum.FOREST, TerrainBonusesEnum.HIGH}, // LOW MID HIGH (9)
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.VERY_HIGH } // LOW LOW VERY_HIGH (9)
    };

    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> woodBonuses = new()
    {
        { TerrainsEnum.SEA, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.COAST, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.WASTE, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.GRASS, TerrainBonusesEnum.LOW},
        { TerrainsEnum.ASHES, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.MID},
        { TerrainsEnum.HILLS, TerrainBonusesEnum.LOW},
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.MID},
        { TerrainsEnum.ICE, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.DESERT, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.FOREST, TerrainBonusesEnum.VERY_HIGH},
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.HIGH},
    };

    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> metalBonuses = new()
    {
        { TerrainsEnum.SEA, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.COAST, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.WASTE, TerrainBonusesEnum.LOW},
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.GRASS, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.ASHES, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.VERY_HIGH},
        { TerrainsEnum.HILLS, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.ICE, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.DESERT, TerrainBonusesEnum.LOW},
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.FOREST, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.VERY_LOW},
    };

    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> horsesBonuses = new()
    {
        { TerrainsEnum.SEA, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.COAST, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.WASTE, TerrainBonusesEnum.LOW},
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.GRASS, TerrainBonusesEnum.VERY_HIGH},
        { TerrainsEnum.ASHES, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.LOW},
        { TerrainsEnum.HILLS, TerrainBonusesEnum.MID},
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.ICE, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.DESERT, TerrainBonusesEnum.LOW},
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.FOREST, TerrainBonusesEnum.MID},
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.VERY_LOW},
    };

    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> gemsBonuses = new()
    {
        { TerrainsEnum.SEA, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.COAST, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.WASTE, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.GRASS, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.ASHES, TerrainBonusesEnum.MID},
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.VERY_HIGH},
        { TerrainsEnum.HILLS, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.ICE, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.DESERT, TerrainBonusesEnum.MID},
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.FOREST, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.VERY_LOW},
    };

    public static Dictionary<TerrainsEnum, TerrainBonusesEnum> leatherBonuses = new()
    {
        { TerrainsEnum.SEA, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.COAST, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.WASTE, TerrainBonusesEnum.VERY_HIGH},
        { TerrainsEnum.PLAINS, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.GRASS, TerrainBonusesEnum.MID},
        { TerrainsEnum.ASHES, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.MOUNTAIN, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.HILLS, TerrainBonusesEnum.LOW},
        { TerrainsEnum.SNOWHILLS, TerrainBonusesEnum.MID},
        { TerrainsEnum.ICE, TerrainBonusesEnum.VERY_LOW},
        { TerrainsEnum.DESERT, TerrainBonusesEnum.HIGH},
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, TerrainBonusesEnum.LOW},
        { TerrainsEnum.FOREST, TerrainBonusesEnum.MID},
        { TerrainsEnum.SWAMP, TerrainBonusesEnum.MID},
    };

    public static Dictionary<TerrainsEnum, short> movementCost = new()
    {
        { TerrainsEnum.SEA, 2},
        { TerrainsEnum.COAST, 1},
        { TerrainsEnum.WASTE, 1},
        { TerrainsEnum.PLAINS, 1},
        { TerrainsEnum.GRASS, 1},
        { TerrainsEnum.ASHES, 2},
        { TerrainsEnum.MOUNTAIN, 3},
        { TerrainsEnum.HILLS, 2},
        { TerrainsEnum.SNOWHILLS, 3},
        { TerrainsEnum.ICE, 2},
        { TerrainsEnum.DESERT, 2},
        { TerrainsEnum.OTHER_HILLS_MOUNTAIN, 3},
        { TerrainsEnum.FOREST, 2},
        { TerrainsEnum.SWAMP, 3},
    };

}
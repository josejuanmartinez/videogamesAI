using System.Collections.Generic;

public enum TerrainBonusesEnum {
    VERY_HIGH,
    HIGH,
    MID,
    LOW,
    VERY_LOW
}


public static class TerrainBonuses
{
    public static Dictionary<TerrainBonusesEnum, short> maxBonuses = new() {
        { TerrainBonusesEnum.VERY_HIGH, 4 },
        { TerrainBonusesEnum.HIGH, 3 },
        { TerrainBonusesEnum.MID, 2 },
        { TerrainBonusesEnum.LOW, 1 },
        { TerrainBonusesEnum.VERY_LOW, 0 },
    };

    public static Dictionary<TerrainBonusesEnum, short> minBonuses = new() {
        { TerrainBonusesEnum.VERY_HIGH, 3 },
        { TerrainBonusesEnum.HIGH, 2 },
        { TerrainBonusesEnum.MID, 1 },
        { TerrainBonusesEnum.LOW, 0 },
        { TerrainBonusesEnum.VERY_LOW, 0 },
    };

}

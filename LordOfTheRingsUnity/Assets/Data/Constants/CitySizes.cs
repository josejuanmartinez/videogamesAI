using System.Collections.Generic;

public enum CitySizesEnum {
    VERY_BIG,
    BIG,
    MEDIUM,
    SMALL,
    VERY_SMALL
}

public static class CitySizes
{
    public static Dictionary<CitySizesEnum, int> automaticAttacks = new()
    {
        { CitySizesEnum.VERY_BIG, 5},
        { CitySizesEnum.BIG, 4},
        { CitySizesEnum.MEDIUM, 3},
        { CitySizesEnum.SMALL, 2},
        { CitySizesEnum.VERY_SMALL, 1},
    };
}

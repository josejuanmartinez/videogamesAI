using System.Collections.Generic;

public enum NationsEnum
{
    ABANDONED,
    UVATHA,
    KHAMUL,
    ADUNAPHEL,
    BEORN,
    BARD,
    THRANDUIL,
    HUZ,
    MOUTH,
    SARUMAN,
    THEODEN,
    THRAIN,
    WITCH_KING,
    RADAGAST,
    GALADRIEL,
    BALROG,
    SMAUG,
    ALATAR,
    PALLANDO,
    BROCCACH,
    ELROND,
    HOARMURATH,
    DENDRA_DWAR,
    DENETHOR,
    REN
}

public enum NationRegionsEnum
{
    SEA_OF_RHUN,
    SOUTHERN_MIRKWOOD,
    CHELKAR,
    ANDUIN_VALES,
    NORTHERN_RHOVANION,
    WOODLAND_REALM,
    HORSE_PLAINS,
    GORGOROTH,
    GAP_OF_ISEN,
    ROHAN,
    IRON_HILLS,
    ANGMAR,
    WESTERN_MIRKWOOD,
    WOLD_AND_FOOTHILLS,
    REDHORN_GATE,
    WITHERED_HEATH,
    RHUDAUR,
    UDUN,
    ITHILIEN,
    GONDOR,
    UNGOL
}

public enum AlignmentsEnum
{
    FREE_PEOPLE,
    DARK_SERVANTS,
    RENEGADE,
    NEUTRAL,
    CHAOTIC,
    NONE
}

public static class Nations
{
    public static Dictionary<NationsEnum, AlignmentsEnum> alignments = new ()
    {
        { NationsEnum.ABANDONED, AlignmentsEnum.NONE },
        { NationsEnum.UVATHA, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.KHAMUL, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.ADUNAPHEL, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.BEORN , AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.BARD , AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.THRANDUIL, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.HUZ, AlignmentsEnum.NEUTRAL },
        { NationsEnum.MOUTH, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.SARUMAN, AlignmentsEnum.RENEGADE },
        { NationsEnum.THEODEN, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.THRAIN, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.WITCH_KING, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.RADAGAST, AlignmentsEnum.RENEGADE },
        { NationsEnum.GALADRIEL, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.BALROG, AlignmentsEnum.CHAOTIC },
        { NationsEnum.SMAUG, AlignmentsEnum.CHAOTIC },
        { NationsEnum.ALATAR, AlignmentsEnum.RENEGADE },
        { NationsEnum.PALLANDO, AlignmentsEnum.RENEGADE },
        { NationsEnum.BROCCACH, AlignmentsEnum.NEUTRAL },
        { NationsEnum.ELROND, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.HOARMURATH, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.DENDRA_DWAR, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.DENETHOR, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.REN, AlignmentsEnum.DARK_SERVANTS },
    };

    public static Dictionary<NationsEnum, NationRegionsEnum> regions = new ()
    {
        { NationsEnum.UVATHA, NationRegionsEnum.SEA_OF_RHUN },
        { NationsEnum.KHAMUL, NationRegionsEnum.SOUTHERN_MIRKWOOD },
        { NationsEnum.ADUNAPHEL, NationRegionsEnum.CHELKAR },
        { NationsEnum.BEORN , NationRegionsEnum.ANDUIN_VALES},
        { NationsEnum.BARD , NationRegionsEnum.NORTHERN_RHOVANION },
        { NationsEnum.THRANDUIL, NationRegionsEnum.WOODLAND_REALM },
        { NationsEnum.HUZ, NationRegionsEnum.HORSE_PLAINS },
        { NationsEnum.MOUTH, NationRegionsEnum.GORGOROTH },
        { NationsEnum.SARUMAN, NationRegionsEnum.GAP_OF_ISEN },
        { NationsEnum.THEODEN, NationRegionsEnum.ROHAN },
        { NationsEnum.THRAIN, NationRegionsEnum.IRON_HILLS },
        { NationsEnum.WITCH_KING, NationRegionsEnum.ANGMAR },
        { NationsEnum.RADAGAST, NationRegionsEnum.WESTERN_MIRKWOOD },
        { NationsEnum.GALADRIEL, NationRegionsEnum.WOLD_AND_FOOTHILLS },
        { NationsEnum.BALROG, NationRegionsEnum.REDHORN_GATE },
        { NationsEnum.SMAUG, NationRegionsEnum.WITHERED_HEATH },
        { NationsEnum.ALATAR, NationRegionsEnum.SEA_OF_RHUN },
        { NationsEnum.PALLANDO, NationRegionsEnum.CHELKAR },
        { NationsEnum.BROCCACH, NationRegionsEnum.RHUDAUR },
        { NationsEnum.ELROND, NationRegionsEnum.RHUDAUR },
        { NationsEnum.HOARMURATH, NationRegionsEnum.ITHILIEN },
        { NationsEnum.DENDRA_DWAR, NationRegionsEnum.UDUN },
        { NationsEnum.DENETHOR, NationRegionsEnum.GONDOR },
        { NationsEnum.REN, NationRegionsEnum.UNGOL },
    };

    public static short INFLUENCE = 20;

}

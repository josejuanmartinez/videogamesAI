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
    RHUNEN,
    SOUTHERN_MIRKWOOD,
    ANDUIN_RIVER,
    NORTHERN_MIRKWOOD,
    NEAR_HARAD,
    KHAND,
    GORGOROTH,
    GAP_OF_ISEN,
    ROHAN,
    IRON_HILLS,
    ANGMAR,
    RHUN,
    LOTHLORIEN,
    DORWINION,
    WITHERED_HEATH,
    RHUDAUR,
    UDUN,
    ITHILIEN,
    GONDOR,
    UNGOL,
    MORDOR,
    MISTY_MOUNTAINS,
    DUNLAND,
    RHOVANION,
    BROWN_LANDS,
    DEAD_MARSHES
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

    public static Dictionary<NationsEnum, List<NationRegionsEnum>> regions = new ()
    {
        { NationsEnum.UVATHA, new()
            {
            NationRegionsEnum.RHUNEN,
            NationRegionsEnum.KHAND,
            NationRegionsEnum.BROWN_LANDS,
            }
        },
        { NationsEnum.KHAMUL, new()
            {
            NationRegionsEnum.SOUTHERN_MIRKWOOD,
            NationRegionsEnum.BROWN_LANDS,
            NationRegionsEnum.MISTY_MOUNTAINS,
            NationRegionsEnum.NORTHERN_MIRKWOOD,
            }
        },
        { NationsEnum.ADUNAPHEL, new()
            {
            NationRegionsEnum.NEAR_HARAD
            }
        },
        { NationsEnum.BEORN , new()
            {
            NationRegionsEnum.ANDUIN_RIVER
            }
        },
        { NationsEnum.BARD , new()
            {
            NationRegionsEnum.RHOVANION,
            NationRegionsEnum.DORWINION,
            NationRegionsEnum.RHUNEN
            }
        },
        { NationsEnum.THRANDUIL, new()
           {
            NationRegionsEnum.NORTHERN_MIRKWOOD
           }
        },
        { NationsEnum.HUZ, new()
            {
                NationRegionsEnum.RHUN,
                NationRegionsEnum.RHUNEN,
                NationRegionsEnum.DORWINION,
            }
        },
        { NationsEnum.MOUTH, new()
            {
                NationRegionsEnum.GORGOROTH
            }
        },
        { NationsEnum.SARUMAN, new()
            {
                NationRegionsEnum.GAP_OF_ISEN,
                NationRegionsEnum.DUNLAND,
                NationRegionsEnum.ROHAN
            }
        },
        { NationsEnum.THEODEN, new()
            {
                NationRegionsEnum.ROHAN
            }
        },
        { NationsEnum.THRAIN, new()
            {
                NationRegionsEnum.IRON_HILLS,
                NationRegionsEnum.RHUNEN
            }
        },
        { NationsEnum.WITCH_KING, new()
            {
                NationRegionsEnum.ANGMAR,
                NationRegionsEnum.UNGOL
            }
        },
        { NationsEnum.RADAGAST, new()
            {
                NationRegionsEnum.ANDUIN_RIVER
            }
        },
        { NationsEnum.GALADRIEL, new()
            {
                NationRegionsEnum.LOTHLORIEN,
                NationRegionsEnum.GONDOR
            }
        },
        { NationsEnum.BALROG, new()
            {
                NationRegionsEnum.MISTY_MOUNTAINS
            }
        },
        { NationsEnum.SMAUG, new()
            {
                NationRegionsEnum.RHOVANION
            }
        },
        { NationsEnum.ALATAR, new()
            {
                NationRegionsEnum.DORWINION
            }
        },
        { NationsEnum.PALLANDO, new()
            {
                NationRegionsEnum.KHAND
            }
        },
        { NationsEnum.BROCCACH, new()
            {
                NationRegionsEnum.RHUDAUR
            }
        },
        { NationsEnum.ELROND, new()
            {
                NationRegionsEnum.RHUDAUR
            }
        },
        { NationsEnum.HOARMURATH, new()
            {
                NationRegionsEnum.ITHILIEN,
                NationRegionsEnum.UDUN
            }
        },
        { NationsEnum.DENDRA_DWAR, new()
            {
                NationRegionsEnum.UDUN,
                NationRegionsEnum.DEAD_MARSHES
            }
        },
        { NationsEnum.DENETHOR, new()
            {
                NationRegionsEnum.GONDOR,
                NationRegionsEnum.ITHILIEN
            }
        },
        { NationsEnum.REN, new()
            {
                NationRegionsEnum.UNGOL,
                NationRegionsEnum.GORGOROTH
            }
        },
    };

    public static short INFLUENCE = 20;

}

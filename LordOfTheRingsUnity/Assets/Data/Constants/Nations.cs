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
    REN,
    STRIDER,
    ENION,
    CIRDAN,
    OVATHA_II,
    HARUTH_RAMAM,
    ANGAMAITE,
    PALADIN,
    JI_INDUR,
    AKHORAHIL,
    GANDALF
}

public enum NationRegionsEnum
{
    RHUNEN,
    SOUTHERN_MIRKWOOD,
    ANDUIN_RIVER,
    NORTHERN_MIRKWOOD,
    HARAD,
    KHAND,
    GORGOROTH,
    ROHAN,
    IRON_HILLS,
    ANGMAR,
    RHUN,
    LOTHLORIEN,
    WITHERED_HEATH,
    RHUDAUR,
    UDUN_AND_DAGORLAD,
    ITHILIEN,
    GONDOR,
    UNGOL,
    MORDOR,
    MISTY_MOUNTAINS,
    DUNLAND_AND_ENEDWAITH,
    RHOVANION,
    BROWN_LANDS,
    ERIADOR,
    LINDON,
    BLUE_MOUNTAINS,
    UMBAR,
    EREGION
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
        { NationsEnum.STRIDER, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.ANGAMAITE, AlignmentsEnum.NEUTRAL },
        { NationsEnum.HARUTH_RAMAM, AlignmentsEnum.NEUTRAL },
        { NationsEnum.ENION, AlignmentsEnum.NEUTRAL },
        { NationsEnum.OVATHA_II, AlignmentsEnum.NEUTRAL },
        { NationsEnum.CIRDAN, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.PALADIN, AlignmentsEnum.FREE_PEOPLE },
        { NationsEnum.JI_INDUR, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.AKHORAHIL, AlignmentsEnum.DARK_SERVANTS },
        { NationsEnum.GANDALF, AlignmentsEnum.FREE_PEOPLE },
    };

    public static Dictionary<NationsEnum, List<NationRegionsEnum>> regions = new()
    {
        { NationsEnum.UVATHA, new()
            {
            NationRegionsEnum.RHUNEN,
            NationRegionsEnum.KHAND,
            NationRegionsEnum.BROWN_LANDS,
            NationRegionsEnum.MORDOR,
            }
        },
        { NationsEnum.KHAMUL, new()
            {
            NationRegionsEnum.SOUTHERN_MIRKWOOD,
            NationRegionsEnum.BROWN_LANDS,
            NationRegionsEnum.MISTY_MOUNTAINS,
            NationRegionsEnum.NORTHERN_MIRKWOOD,
            NationRegionsEnum.MORDOR,
            }
        },
        { NationsEnum.ADUNAPHEL, new()
            {
            NationRegionsEnum.HARAD,
            NationRegionsEnum.KHAND,
            NationRegionsEnum.UMBAR,
            }
        },
        { NationsEnum.BEORN , new()
            {
            NationRegionsEnum.ANDUIN_RIVER,
            NationRegionsEnum.MISTY_MOUNTAINS,
            NationRegionsEnum.NORTHERN_MIRKWOOD,
            NationRegionsEnum.SOUTHERN_MIRKWOOD
            }
        },
        { NationsEnum.BARD , new()
            {
            NationRegionsEnum.RHOVANION,
            NationRegionsEnum.RHUNEN,
            }
        },
        { NationsEnum.THRANDUIL, new()
           {
            NationRegionsEnum.NORTHERN_MIRKWOOD,
            NationRegionsEnum.SOUTHERN_MIRKWOOD,
            NationRegionsEnum.RHUNEN,
           }
        },
        { NationsEnum.HUZ, new()
            {
                NationRegionsEnum.RHUN,
                NationRegionsEnum.RHUNEN,
                NationRegionsEnum.BROWN_LANDS,
                NationRegionsEnum.UDUN_AND_DAGORLAD,
                NationRegionsEnum.RHOVANION,
                NationRegionsEnum.IRON_HILLS,
            }
        },
        { NationsEnum.MOUTH, new()
            {
                NationRegionsEnum.GORGOROTH,
                NationRegionsEnum.MORDOR,
                NationRegionsEnum.UNGOL,
                NationRegionsEnum.UDUN_AND_DAGORLAD,
            }
        },
        { NationsEnum.SARUMAN, new()
            {
                NationRegionsEnum.DUNLAND_AND_ENEDWAITH,
                NationRegionsEnum.ROHAN,
                NationRegionsEnum.EREGION,
            }
        },
        { NationsEnum.THEODEN, new()
            {
                NationRegionsEnum.ROHAN,
                NationRegionsEnum.DUNLAND_AND_ENEDWAITH,
                NationRegionsEnum.BROWN_LANDS,
            }
        },
        { NationsEnum.THRAIN, new()
            {
                NationRegionsEnum.IRON_HILLS,
                NationRegionsEnum.BLUE_MOUNTAINS,
            }
        },
        { NationsEnum.WITCH_KING, new()
            {
                NationRegionsEnum.ANGMAR,
                NationRegionsEnum.UNGOL,
                NationRegionsEnum.MORDOR,
                NationRegionsEnum.RHUDAUR
            }
        },
        { NationsEnum.RADAGAST, new()
            {
                NationRegionsEnum.ANDUIN_RIVER,
                NationRegionsEnum.NORTHERN_MIRKWOOD,
                NationRegionsEnum.SOUTHERN_MIRKWOOD,
            }
        },
        { NationsEnum.GALADRIEL, new()
            {
                NationRegionsEnum.LOTHLORIEN,
                NationRegionsEnum.GONDOR,
                NationRegionsEnum.EREGION,
            }
        },
        { NationsEnum.BALROG, new()
            {
                NationRegionsEnum.MISTY_MOUNTAINS,
                NationRegionsEnum.EREGION,
            }
        },
        { NationsEnum.SMAUG, new()
            {
                NationRegionsEnum.RHOVANION,
                NationRegionsEnum.WITHERED_HEATH,
                NationRegionsEnum.NORTHERN_MIRKWOOD,
            }
        },
        { NationsEnum.ALATAR, new()
            {
                NationRegionsEnum.RHUN
            }
        },
        { NationsEnum.PALLANDO, new()
            {
                NationRegionsEnum.KHAND,
            }
        },
        { NationsEnum.BROCCACH, new()
            {
                NationRegionsEnum.RHUDAUR,
                NationRegionsEnum.ANGMAR,
                NationRegionsEnum.MISTY_MOUNTAINS,
            }
        },
        { NationsEnum.ELROND, new()
            {
                NationRegionsEnum.RHUDAUR,
                NationRegionsEnum.MISTY_MOUNTAINS,
                NationRegionsEnum.EREGION,
                NationRegionsEnum.ERIADOR,
            }
        },
        { NationsEnum.HOARMURATH, new()
            {
                NationRegionsEnum.ITHILIEN,
                NationRegionsEnum.UDUN_AND_DAGORLAD,
                NationRegionsEnum.GORGOROTH,
                NationRegionsEnum.MORDOR,
                NationRegionsEnum.UNGOL,
            }
        },
        { NationsEnum.DENDRA_DWAR, new()
            {
                NationRegionsEnum.UDUN_AND_DAGORLAD,
                NationRegionsEnum.RHUN,
                NationRegionsEnum.ITHILIEN,
                NationRegionsEnum.MORDOR,
            }
        },
        { NationsEnum.DENETHOR, new()
            {
                NationRegionsEnum.GONDOR,
                NationRegionsEnum.ITHILIEN,
            }
        },
        { NationsEnum.REN, new()
            {
                NationRegionsEnum.UNGOL,
                NationRegionsEnum.GORGOROTH,
                NationRegionsEnum.MORDOR,
            }
        },
        { NationsEnum.STRIDER, new()
            {
                NationRegionsEnum.ERIADOR,
                NationRegionsEnum.RHUDAUR,
                NationRegionsEnum.LINDON,
            }
        },
        { NationsEnum.ENION, new()
            {
                NationRegionsEnum.DUNLAND_AND_ENEDWAITH,
                NationRegionsEnum.ROHAN,
                NationRegionsEnum.EREGION,
            }
        },
        { NationsEnum.CIRDAN, new()
            {
                NationRegionsEnum.LINDON,
                NationRegionsEnum.ERIADOR,
                NationRegionsEnum.BLUE_MOUNTAINS,
            }
        },
        { NationsEnum.HARUTH_RAMAM, new()
            {
                NationRegionsEnum.HARAD,
                NationRegionsEnum.GONDOR,
            }
        },

        { NationsEnum.ANGAMAITE, new()
            {
                NationRegionsEnum.HARAD,
                NationRegionsEnum.UMBAR,
            }
        },
        { NationsEnum.OVATHA_II, new()
            {
                NationRegionsEnum.KHAND
            }
        },
        { NationsEnum.PALADIN, new()
            {
                NationRegionsEnum.ERIADOR,
            }
        },
        { NationsEnum.JI_INDUR, new()
            {
                NationRegionsEnum.MORDOR,
                NationRegionsEnum.HARAD,
            }
        },
        { NationsEnum.AKHORAHIL, new()
            {
                NationRegionsEnum.MORDOR,
                NationRegionsEnum.KHAND,
            }
        },
        { NationsEnum.GANDALF, new()
            {
                NationRegionsEnum.ERIADOR,
                NationRegionsEnum.RHUDAUR,
                NationRegionsEnum.ROHAN,
                NationRegionsEnum.GONDOR,
                NationRegionsEnum.MISTY_MOUNTAINS,
            }
        },
    };

    public static short INFLUENCE = 20;

}

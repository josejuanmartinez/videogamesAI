using UnityEngine;

public class GoldRingDetails : CardDetails
{
    [Header("Gold Ring")]
    public short theOneRingMax = 12;
    public short theOneRingMin = 12;

    public short dwarvenRingMax = 12;
    public short dwarvenRingMin = 10;

    public short magicRingMax = 12;
    public short magicRingMin = 9;
    
    public short mindRingMax = 12;
    public short mindRingMin = 7;

    private RingType revealedSlot;

    private void Awake()
    {
        base.Initialize(CardClass.GoldRing, new Resources(0, 0, 0, 0, 0, 0, 0, 0));
    }
    public void Initialize()
    {
        Awake();
    }

    public string OneRing()
    {
        return Between(theOneRingMin, theOneRingMax);
    }
    public string DwarvenRing()
    {
        return Between(dwarvenRingMin, dwarvenRingMax);
    }
    public string MagicRing()
    {
        return Between(magicRingMin, magicRingMax);
    }
    public string MindRing()
    {
        return Between(mindRingMin, mindRingMax);
    }
    public string Between(int min, int max)
    {
        if (min == max)
            return min.ToString();
        else if (max < min)
            return "-";
        else
            return min.ToString() + " - " + max.ToString();
    }

    public bool CanConvertInto(RingType slot)
    {
        switch(slot)
        {
            case RingType.MindRing:
                return mindRingMax > 0 && mindRingMin > 0;
            case RingType.DwarvenRing:
                return dwarvenRingMax > 0 && dwarvenRingMin > 0;
            case RingType.MagicRing:
                return magicRingMax > 0 && magicRingMin > 0;
            case RingType.TheOneRing:
                return theOneRingMax > 0 && theOneRingMin> 0;
            default:
                break;
        }
        return false;
    }
    public RingType GetRevealedSlot()
    {
        return revealedSlot;
    }
    public void SetRevealedSlot(RingType revealedSlot)
    {
        this.revealedSlot = revealedSlot;
    }
}

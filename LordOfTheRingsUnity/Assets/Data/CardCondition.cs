using System;
using System.Collections.Generic;
using UnityEngine;

public class CardCondition
{
    private DirtyReasonEnum dirtyCheck;
    private List<string> involvedSprites;
    private Func<HashSet<PlayableConditionResultEnum>> condition;
    private HashSet<PlayableConditionResultEnum> lastResult;

    public CardCondition( DirtyReasonEnum dirtyCheck, Func<HashSet<PlayableConditionResultEnum>> condition)
    {
        this.dirtyCheck = dirtyCheck;
        this.involvedSprites = new();
        switch (dirtyCheck)
        {
            case DirtyReasonEnum.INITIALIZATION:
                foreach (ResourceType c in Enum.GetValues(typeof(ResourceType)))
                    involvedSprites.Add(c.ToString());
                foreach (CardTypesEnum c in Enum.GetValues(typeof(CardTypesEnum)))
                    involvedSprites.Add(c.ToString());
                involvedSprites.Add("character");
                involvedSprites.Add("city");
                involvedSprites.Add("slot");
                involvedSprites.Add("ring");
                involvedSprites.Add("influence");
                break;
            case DirtyReasonEnum.CHAR_SELECTED:
                involvedSprites.Add("character");
                involvedSprites.Add("city");
                involvedSprites.Add("slot");
                involvedSprites.Add("ring");
                break;
            case DirtyReasonEnum.NEW_RESOURCES:
                foreach (ResourceType c in Enum.GetValues(typeof(ResourceType)))
                    involvedSprites.Add(c.ToString());
                break;
            case DirtyReasonEnum.NEW_MANA:
                foreach (CardTypesEnum c in Enum.GetValues(typeof(CardTypesEnum)))
                    involvedSprites.Add(c.ToString());
                break;
            case DirtyReasonEnum.NEW_INFLUENCE:
                involvedSprites.Add("influence");
                break;
            case DirtyReasonEnum.NONE:
                break;
        }

        this.condition = condition;
        lastResult = new();
    }
    public DirtyReasonEnum GetDirtyCheck()
    {
        return dirtyCheck;
    }
    public HashSet<PlayableConditionResultEnum> RunCondition()
    {
        lastResult = condition();
        return lastResult;
    }

    public HashSet<PlayableConditionResultEnum> GetLastResult()
    {
        return lastResult;
    }

    public List<string> GetInvolvedSprites()
    {
        return involvedSprites;
    }
}

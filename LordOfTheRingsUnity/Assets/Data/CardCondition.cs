using System;
using UnityEngine;

public class CardCondition
{
    private DirtyReasonEnum dirtyCheck {  get; set; }
    private Func<bool> condition {  get; set; }

    public CardCondition( DirtyReasonEnum dirtyCheck, Func<bool> condition)
    {
        this.dirtyCheck = dirtyCheck;
        this.condition = condition;
    }
}

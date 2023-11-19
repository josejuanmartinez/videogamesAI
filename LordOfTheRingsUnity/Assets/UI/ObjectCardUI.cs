using UnityEngine;

public class ObjectCardUI : CardUI
{
    public override bool Initialize(string cardId, NationsEnum owner)
    {
        if (!base.Initialize(cardId, owner))
            return false;

        initialized = true;

        return true;
    }
}

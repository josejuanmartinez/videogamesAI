using UnityEngine;

public enum BattleResult
{
    WON,
    HURT,
    RESISTED,
    EXHAUSTED,
}

public readonly struct CombatResult
{
    readonly BattleResult battleResult;
    readonly StatusEffect statusEffect;
    public CombatResult(BattleResult battleResult,  StatusEffect statusEffect)
    {
        this.battleResult = battleResult;
        this.statusEffect = statusEffect;
    }

    public readonly BattleResult GetBattleResult()
    {
        return battleResult;
    }

    public readonly StatusEffect GetStatusEffect()
    {
        return statusEffect;
    }
}

public static class CombatCalculator
{
    public static CombatResult Combat(
        int attackerProwess, 
        int attackerDefence,
        int defenderProwess, 
        int defenderDefence,
        StatusEffect attackerStatusEffectAbility = StatusEffect.NONE,
        float criticalFrom = 0.9f)
    {
        if(attackerProwess > defenderProwess)
        {
            if(attackerStatusEffectAbility != StatusEffect.NONE)
            {
                if (Random.Range(0f, 1f) >= criticalFrom)
                    return new CombatResult(BattleResult.HURT, attackerStatusEffectAbility);
                else
                    return new CombatResult(BattleResult.HURT, StatusEffect.NONE);
            }            
            else if (attackerProwess > defenderDefence)
                return new CombatResult(BattleResult.HURT, StatusEffect.NONE);
            else
                return new CombatResult(BattleResult.EXHAUSTED, StatusEffect.NONE);
        }
        else
        {
            if (defenderProwess > attackerDefence)
                return new CombatResult(BattleResult.WON, StatusEffect.NONE);
            else
                return new CombatResult(BattleResult.RESISTED, StatusEffect.NONE);
        }
    }
}

public enum BattleResult
{
    WON,
    HURT,
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
        int enemyProwess, 
        int enemyDefence,
        int playerProwess, 
        int playerDefence,
        float critical,
        StatusEffect enemyStatusEffect = StatusEffect.NONE)
    {
        if (enemyProwess > playerDefence)
        {
            if (enemyStatusEffect != StatusEffect.NONE && enemyProwess > playerProwess && UnityEngine.Random.Range(0f,1f) >= critical)
                return new CombatResult(BattleResult.HURT, enemyStatusEffect);
            else
                return new CombatResult(BattleResult.HURT, StatusEffect.NONE);        
        }
        else if (enemyDefence <= playerDefence)
            return new CombatResult(BattleResult.WON, StatusEffect.NONE);
        else
            return new CombatResult(BattleResult.EXHAUSTED, StatusEffect.NONE);
    }
}

public enum StatusEffect
{
    NONE,
    BLOOD,
    POISON,
    MORGUL,
    ICE,
    FIRE,
    IMMOVIBILITY,
    BLIND,
    WOUND,
    EXHAUSTATION,
    TRAP,
    BUFF,
    DEBUFF
}

public struct StatusEffectsApplied
{
    public StatusEffect effect;
    public int turn;
    public bool healable;

    public StatusEffectsApplied(StatusEffect effect, int turn, bool healable)
    {
        this.effect = effect;
        this.turn = turn;
        this.healable = healable;
    }
}

using System.Collections.Generic;
using UnityEngine.EventSystems;

public class AttackerToCreature: Attacker, IPointerEnterHandler, IPointerExitHandler
{
    private HazardCreatureCardUIPopup targetCreature;
    public override bool Initialize(string cardId, Dictionary<string, CardUI> company, int attackerNum, NationsEnum owner)
    {
        if (!base.Initialize(cardId, company, attackerNum, owner))
            return false;

        targetCreature = target as HazardCreatureCardUIPopup;
        if (targetCreature == null)
            return false;
        initialized = true;

        return true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        targetCreature.UndrawTargetted();
        if (attackerDetails != null)
            placeDeck.RemoveCardToShow(new HoveredCard(attackerNation, attackerDetails.cardId, attackerDetails.cardClass));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!initialized || resolved)
            return;
        targetCreature.DrawTargetted();
        if (attackerDetails != null)
            placeDeck.SetCardToShow(new HoveredCard(attackerNation, attackerDetails.cardId, attackerDetails.cardClass));
    }
    public bool GatherDiceResults(int diceValue)
    {
        if (!initialized)
            return false;

        short diceResults = (short)diceValue;

        int raceEffects = 0;
        switch (race)
        {
            case RacesEnum.Dwarf:
                if (targetCreature.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.HatesDwarves))
                    raceEffects = 1;
                break;
            case RacesEnum.Elf:
                if (targetCreature.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.HatesElves))
                    raceEffects = 1;
                break;
            case RacesEnum.Man:
                if (targetCreature.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.HatesElves))
                    raceEffects = 1;
                break;
            case RacesEnum.Plant:
                if (targetCreature.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.Burns))
                    raceEffects = 2;
                break;
        }

        int counterEffects = 0;
        if (targetCreature.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.CountersMounted) &&
            attackerDetails.GetAbilities().Contains(HazardAbilitiesEnum.Mounted))
            counterEffects += 1;


        int playerProwess = targetCreature.GetTotalProwess() + diceResults + raceEffects;

        int playerDefence = targetCreature.GetTotalDefence() + diceResults + counterEffects + raceEffects;
        
        if (playerProwess < 1)
            playerProwess = 1;

        if (playerDefence < 1)
            playerDefence = 1;

        CombatResult combatResult = CombatCalculator.Combat(
            attackerDetails.prowess,
            attackerDetails.defence,
            playerProwess,
            playerDefence,
            game.GetCriticalByDifficulty(),
            attackerDetails.GetStatusEffect()
        );

        switch (combatResult.GetBattleResult())
        {
            case BattleResult.EXHAUSTED:
                targetCreature.Exhausted(attackerDetails);
                break;
            case BattleResult.HURT:
                targetCreature.Hurt(attackerDetails);
                break;
            case BattleResult.WON:
                targetCreature.Won(attackerDetails);
                break;
        }

        switch (combatResult.GetStatusEffect())
        {
            case StatusEffect.BLOOD:
                targetCreature.Bleeding();
                break;
            case StatusEffect.POISON:
                targetCreature.Poisoned();
                break;
            case StatusEffect.MORGUL:
                targetCreature.Morgul();
                break;
            case StatusEffect.ICE:
                targetCreature.Ice();
                break;
            case StatusEffect.FIRE:
                targetCreature.Fire();
                break;
            case StatusEffect.TRAP:
                targetCreature.Trapped();
                break;
            case StatusEffect.BLIND:
                targetCreature.Blind();
                break;
        }

        canvasGroup.alpha = 0.25f;
        result.enabled = true;
        result.sprite = combatResult.GetBattleResult() == BattleResult.WON ? spritesRepo.GetSprite("win") : spritesRepo.GetSprite("lose");
        button.interactable = false;

        resolved = true;

        return combatResult.GetBattleResult() != BattleResult.HURT;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class AttackerToCreature: Attacker, IPointerEnterHandler, IPointerExitHandler
{
    private HazardCreatureCardUIPopup target;
    private NationsEnum attackerNation;
    public override bool Initialize(string cardId, Dictionary<string, CardUI> company, int attackerNum, NationsEnum owner)
    {
        if (!base.Initialize(cardId, company, attackerNum, owner))
            return false;

        int target_num = UnityEngine.Random.Range(0, this.company.Count);
        target = this.company[this.company.Keys.ToList()[target_num]] as HazardCreatureCardUIPopup;
        if (target == null)
            return false;
        attackerNation = owner;
        initialized = true;

        return true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        target.UndrawTargetted();
        if (attackerDetails != null)
            placeDeck.RemoveCardToShow(new HoveredCard(attackerNation, attackerDetails.cardId, attackerDetails.cardClass));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!initialized || resolved)
            return;
        target.DrawTargetted();
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
                if (target.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.HatesDwarves))
                    raceEffects = 1;
                break;
            case RacesEnum.Elf:
                if (target.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.HatesElves))
                    raceEffects = 1;
                break;
            case RacesEnum.Man:
                if (target.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.HatesElves))
                    raceEffects = 1;
                break;
            case RacesEnum.Plant:
                if (target.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.Burns))
                    raceEffects = 2;
                break;
        }

        int counterEffects = 0;
        if (target.GetHazardCreatureDetails().GetAbilities().Contains(HazardAbilitiesEnum.CountersMounted) &&
            attackerDetails.GetAbilities().Contains(HazardAbilitiesEnum.Mounted))
            counterEffects += 1;


        int playerProwess = target.GetTotalProwess() + diceResults + raceEffects;

        int playerDefence = target.GetTotalDefence() + diceResults + counterEffects + raceEffects;
        
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
                target.Exhausted(attackerDetails);
                break;
            case BattleResult.HURT:
                target.Hurt(attackerDetails);
                break;
            case BattleResult.WON:
                target.Won(attackerDetails);
                break;
        }

        switch (combatResult.GetStatusEffect())
        {
            case StatusEffect.BLOOD:
                target.Bleeding();
                break;
            case StatusEffect.POISON:
                target.Poisoned();
                break;
            case StatusEffect.MORGUL:
                target.Morgul();
                break;
            case StatusEffect.ICE:
                target.Ice();
                break;
            case StatusEffect.FIRE:
                target.Fire();
                break;
            case StatusEffect.TRAP:
                target.Trapped();
                break;
            case StatusEffect.BLIND:
                target.Blind();
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

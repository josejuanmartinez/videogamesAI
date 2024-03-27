using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackerToCompany: Attacker, IPointerEnterHandler, IPointerExitHandler
{
    private CharacterCardUIPopup characterTarget;

    public override bool Initialize(string cardId, Dictionary<string, CardUI> company, int attackerNum, NationsEnum owner)
    {
        if (!base.Initialize(cardId, company, attackerNum, owner))
            return false;
        
        characterTarget = target as CharacterCardUIPopup;
        if (characterTarget == null)
            return false;

        initialized = true;

        return true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        characterTarget.UndrawTargetted();
        if (attackerDetails != null)
            placeDeck.RemoveCardToShow(new HoveredCard(attackerNation, attackerDetails.cardId, attackerDetails.cardClass));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!initialized || resolved)
            return;
        characterTarget.DrawTargetted();
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
            case RacesEnum.Ringwraith:
                if (characterTarget.GetCharacterDetails().GetAbilities().Contains(CharacterAbilitiesEnum.BonusToNazgul))
                    raceEffects = 3;
                break;            
        }

        int playerProwess = characterTarget.GetTotalProwess() + diceResults + raceEffects;

        int playerDefence = characterTarget.GetTotalDefence() + diceResults;

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
            attackerDetails.GetStatusEffect());

        switch (combatResult.GetBattleResult())
        {
            case BattleResult.EXHAUSTED:
                characterTarget.Exhausted(attackerDetails);
                break;
            case BattleResult.HURT:
                characterTarget.Hurt(attackerDetails);
                break;
            case BattleResult.WON:
                characterTarget.Won(attackerDetails);
                break;
        }

        switch (combatResult.GetStatusEffect())
        {
            case StatusEffect.BLOOD:
                characterTarget.Bleeding();
                break;
            case StatusEffect.POISON:
                characterTarget.Poisoned();
                break;
            case StatusEffect.MORGUL:
                characterTarget.Morgul();
                break;
            case StatusEffect.ICE:
                characterTarget.Ice();
                break;
            case StatusEffect.FIRE:
                characterTarget.Fire();
                break;
            case StatusEffect.TRAP:
                characterTarget.Trapped();
                break;
            case StatusEffect.BLIND:
                characterTarget.Blind();
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

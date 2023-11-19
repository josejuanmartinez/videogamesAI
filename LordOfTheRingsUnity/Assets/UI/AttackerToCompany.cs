using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackerToCompany: Attacker, IPointerEnterHandler, IPointerExitHandler
{
    private CharacterCardUIPopup target;
    private NationsEnum attackerNation;
    public override bool Initialize(string cardId, Dictionary<string, CardUI> company, int attackerNum, NationsEnum owner)
    {
        if (!base.Initialize(cardId, company, attackerNum, owner))
            return false;

        int target_num = UnityEngine.Random.Range(0, this.company.Count);
        target = this.company[this.company.Keys.ToList()[target_num]] as CharacterCardUIPopup;
        if (target == null)
            return false;

        attackerNation = owner;
        initialized = true;

        return true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        target.UndrawTargetted();
        if (hazardDetails != null)
            placeDeck.RemoveCardToShow(new HoveredCard(attackerNation, hazardDetails.cardId, hazardDetails.cardClass));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!initialized || resolved)
            return;
        target.DrawTargetted();
        if (hazardDetails != null)
            placeDeck.SetCardToShow(new HoveredCard(attackerNation, hazardDetails.cardId, hazardDetails.cardClass));
    }
    public bool GatherDiceResults(int diceValue)
    {
        if (!initialized)
            return false;

        short diceResults = (short)diceValue;

        int prowess = target.GetTotalProwess() + diceResults;

        int defence = target.GetTotalDefence() + diceResults;

        if (defence < 1)
            defence = 1;

        CombatResult combatResult = CombatCalculator.Combat(
            hazardDetails.prowess,
            hazardDetails.defence,
            prowess,
            defence,
            hazardDetails.GetStatusEffect(),
            game.GetCriticalByDifficulty());

        switch (combatResult.GetBattleResult())
        {
            case BattleResult.EXHAUSTED:
                //NOTHING HAPPENS
                break;
            case BattleResult.HURT:
                //DIES
                break;
            case BattleResult.WON:
                //CITY LOSSES HEALTH
                break;
            case BattleResult.RESISTED:
                //NOTHING HAPPENS
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

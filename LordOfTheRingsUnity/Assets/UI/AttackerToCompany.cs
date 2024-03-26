using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackerToCompany: Attacker, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float waitForAnimationBase = 2f;
    [SerializeField]
    private float moveSpeed = 2f;

    private CharacterCardUIPopup target;
    private NationsEnum attackerNation;
    private float waitForAnimation;
    
    private Vector3 NONE = Vector3.one * int.MinValue;
    private Vector3 moveTo = Vector3.one * int.MinValue;

    public override bool Initialize(string cardId, Dictionary<string, CardUI> company, int attackerNum, NationsEnum owner)
    {
        if (!base.Initialize(cardId, company, attackerNum, owner))
            return false;
        
        waitForAnimation = waitForAnimationBase * (attackerNum + 1);
        
        int target_num = UnityEngine.Random.Range(0, this.company.Count);
        target = this.company[this.company.Keys.ToList()[target_num]] as CharacterCardUIPopup;
        if (target == null)
            return false;

        attackerNation = owner;
        initialized = true;

        StartCoroutine(AttackAnimation());

        return true;
    }
    IEnumerator AttackAnimation()
    {
        yield return new WaitForSecondsRealtime(waitForAnimation);
        yield return new WaitWhile(() => diceManager.IsDicing());
        moveTo = target.gameObject.transform.position;
    }

    void Update()
    {
        if (moveTo != NONE)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, moveTo, Time.deltaTime * moveSpeed);
            transform.position = newPosition;
            if (transform.position == moveTo)
                moveTo = NONE;
        }
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
            case RacesEnum.Ringwraith:
                if (target.GetCharacterDetails().GetAbilities().Contains(CharacterAbilitiesEnum.BonusToNazgul))
                    raceEffects = 3;
                break;            
        }

        int playerProwess = target.GetTotalProwess() + diceResults + raceEffects;

        int playerDefence = target.GetTotalDefence() + diceResults;

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

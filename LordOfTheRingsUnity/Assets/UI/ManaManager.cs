using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManager : MonoBehaviour
{
    public float hideAddManaSec = 3f;
    public TMPro.TextMeshProUGUI textFreeBastion;
    public TMPro.TextMeshProUGUI textDarkBastion;
    public TMPro.TextMeshProUGUI textNeutralBastion;
    public TMPro.TextMeshProUGUI textLair;
    public TMPro.TextMeshProUGUI textWilderness;
    public TMPro.TextMeshProUGUI textSea;

    public GameObject animationMana;
    public CanvasGroup animationManaCG;
    public CanvasGroup freeCG;
    public CanvasGroup darkCG;
    public CanvasGroup neutralCG;
    public CanvasGroup lairCG;
    public CanvasGroup wildCG;
    public CanvasGroup seaCG;

    public TMPro.TextMeshProUGUI textFreeBastionAdd;
    public TMPro.TextMeshProUGUI textDarkBastionAdd;
    public TMPro.TextMeshProUGUI textNeutralBastionAdd;
    public TMPro.TextMeshProUGUI textLairAdd;
    public TMPro.TextMeshProUGUI textWildernessAdd;
    public TMPro.TextMeshProUGUI textSeaAdd;

    private bool dirty = false;

    public Dictionary<NationsEnum, Dictionary<CardTypesEnum, short>> mana;
    public Dictionary<NationsEnum, Dictionary<CardTypesEnum, short>> addMana;

    private DeckManager deckManager;
    private ColorManager colorManager;
    private Turn turn;

    void Awake()
    {
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        colorManager = GameObject.Find("ColorManager").GetComponent<ColorManager>();

        mana = new();
        addMana = new();
        foreach (NationsEnum nation in Enum.GetValues(typeof(NationsEnum)))
        {
            mana[nation] = new Dictionary<CardTypesEnum, short> {
                { CardTypesEnum.FREE_BASTION, 0 },
                { CardTypesEnum.DARK_BASTION, 0 },
                { CardTypesEnum.NEUTRAL_BASTION, 0 },
                { CardTypesEnum.LAIR, 0 },
                { CardTypesEnum.WILDERNESS, 0 },
                { CardTypesEnum.SEA, 0 }
            };
        }
        foreach (NationsEnum nation in Enum.GetValues(typeof(NationsEnum)))
        {
            addMana[nation] = new Dictionary<CardTypesEnum, short> {
                { CardTypesEnum.FREE_BASTION, 0 },
                { CardTypesEnum.DARK_BASTION, 0 },
                { CardTypesEnum.NEUTRAL_BASTION, 0 },
                { CardTypesEnum.LAIR, 0 },
                { CardTypesEnum.WILDERNESS, 0 },
                { CardTypesEnum.SEA, 0 }
            };
        }
        animationManaCG.alpha = 0;
    }
    void Update()
    {
        if (dirty)
        {
            textFreeBastion.text = mana[turn.GetCurrentPlayer()][CardTypesEnum.FREE_BASTION].ToString();
            textDarkBastion.text = mana[turn.GetCurrentPlayer()][CardTypesEnum.DARK_BASTION].ToString();
            textNeutralBastion.text = mana[turn.GetCurrentPlayer()][CardTypesEnum.NEUTRAL_BASTION].ToString();
            textLair.text = mana[turn.GetCurrentPlayer()][CardTypesEnum.LAIR].ToString();
            textWilderness.text = mana[turn.GetCurrentPlayer()][CardTypesEnum.WILDERNESS].ToString();
            textSea.text = mana[turn.GetCurrentPlayer()][CardTypesEnum.SEA].ToString();

            deckManager.Dirty(DirtyReason.NEW_RESOURCES);
            dirty = false;
        }
    }

    public bool HasEnoughMana(NationsEnum nation, List<CardTypesEnum> cards)
    {
        foreach (CardTypesEnum card in cards)
        {
            int required = cards.FindAll(x => x.Equals(card)).Count;
            if (required > 0)
            {
                if (mana[nation][card] >= required)
                    return true;
            }
        }
        return false;
    }

    public void AddMana(NationsEnum nation, List<CardTypesEnum> cardTypes)
    {
        foreach (CardTypesEnum cardType in cardTypes)
            AddMana(nation, cardType);
        if (nation == turn.GetCurrentPlayer())
        {
            Dirty();
            AnimateMana(nation, true);
        }        
    }
    public void RemoveMana(NationsEnum nation, List<CardTypesEnum> cardTypes)
    {
        foreach (CardTypesEnum cardType in cardTypes)
            RemoveMana(nation, cardType);
        if(nation == turn.GetCurrentPlayer())
        {
            Dirty();
            AnimateMana(nation, false);
        }
    }
    private void RemoveMana(NationsEnum nation, CardTypesEnum cardType)
    {
        mana[nation][cardType]--;
        addMana[nation][cardType]--;
    }

    private void AddMana(NationsEnum nation, CardTypesEnum cardType)
    {
        mana[nation][cardType]++;
        addMana[nation][cardType]++;
    }

    void RefreshAnimationMana(NationsEnum nation)
    {
        addMana[nation] = new Dictionary<CardTypesEnum, short> {
            { CardTypesEnum.FREE_BASTION, 0 },
            { CardTypesEnum.DARK_BASTION, 0 },
            { CardTypesEnum.NEUTRAL_BASTION, 0 },
            { CardTypesEnum.LAIR, 0 },
            { CardTypesEnum.WILDERNESS, 0 },
            { CardTypesEnum.SEA, 0 }
        };
    }

    public void AnimateMana(NationsEnum nation, bool isAddition)
    {;
        foreach (CardTypesEnum cardType in Enum.GetValues(typeof(CardTypesEnum))) {
            switch(cardType)
            {
                case CardTypesEnum.FREE_BASTION:
                    textFreeBastionAdd.text = string.Format("{0}{1}", isAddition ? "+" : "-", addMana[nation][cardType].ToString());
                    textFreeBastionAdd.color = colorManager.GetColor(isAddition ? "success" : "failure");
                    freeCG.alpha = addMana[nation][cardType] < 1 ? 0 : 1;
                    break;
                case CardTypesEnum.NEUTRAL_BASTION:
                    textNeutralBastionAdd.text = string.Format("{0}{1}", isAddition ? "+" : "-", addMana[nation][cardType].ToString());
                    textNeutralBastionAdd.color = colorManager.GetColor(isAddition ? "success" : "failure");
                    neutralCG.alpha = addMana[nation][cardType] < 1 ? 0 : 1;
                    break;
                case CardTypesEnum.DARK_BASTION:
                    textDarkBastionAdd.text = string.Format("{0}{1}", isAddition ? "+" : "-", addMana[nation][cardType].ToString());
                    textDarkBastionAdd.color = colorManager.GetColor(isAddition ? "success" : "failure");
                    darkCG.alpha = addMana[nation][cardType] < 1 ? 0 : 1;
                    break;
                case CardTypesEnum.LAIR:
                    textLairAdd.text = string.Format("{0}{1}", isAddition ? "+" : "-", addMana[nation][cardType].ToString());
                    textLairAdd.color = colorManager.GetColor(isAddition ? "success" : "failure");
                    lairCG.alpha = addMana[nation][cardType] < 1 ? 0 : 1;
                    break;
                case CardTypesEnum.WILDERNESS:
                    textWildernessAdd.text = string.Format("{0}{1}", isAddition ? "+" : "-", addMana[nation][cardType].ToString());
                    textWildernessAdd.color = colorManager.GetColor(isAddition ? "success" : "failure");
                    wildCG.alpha = addMana[nation][cardType] < 1 ? 0 : 1;
                    break;
                case CardTypesEnum.SEA:
                    textSeaAdd.text = string.Format("{0}{1}", isAddition ? "+" : "-", addMana[nation][cardType].ToString());
                    textSeaAdd.color = colorManager.GetColor(isAddition ? "success" : "failure");
                    seaCG.alpha = addMana[nation][cardType] < 1 ? 0 : 1;
                    break;
            }
        }
        animationManaCG.alpha = 1;
        animationMana.GetComponent<Animation>().Play();
        StartCoroutine(RewindMana());

    }

    IEnumerator RewindMana()
    {
        yield return new WaitForSeconds(hideAddManaSec);
        animationMana.GetComponent<Animation>().Rewind();
        RefreshAnimationMana(turn.GetCurrentPlayer());
        animationManaCG.alpha = 0;
    }

    public void AddMana(NationsEnum nation, CardTypesEnum cardType, short amount)
    {
        mana[nation][cardType] += amount;
    }
    public void Dirty()
    {
        dirty = true;
    }

    /*public bool AddManaToOpponent(NationsEnum owner, CardDetails cardDetails, List<CardTypesEnum> accumulatedMana)
    {
        if (cardDetails as HazardCreatureCardDetails == null)
            return false;

        foreach(CardTypesEnum cardTypes in accumulatedMana)
            mana[owner][cardTypes]++;
        return HasEnoughMana(owner, (cardDetails as HazardCreatureCardDetails).cardTypes);
    }*/
}

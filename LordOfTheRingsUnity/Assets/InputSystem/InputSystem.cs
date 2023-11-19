using System;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    private ResourcesManager resourcesManager;
    private Turn turn;
    private Board board;
    private ManaManager manaManager;

    private readonly short addition = 100;
    private void Awake()
    {
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        board = GameObject.Find("Board").GetComponent<Board>();
        manaManager = GameObject.Find("ManaManager").GetComponent<ManaManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                resourcesManager.AddToStores(turn.GetCurrentPlayer(),
                    new(addition, addition, addition, addition, addition, addition, addition, addition));

                board.GetCharacterManager().RefreshMovement(turn.GetCurrentPlayer());
                board.GetHazardCreaturesManager().RefreshMovement(turn.GetCurrentPlayer());

                List<CardTypesEnum> accumulatedMana = new();
                foreach (CardTypesEnum cardType in Enum.GetValues(typeof(CardTypesEnum)))
                    for(int i=0; i<addition;i++)
                        accumulatedMana.Add(cardType);

                manaManager.AddMana(turn.GetCurrentPlayer(), accumulatedMana);
                return;
            }
        }
    }
}

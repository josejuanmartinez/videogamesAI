using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyInputManager : MonoBehaviour
{
    private SelectedItems selectedItems;
    private MovementManager movementManager;
    private Game game;
    private Board board;
    private ResourcesManager resourcesManager;
    private Turn turn;
    private ManaManager manaManager;

    private readonly short addition = 100;
    void Awake()
    {
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
        game = GameObject.Find("Game").GetComponent<Game>();
        board = GameObject.Find("Board").GetComponent<Board>();
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        turn = GameObject.Find("Turn").GetComponent<Turn>();
        manaManager = GameObject.Find("ManaManager").GetComponent<ManaManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!game.FinishedLoading())
            return;

        MovementKeys();
        SelectionKeys();
        LookDropdownKeys();
        CheatKeys();
    }

    void MovementKeys()
    {
        short direction = -1;
        if (Input.GetKeyUp(KeyCode.A))
            direction = MovementManager.LEFT;
        else if (Input.GetKeyUp(KeyCode.Q))
            direction = MovementManager.UP_LEFT;
        else if (Input.GetKeyUp(KeyCode.E))
            direction = MovementManager.UP_RIGHT;
        else if (Input.GetKeyUp(KeyCode.D))
            direction = MovementManager.RIGHT;
        else if (Input.GetKeyUp(KeyCode.C))
            direction = MovementManager.DOWN_RIGHT;
        else if (Input.GetKeyUp(KeyCode.Z))
            direction = MovementManager.DOWN_LEFT;

        if (direction == -1)
            return;

        bool movableCard = selectedItems.IsMovableSelected() || selectedItems.IsHazardCreatureSelected();

        if (movableCard)
        {
            CardUI cardUI = selectedItems.GetSelectedMovableCardUI();
            if (cardUI == null)
                return;

            CardDetails cardDetails = cardUI.GetDetails();

            if (cardDetails == null)
                return;

            bool isImmovable = false;
            Vector2Int hex = new(int.MinValue, int.MinValue);

            if (cardDetails.cardClass == CardClass.Character)
            {
                CharacterCardUIBoard character = cardUI as CharacterCardUIBoard;
                if (character == null)
                    return;
                if (cardDetails as CharacterCardDetails != null)
                {
                    CharacterCardDetails charDetails = cardDetails as CharacterCardDetails;
                    isImmovable = charDetails.isImmovable;
                }
                hex = character.GetHex();
            }
            else if (cardDetails.cardClass == CardClass.HazardCreature)
            {
                HazardCreatureCardUIBoard creature = cardUI as HazardCreatureCardUIBoard;
                if (creature == null)
                    return;
                hex = creature.GetHex();
            }

            if (isImmovable)
                return;
            List<Vector3Int> surroundings = HexTranslator.GetSurroundingsWithoutSelf(hex);
            if (Input.GetKeyUp(KeyCode.A))
                direction = MovementManager.LEFT;
            if (Input.GetKeyUp(KeyCode.Q))
                direction = MovementManager.UP_LEFT;
            if (Input.GetKeyUp(KeyCode.E))
                direction = MovementManager.UP_RIGHT;
            if (Input.GetKeyUp(KeyCode.D))
                direction = MovementManager.RIGHT;
            if (Input.GetKeyUp(KeyCode.C))
                direction = MovementManager.DOWN_RIGHT;
            if (Input.GetKeyUp(KeyCode.Z))
                direction = MovementManager.DOWN_LEFT;
            if (direction != -1)
            {
                Vector3Int hex3D = new(hex.x, hex.y, 0);
                Vector3Int newHex = surroundings[direction];
                movementManager.Move(new List<Vector3Int>() { hex3D, newHex });
            }
        }
    }

    void SelectionKeys()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            selectedItems.UnselectAll();
            GameObject.Find("LookDropdown").GetComponent<LookDropdown>().Hide();
            return;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            CardUI next = board.GetNextCardUI(selectedItems.GetSelection().GetSelectedMovableCardUI());
            if (next != null)
                selectedItems.SelectCardDetails(next.GetDetails(), next.GetOwner());
        }
        selectedItems.CheckIfShowLastChar();
    }

    void LookDropdownKeys()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            if (Input.GetKeyUp(KeyCode.L))
                GameObject.Find("LookDropdown").GetComponent<LookDropdown>().Toggle();
    }

    void CheatKeys()
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
                    for (int i = 0; i < addition; i++)
                        accumulatedMana.Add(cardType);

                manaManager.AddMana(turn.GetCurrentPlayer(), accumulatedMana);
                return;
            }
        }
    }
}

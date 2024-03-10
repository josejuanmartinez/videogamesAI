using System.Collections.Generic;
using UnityEngine;

public class KeyInputManager : MonoBehaviour
{
    SelectedItems selectedItems;
    MovementManager movementManager;
    void Awake()
    {
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
    }

    // Update is called once per frame
    void Update()
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
                Vector3Int hex3D = new (hex.x, hex.y, 0);
                Vector3Int newHex = surroundings[direction];
                movementManager.Move(new List<Vector3Int>() { hex3D, newHex });
            }            
        }
    }
}

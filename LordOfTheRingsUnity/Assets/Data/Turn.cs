using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Turn : MonoBehaviour
{
    public TextMeshProUGUI turnText;
    public Image turnPlayerImage;
    public Image alignment;

    private short turnNumber;
    private NationsEnum currentTurnPlayer;

    private bool isDirty = true;

    private Board board;
    private Game game;
    private bool isInitialized;
    private bool newTurnLoaded;
    
    private CameraController cameraController;
    private SpritesRepo spritesRepo;
    private LookDropdown lookDropdown;

    private void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        cameraController = Camera.main.GetComponent<CameraController>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        lookDropdown = GameObject.Find("LookDropdown").GetComponent<LookDropdown>();
        isInitialized = false;
        newTurnLoaded = false;
        turnNumber = -1;
    }

    public void Initialize()
    {
        if(board.IsInitialized() && game.IsInitialized())
        {
            currentTurnPlayer = game.GetHumanNation();
            isInitialized = true;
            //Debug.Log("Turn initialized at " + Time.realtimeSinceStartup);
        }
    }

    void Update()
    {
        if (!isInitialized)
        {
            Initialize();
            return;
        }            
        
        if(turnNumber == -1)
        {
            NewTurn();
            return;
        }

        if (isDirty)
        {
            if(currentTurnPlayer == NationsEnum.ABANDONED)
                return;
            turnText.text = turnNumber.ToString();
            turnPlayerImage.sprite = spritesRepo.GetNationSprite(currentTurnPlayer);
            alignment.sprite = spritesRepo.GetSprite(Nations.alignments[currentTurnPlayer].ToString());
            isDirty = false;
        }
        
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public void FinishTurn()
    {
        return;
    }

    public void NewTurn()
    {
        newTurnLoaded = false;
        if (!board.IsAllLoaded())
            return;
        
        turnNumber++;

        isDirty = true;
        
        RefreshTurnInfo();
        if(turnNumber > 1)
        {
            RefreshStatusEffects();
            board.GetCharacterManager().RefreshMovement(currentTurnPlayer);
        }

        CardUI avatar = board.GetCharacterManager().GetAvatar(currentTurnPlayer);
        if (avatar)
            cameraController.LookToCard(avatar);
        
        newTurnLoaded = true;
        Debug.Log(string.Format("New turn: {0} at {1}", turnNumber, Time.realtimeSinceStartup));
    }

    public void RefreshStatusEffects()
    {
        foreach(NationsEnum nation in Enum.GetValues(typeof(NationsEnum)))
        {
            if (nation == NationsEnum.ABANDONED)
                return;
            foreach (CardUI character in board.GetCharacterManager().GetCharactersOfPlayer(nation))
            {
                if (character as CharacterCardUI == null)
                    (character as CharacterCardUI).SetEffects((character as CharacterCardUI).GetEffects().FindAll(x => turnNumber < (x.turn + game.GetTurnsOfStatusEffectByDifficulty(x.effect))).ToList());
            }
            foreach (CardUI creature in board.GetHazardCreaturesManager().GetHazardCreaturesOfPlayer(nation))
            {
                if (creature as HazardCreatureCardUI == null)
                    (creature as HazardCreatureCardUI).SetEffects((creature as HazardCreatureCardUI).GetEffects().FindAll(x => turnNumber < (x.turn + game.GetTurnsOfStatusEffectByDifficulty(x.effect))).ToList());
            }
        }
    }

    public bool IsNewTurnLoaded()
    {
        return newTurnLoaded;
    }

    public void RefreshTurnInfo()
    {
        turnText.text = turnNumber.ToString();
        //playerText.text = LocalizationEN.Localize(currentTurnPlayer.ToString()) + " [" + LocalizationEN.Localize(Nations.regions[currentTurnPlayer].ToString()) + "]";
        alignment.sprite = spritesRepo.GetSprite(Nations.alignments[currentTurnPlayer].ToString());
    }

    public NationsEnum GetCurrentPlayer()
    {
        return currentTurnPlayer;
    }

    public void PlayIATurn()
    {
        RefreshTurnInfo();
        NextPlayer();
    }

    public void NextPlayer()
    {
        currentTurnPlayer = (NationsEnum)(((int)currentTurnPlayer + 1) % Enum.GetValues(typeof(NationsEnum)).Length);
        if (currentTurnPlayer == NationsEnum.ABANDONED || currentTurnPlayer == game.GetHumanNation())
            NewTurn();
        else
            PlayIATurn();
    }

    public short GetTurnNumber()
    {
        return turnNumber;
    }
}

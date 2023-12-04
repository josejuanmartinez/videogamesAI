using System;
using System.Collections.Generic;
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

    private void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = GameObject.Find("Game").GetComponent<Game>();
        cameraController = Camera.main.GetComponent<CameraController>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        isInitialized = false;
        newTurnLoaded = false;
        turnNumber = -1;
    }

    public void Initialize()
    {
        if(board.IsInitialized() && game.IsInitialized())
        {
            currentTurnPlayer = game.GetHumanPlayer().GetNation();
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

        board.GetCharacterManager().RefreshMovement(currentTurnPlayer);
        
        CardUI avatar = board.GetCharacterManager().GetAvatar(currentTurnPlayer);
        if (avatar)
            cameraController.LookToCard(avatar);
        
        newTurnLoaded = true;
        //Debug.Log(string.Format("New turn: {0} at {1}", turnNumber, Time.realtimeSinceStartup));
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
        if (currentTurnPlayer == NationsEnum.ABANDONED || currentTurnPlayer == game.GetHumanPlayer().GetNation())
            NewTurn();
        else
            PlayIATurn();
    }

}

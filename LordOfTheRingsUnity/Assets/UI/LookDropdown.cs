using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMPro.TMP_Dropdown)), RequireComponent(typeof(GraphicRaycaster))]
public class LookDropdown : MonoBehaviour
{
    private TMPro.TMP_Dropdown dropdown;
    private Settings settings;
    private Board board;
    private CameraController cameraController;

    private Dictionary<string, string> unitsDict;
    private Dictionary<string, string> citiesDict;
    private SpritesRepo spritesRepo;
    private SelectedItems selectedItems;

    private bool hidden;

    void Awake()
    {
        dropdown = GetComponent<TMPro.TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(LookTo);
        settings = GameObject.Find("Settings").GetComponent<Settings>();
        board = GameObject.Find("Board").GetComponent<Board>();
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        selectedItems = GameObject.Find("SelectedItems").GetComponent<SelectedItems>();
        unitsDict = new();
        citiesDict = new();
        hidden = true;
    }

    public void Refresh()
    {
        dropdown.ClearOptions();
        citiesDict.Clear();
        unitsDict.Clear();
        List<CardUI> cards = board.GetCharacterManager().GetCharactersOfPlayer(settings.GetHumanPlayer());
        cards.AddRange(board.GetHazardCreaturesManager().GetHazardCreaturesOfPlayer(settings.GetHumanPlayer()));
        foreach (CardUI card in cards)
        {
            string localizedId = GameObject.Find("Localization").GetComponent<Localization>().Localize(card.GetCardId());
            unitsDict.Add(localizedId, card.GetCardId());
            dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(localizedId, spritesRepo.GetSprite("unit"), Color.white));
        }
        List<CityUI> cities = board.GetCityManager().GetCitiesOfPlayer(settings.GetHumanPlayer());
        foreach (CityUI city in cities)
        {
            string localizedId = GameObject.Find("Localization").GetComponent<Localization>().Localize(city.GetCityId());
            citiesDict.Add(localizedId, city.GetCityId());
            dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(localizedId, spritesRepo.GetSprite("city"), Color.white));
        }
    }
    public void LookTo(int value)
    {
        string localizedId = dropdown.options[value].text;
        if (citiesDict.ContainsKey(localizedId))
        {
            CityUI cityUI = board.GetCityManager().GetCityUI(citiesDict[localizedId]);
            if (cityUI != null)
                selectedItems.SelectCityUI(cityUI);
        }            
        else if (unitsDict.ContainsKey(localizedId))
        {
            CardUI cardUI = board.GetCardManager().GetCardUI(unitsDict[localizedId]);
            if (cardUI != null)
                selectedItems.SelectCardDetails(cardUI);
        }
        Hide();
    }

    public void Show()
    {
        GameObject.Find("LookupDropdownBackground").GetComponent<CanvasGroup>().alpha = 1;
        GameObject.Find("LookupDropdownBackground").GetComponent<CanvasGroup>().interactable = true;
        GameObject.Find("LookupDropdownBackground").GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameObject.Find("LookupDropdownBackground").GetComponent<GraphicRaycaster>().enabled = true;
        GetComponent<GraphicRaycaster>().enabled = true;
        hidden = false;
    }

    public void Hide()
    {
        GameObject.Find("LookupDropdownBackground").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("LookupDropdownBackground").GetComponent<CanvasGroup>().interactable = false;
        GameObject.Find("LookupDropdownBackground").GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.Find("LookupDropdownBackground").GetComponent<GraphicRaycaster>().enabled = false;
        GetComponent<GraphicRaycaster>().enabled = false;
        List<TMPro.TMP_Dropdown.OptionData> options = new();
        dropdown.options.ForEach((item) =>
        {
            options.Add(item);
        });

        dropdown.ClearOptions();
        dropdown.options = options;
        hidden = true;
    }

    public void Toggle()
    {
        if (hidden)
        {
            Refresh();
            Show();
        }
        else Hide();
    }
}

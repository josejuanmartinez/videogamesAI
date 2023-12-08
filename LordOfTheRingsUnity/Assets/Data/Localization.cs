using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

public class Localization: MonoBehaviour
{
    private SpritesRepo spritesRepo;
    private Dictionary<string, Dictionary<string, string>> locales;

    private readonly List<string> alreadyWarned = new();
    private bool initialized = false;

    void Awake()
    {
        locales = new();
        spritesRepo = GameObject.Find("SpritesRepo").GetComponent<SpritesRepo>();
        string folderPath = Application.streamingAssetsPath + "/Locales/";
        string[] filePaths = Directory.GetFiles(folderPath, "*.json");
        foreach(string file in filePaths)
        {
            if (File.Exists(file))
            {
                string locale = Path.GetFileNameWithoutExtension(file);
                string json = File.ReadAllText(file);
                locales.Add(locale, JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
            }
        }
        initialized = true;
    }

    public string LocalizeWithSprite(string key, string lan = "en-US")
    {
        if (!initialized)
        {
            Debug.LogWarning("Locales still not loaded");
            return null;
        }

        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("You are trying to localize and empty or null string.");
            return null;
        }
        if (locales.TryGetValue(lan, out Dictionary<string, string> dictionary))
        {
            bool existsSprite = spritesRepo.ExistsSpriteInSpriteAsset(key.ToLower());
            if (!existsSprite && !alreadyWarned.Contains(key.ToLower()))
            {
                alreadyWarned.Add(key.ToLower());
                Debug.Log(string.Format("{0} does not have sprite, consider adding one", key.ToLower()));
            }
            if (dictionary.TryGetValue(key.ToLower(), out string value))
            {
                return
                    existsSprite ?
                    string.Format("<link=\"{0}\"><sprite name=\"{0}\">aaaaaaaaaaaaaaa</link>", key.ToLower(), value) :
                    key;
            }
            else
            {
                Debug.Log(string.Format("{0} not localized, consider adding it to the json file.", key.ToLower()));
                return string.Format("[{0}]", key.ToLower());
                //TextInfo textInfo = new CultureInfo(lan, false).TextInfo;
                //return textInfo.ToTitleCase(key.Replace("_", " "));
            }
        }
        else
            Debug.LogError("Dictionary " + lan + " not found");
        return key.ToLower();
    }

    public string Localize(string key, string lan="en-US")
    {
        if (!initialized)
        {
            Debug.LogWarning("Locales still not loaded");
            return null;
        }
            
        if(string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("You are trying to localize and empty or null string.");
            return null;
        }
        if (locales.TryGetValue(lan, out Dictionary<string, string> dictionary))
        {
            if (dictionary.TryGetValue(key.ToLower(), out string value))
                return value;
            else
            {
                TextInfo textInfo = new CultureInfo(lan, false).TextInfo;
                return textInfo.ToTitleCase(key.Replace("_", " "));
            }
        }
        else
            Debug.LogError("Dictionary " + lan + " not found");
        return key.ToLower();
    }

    public string LocalizeQuote(string cardId, string lan = "en-US")
    {
        if (locales.TryGetValue(lan, out Dictionary<string, string> dictionary))
        {
            if (dictionary.TryGetValue("quote_" + cardId.ToLower(), out string value))
                return "\" " + value + "\"";
        }
        else
            Debug.LogError("Dictionary " + lan + " not found");
        return "";
    }

    public string LocalizeTooltipRight(string linkID, string lan = "en-US")
    {
        if (locales.TryGetValue(lan, out Dictionary<string, string> dictionary))
        {
            string res = "~" + Localize(linkID) + "\n";
            if (dictionary.TryGetValue("lefttooltip_" + linkID.ToLower(), out string value))
                res += value;
            return res;
        }
        else
            Debug.LogError("Dictionary " + lan + " not found");
        return "";
    }

    public string CreateDescriptionForGeneration(CardDetails details, string lan = "en-US")
    {
        string text = Localize("a", lan) + " ";
        if (locales.ContainsKey(lan))
        {
            switch (details.cardClass)
            {
                case CardClass.Character:
                    CharacterCardDetails charDetails = (CharacterCardDetails)details;
                    string classes = "";
                    foreach (CharacterClassEnum classEnum in charDetails.classes)
                        classes += Localize(classEnum.ToString(), lan) + ", ";
                    text += string.Format("{0} {1} {2} {3} {4} \"{5}\"",
                        charDetails.subRace != SubRacesEnum.None ? Localize(charDetails.race.ToString()) + " " + Localize(charDetails.subRace.ToString()) : Localize(charDetails.race.ToString()),
                        classes,
                        Localize("character_from", lan),
                        Localize(charDetails.GetHomeTown(), lan),
                        Localize("called", lan),
                        Localize(details.cardId, lan));
                    break;
                case CardClass.Object:
                    ObjectCardDetails objectCardDetails = (ObjectCardDetails)details;
                    text += string.Format("{0} {1} \"{2}\"",
                            Localize(objectCardDetails.objectSlot != ObjectType.Jewelry ? objectCardDetails.objectSlot.ToString() : "object", lan),
                            Localize("called", lan),
                            Localize(details.cardId, lan));
                    break;
                case CardClass.Faction:
                    text += string.Format("{0} \"{1}\"",
                            Localize("group_of_allies_called", lan),
                            Localize(details.cardId));
                    break;
                case CardClass.Event:
                    text += string.Format("{0} \"{1}\"",
                            Localize("event_happening_in_middle_earth_called", lan),
                            Localize(details.cardId));
                    break;
                case CardClass.HazardEvent:
                    text += string.Format("{0} \"{1}\"",
                            Localize("adversity_happening_in_middle_earth_called", lan),
                            Localize(details.cardId));
                    break;
                case CardClass.HazardCreature:
                    HazardCreatureCardDetails hazardCardDetails = (HazardCreatureCardDetails)details;
                    text += string.Format("{0} {1} {2} \"{3}\"",
                            Localize("group_of_", lan),
                            Localize(hazardCardDetails.race.ToString(), lan),
                            Localize("foes_called", lan),
                            Localize(details.cardId));
                    break;
                case CardClass.Ally:
                    AllyCardDetails allyDetails = (AllyCardDetails)details;
                    text += string.Format("{0} {1} {2} {3} {4} \"{5}\"",
                        allyDetails.GetSubRace() != SubRacesEnum.None ? Localize(allyDetails.GetRace().ToString()) + " " + Localize(allyDetails.GetSubRace().ToString()) : Localize(allyDetails.GetRace().ToString()),
                        Localize(allyDetails.GetClass().ToString(), lan),
                        Localize("character_from", lan),
                        Localize(allyDetails.GetHomeTown(), lan),
                        Localize("called", lan),
                        Localize(details.cardId, lan));
                    break;
                case CardClass.GoldRing:
                    text += string.Format("{0} \"{1}\"",
                            Localize("misterious_gold_ring_called", lan),
                            Localize(details.cardId));
                    break;
                case CardClass.Ring:
                    RingCardDetails ringDetails = (RingCardDetails)details;
                    text += string.Format("{0} {1} \"{2}\"",
                        Localize(ringDetails.objectSlot.ToString(), lan),
                        Localize("called", lan),
                        Localize(details.cardId, lan));
                    break;
                case CardClass.NONE:
                    text = "";
                    break;
            }

            if (!string.IsNullOrEmpty(details.descForAIGeneration) && !string.IsNullOrEmpty(text))
                text += string.Format(": {0}", details.descForAIGeneration);
        }
        else
        {
            text = "";
            Debug.LogError("Dictionary " + lan + " not found");
        }
        return text;
    }

    public string CreateDescriptionForGeneration(CityUI city, string lan = "en-US")
    {
        string text = Localize("a_place_in_the_middle_earth_region_of", lan) + " ";
        if (locales.ContainsKey(lan))
        {
            text += string.Format("{0} {1} \"{2}\"",
                Localize(city.GetRegion().ToString(), lan),
                Localize("called", lan),
                Localize(city.GetCityId(), lan));

            if(!string.IsNullOrEmpty(city.GetDescForAI()))
                text += string.Format(": {0}", city.GetDescForAI());
        }
        else
        {
            text = "";
            Debug.LogError("Dictionary " + lan + " not found");
        }
        return text;
    }

    public bool IsInitialized()
    {
        return initialized;
    }
}

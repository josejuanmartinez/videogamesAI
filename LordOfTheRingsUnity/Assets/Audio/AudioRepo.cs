using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioRepo : MonoBehaviour
{
    [Header("Audios By String")]
    [SerializeField]
    private List<string> audioIds;
    [SerializeField]
    private List<AudioResource> audioResources;
    [Header("Sound by Terrain")]
    [SerializeField]
    private List<TerrainsEnum> terrains;
    [SerializeField]
    private List<AudioResource> terrainAudioResources;
    [Header("Music by Terrain")]
    [SerializeField]
    private List<AudioResource> terrainMusicResources;

    [Header("Voices")]
    [SerializeField]
    private List<AudioResource> maleVoices;
    [SerializeField]
    private List<AudioResource> femaleVoices;
    [SerializeField]
    private List<AudioResource> orcVoices;
    [SerializeField]
    private List<AudioResource> demonVoices;
    [SerializeField]
    private List<AudioResource> undeadVoices;
    [SerializeField]
    private List<AudioResource> wolfVoices;
    [SerializeField]
    private List<AudioResource> weatherVoices;
    [SerializeField]
    private List<AudioResource> trollVoices;
    [SerializeField]
    private List<AudioResource> bearVoices;
    [SerializeField]
    private List<AudioResource> plantVoices;
    [SerializeField]
    private List<AudioResource> spiderVoices;
    [SerializeField]
    private List<AudioResource> otherAnimalsVoices;
    [SerializeField]
    private List<AudioResource> machineryVoices;
    [SerializeField]
    private List<AudioResource> nazgulVoices;

    [Header("Music")]
    [SerializeField]
    private List<AudioResource> freeMusic;
    [SerializeField]
    private List<AudioResource> evilMusic;
    [SerializeField]
    private List<AudioResource> neutralMusic;

    [Header("Event Music")]
    [SerializeField]
    private List<AudioResource> freeCombatMusic;
    [SerializeField]
    private List<AudioResource> evilCombatMusic;
    [SerializeField]
    private List<AudioResource> freeEventMusic;
    [SerializeField]
    private List<AudioResource> evilEventMusic;

    public AudioResource GetMusic(AlignmentsEnum alignment)
    {
        switch (alignment)
        {
            case AlignmentsEnum.FREE_PEOPLE:
                return freeMusic[Random.Range(0, freeMusic.Count)];
            case AlignmentsEnum.CHAOTIC:
            case AlignmentsEnum.DARK_SERVANTS:
                return evilMusic[Random.Range(0, evilMusic.Count)];
            case AlignmentsEnum.RENEGADE:
            case AlignmentsEnum.NEUTRAL:
                return neutralMusic[Random.Range(0, neutralMusic.Count)];
        }
        return null;
    }

    public AudioResource GetEventMusic(AlignmentsEnum alignment, bool isCombat)
    {
        AudioResource res = null;
        List<AudioResource> music = null;
        switch (alignment)
        {
            case AlignmentsEnum.FREE_PEOPLE:
                if (isCombat)
                    music = freeCombatMusic;
                else
                    music = freeEventMusic;
                break;
            case AlignmentsEnum.CHAOTIC:
            case AlignmentsEnum.DARK_SERVANTS:
                if (isCombat)
                    music = evilCombatMusic;
                else
                    music = evilEventMusic;
                break;
            case AlignmentsEnum.RENEGADE:
            case AlignmentsEnum.NEUTRAL:
            case AlignmentsEnum.NONE:
                if (isCombat)
                    music = evilCombatMusic;
                else
                    music = freeEventMusic;
                break;
        }
        if (music != null && music.Count > 0)
            res = music[Random.Range(0, music.Count)];
        return res;
    }

    public AudioResource GetAudio(string id)
    {
        if(string.IsNullOrEmpty(id)) 
            return null;
        int index = audioIds.IndexOf(id);
        if (index != -1)
            return audioResources[index];
        else
            return null;
    }


    public AudioResource GetAudio(TerrainsEnum terrain)
    {
        int index = terrains.IndexOf(terrain);
        if (index != -1)
            return terrainAudioResources[index];
        else
            return null;
    }

    public AudioResource GetVoice(RacesEnum race, AlignmentsEnum alignment, bool isFemale = false)
    {
        List<AudioResource> res = null;

        switch (race)
        {
            case RacesEnum.Man:
            case RacesEnum.Dwarf:
            case RacesEnum.Elf:
            case RacesEnum.Hobbit:
            case RacesEnum.Dunadan:
            case RacesEnum.Wizard:
            case RacesEnum.FallenWizard:
            case RacesEnum.Maia:
            case RacesEnum.Beorning:
                res = isFemale ? femaleVoices : maleVoices;
                break;
            case RacesEnum.Undead:
                res = undeadVoices;
                break;
            case RacesEnum.Orc:
                res = orcVoices;
                break;
            case RacesEnum.Giant:
            case RacesEnum.Troll:
                res = trollVoices;
                break;
            case RacesEnum.Ringwraith:
                res = nazgulVoices;
                break;
            case RacesEnum.Balrog:
            case RacesEnum.Dragon:
                res = demonVoices;
                break;
            case RacesEnum.Wolf:
                res = wolfVoices;
                break;
            case RacesEnum.OtherAnimals:
                res = otherAnimalsVoices;
                break;
            case RacesEnum.Machinery:
                res = machineryVoices;
                break;
            case RacesEnum.Spider:
                res = spiderVoices;
                break;
            case RacesEnum.Plant:
                res = plantVoices;
                break;
            case RacesEnum.Bear:
                res = bearVoices;
                break;         
            case RacesEnum.Weather:
                res = weatherVoices;
                break;
        }
        if (res == null)
            return null;
        if (res.Count < 1)
            return null;

        return res[Random.Range(0, res.Count - 1)];
    }


    public AudioResource GetTerrainMusic(TerrainsEnum terrain)
    {
        int index = terrains.IndexOf(terrain);
        if (index != -1)
            return terrainMusicResources[index];
        else
            return null;
    }
}

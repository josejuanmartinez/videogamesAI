using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioRepo : MonoBehaviour
{
    [SerializeField]
    private List<string> audioIds;
    [SerializeField]
    private List<AudioResource> audioResources;
    [SerializeField]
    private List<AudioResource> freeMaleVoices;
    [SerializeField]
    private List<AudioResource> evilMaleVoices;
    [SerializeField]
    private List<AudioResource> freeFemaleVoices;
    [SerializeField]
    private List<AudioResource> evilFemaleVoices;
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
                if (isFemale)
                {
                    switch (alignment)
                    {
                        case AlignmentsEnum.FREE_PEOPLE:
                        case AlignmentsEnum.NEUTRAL:
                            res = freeFemaleVoices;
                            break;
                        case AlignmentsEnum.DARK_SERVANTS:
                        case AlignmentsEnum.RENEGADE:
                        case AlignmentsEnum.CHAOTIC:
                            res = evilFemaleVoices;
                            break;
                    }                    
                }
                else
                {
                    switch (alignment)
                    {
                        case AlignmentsEnum.FREE_PEOPLE:
                        case AlignmentsEnum.NEUTRAL:
                            res = freeMaleVoices;
                            break;
                        case AlignmentsEnum.DARK_SERVANTS:
                        case AlignmentsEnum.RENEGADE:
                        case AlignmentsEnum.CHAOTIC:
                            res = evilMaleVoices;
                            break;
                    }
                }
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
}

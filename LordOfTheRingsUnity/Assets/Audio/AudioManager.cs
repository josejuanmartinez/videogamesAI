using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource soundSource;
    [SerializeField]
    private AudioSource terrainMusicSource;

    private Game game;
    private AudioRepo audioRepo;

    private bool isInGame;
    void Awake()
    {
        isInGame = false;
        if (GameObject.Find("Game"))
        {
            game = GameObject.Find("Game").GetComponent<Game>();
            isInGame = true;
        }
            
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
    }

    public void RandomizeMusic()
    {
        if(isInGame)
        {
            // GAME
            if (!game.FinishedLoading())
                return;
            
            AudioResource music = audioRepo.GetMusic(Nations.alignments[game.GetHumanNation()]);
            if (music != null)
            {
                musicSource.resource = music;
                musicSource.Play();
            }
        }
        else
        {
            // MENU
            AudioResource music = audioRepo.GetMusic(AlignmentsEnum.NEUTRAL);
            if (music != null)
            {
                musicSource.resource = music;
                musicSource.Play();
            }
        }            
    }

    public void PlayMusic(AudioResource music)
    {
        if (music != null)
        {
            musicSource.resource = music;
            musicSource.Play();
        }
    }

    void Update()
    {
        if (!musicSource.isPlaying)
            RandomizeMusic();
    }

    public void PlaySound(AudioResource audio)
    {
        if (audio == null)
            return;
        if(soundSource.resource != audio) 
            soundSource.resource = audio;
        //if (!soundSource.isPlaying)
        soundSource.Play();
    }

    public void StopSound(AudioResource audio)
    {
       if (audio == soundSource.resource)
            soundSource.Stop();
    }

    public void PlayEventMusic(bool isCombat)
    {
        AudioResource eventMusic = audioRepo.GetEventMusic(Nations.alignments[game.GetHumanNation()], isCombat);
        if (eventMusic != null)
        {
            musicSource.resource = eventMusic;
            //if(!musicSource.isPlaying)
                musicSource.Play();
        }
    }

    public void PlayTerrainMusic(AudioResource audio)
    {
        if(audio != null)
        {
            terrainMusicSource.resource = audio;
            //if (!terrainMusicSource.isPlaying)
                terrainMusicSource.Play();
        }
    }
}

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
    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
    }

    public void RandomizeMusic()
    {
        AudioResource music = audioRepo.GetMusic(Nations.alignments[game.GetHumanNation()]);
        if (music != null)
        {
            musicSource.resource = music;
            musicSource.Play();
        }
            
    }

    void Update()
    {
        if (game.FinishedLoading() && !musicSource.isPlaying)
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

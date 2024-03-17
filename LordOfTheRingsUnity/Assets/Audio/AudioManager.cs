using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioResource> freeMusic;
    [SerializeField]
    private List<AudioResource> evilMusic;
    [SerializeField]
    private List<AudioResource> neutralMusic;
    [SerializeField]
    private List<AudioResource> chaoticMusic;
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource soundSource;

    private Game game;
    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    void RandomizeMusic()
    {
        switch (Nations.alignments[game.GetHumanNation()])
        {
            case AlignmentsEnum.FREE_PEOPLE:
                musicSource.resource = freeMusic[Random.Range(0, freeMusic.Count)];
                break;
            case AlignmentsEnum.DARK_SERVANTS:
                musicSource.resource = evilMusic[Random.Range(0, evilMusic.Count)];
                break;
            case AlignmentsEnum.RENEGADE:
            case AlignmentsEnum.NEUTRAL:
                musicSource.resource = neutralMusic[Random.Range(0, neutralMusic.Count)];
                break;
            case AlignmentsEnum.CHAOTIC:
                musicSource.resource = chaoticMusic[Random.Range(0, chaoticMusic.Count)];
                break;
        }
        
        musicSource.Play();
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
        if (!soundSource.isPlaying)
            soundSource.Play();
    }

    public void StopSound(AudioResource audio)
    {
        if (audio == null)
            return;

        //if (soundSource.resource == audio)
        //    StartCoroutine(StopSoundCoroutine());
    }

    IEnumerator StopSoundCoroutine()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        soundSource.Stop();
        soundSource.resource = null;
    }

}

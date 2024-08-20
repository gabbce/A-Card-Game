using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;

    [SerializeField] private AudioSource audioEffectsSource;
    [SerializeField] private AudioSource musicSource;
    
    [SerializeField] private AudioClip[] cardSounds;
    [SerializeField] private AudioClip[] gameSounds;
    [SerializeField] private AudioClip[] enemySounds;
    [SerializeField] private AudioClip[] musicClips;


    void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(this);
            musicSource.clip = musicClips[2];
            musicSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "MainMenu")
        {
            musicSource.clip = musicClips[2];
            musicSource.Play();
        }
        else if(scene.name == "RunNavigator")
        {
            musicSource.clip = musicClips[1];
            musicSource.Play();
        }
        else if(scene.name == "EnemyStage1" || scene.name == "Rest")
        {
            musicSource.clip = musicClips[0];
            musicSource.Play();
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayCardSound(int sound, float volume)
    {
        audioEffectsSource.PlayOneShot(cardSounds[sound], volume);
    }
    public void PlayGameSound(int sound, float volume)
    {
        audioEffectsSource.PlayOneShot(gameSounds[sound], volume);
    }

    public void PlayEnemySound(int sound, float volume)
    {
        audioEffectsSource.PlayOneShot(enemySounds[sound], volume);
    }
}

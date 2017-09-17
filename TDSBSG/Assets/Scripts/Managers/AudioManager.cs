using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    Toolbox toolbox;
    EventManager em;

    public Sound[] globalSounds;
    private List<SoundEffect> soundEffects = new List<SoundEffect>();

    int mainMenuIndex = -1;
    int firstLevelIndex = -1;
    int lastLevelIndex = -1;

    [Range(0f, 1f)]
    public float volumeMultiplier = 0.5f;
    float lastVolumeMultiplier = 0.5f;

    string currentSceneMusic = "MainMenu_Music"; //Initialize as the first scene music to play
    bool delaySceneMusicStart = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        lastVolumeMultiplier = volumeMultiplier;

        int count = globalSounds.Length;
        for (int i = 0; i < count; i++)
        {
            Sound s = globalSounds[i];

            s._source = gameObject.AddComponent<AudioSource>();
            s._source.clip = s._clip;

            s._source.volume = s._volume * volumeMultiplier;
            s._source.pitch = s._pitch;

            s._source.loop = s._loop;
            s._source.spatialBlend = 0f;

        }

        SceneManager.sceneLoaded += OnLevelFinishedLoading;

        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();

        em.OnRequestAudio += OnRequestAudio;
        em.OnRegisterSoundEffect += OnRegisterSoundEffect;
        em.OnLoadingScreenStateChange += OnLoadingScreenStateChange;
        em.OnVolumeSliderValueChange += OnVolumeSliderValueChange;
        em.OnRequestVolumeLevel += OnRequestVolumeLevel;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;

        em.OnRequestAudio -= OnRequestAudio;
        em.OnRegisterSoundEffect -= OnRegisterSoundEffect;
        em.OnLoadingScreenStateChange -= OnLoadingScreenStateChange;
        em.OnVolumeSliderValueChange -= OnVolumeSliderValueChange;
        em.OnRequestVolumeLevel -= OnRequestVolumeLevel;
    }

    private void OnLoadingScreenStateChange(bool isOpen)
    {
        if (isOpen)
        {
            delaySceneMusicStart = true;
            PlayAudioExclusive("LoadingScreen_Music");
        }
        else
        {
            delaySceneMusicStart = false;
            PlayAudioExclusive(currentSceneMusic);
        }
    }

    private void OnVolumeSliderValueChange(float newValue)
    {
        volumeMultiplier = newValue;
        lastVolumeMultiplier = volumeMultiplier;

        UpdateVolumeLevels();
    }

    private float OnRequestVolumeLevel()
    {
        return volumeMultiplier;
    }

    private void OnRegisterSoundEffect(SoundEffect newSoundEffect)
    {
        soundEffects.Add(newSoundEffect);
    }

    private void OnRequestAudio(string soundName)
    {
        PlayAudio(soundName);
    }

    private void PlayAudio(string soundName)
    {
        Sound s = Array.Find(globalSounds, sound => sound._name == soundName);
        if (s == null)
        {
            Debug.LogWarning("No sound found with name '" + soundName + "'!");
            return;
        }

        s._source.Play();
    }

    private void PlayAudioExclusive(string soundName)
    {
        bool soundFound = false;
        int count = globalSounds.Length;
        for (int i = 0; i < count; i++)
        {
            if (globalSounds[i]._name == soundName)
            {
                soundFound = true;
                globalSounds[i]._source.Play();
            }
            else
            {
                globalSounds[i]._source.Stop();
            }
        }

        if (!soundFound)
        {
            Debug.LogWarning("No sound found with name '" + soundName + "'!");
        }
    }

    private void StopAudio(string soundName)
    {
        Sound s = Array.Find(globalSounds, sound => sound._name == soundName);
        if (s == null)
        {
            Debug.LogWarning("No sound found with name '" + soundName + "'!");
            return;
        }

        s._source.Stop();
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (mainMenuIndex == -1 || firstLevelIndex == -1 || lastLevelIndex == -1)
        {
            Vector3 sceneIndices = em.BroadcastRequestSceneIndices();
            mainMenuIndex = (int)sceneIndices.x;
            firstLevelIndex = (int)sceneIndices.y;
            lastLevelIndex = (int)sceneIndices.z;
        }

        if (scene.buildIndex == mainMenuIndex)
        {
            currentSceneMusic = "MainMenu_Music";
            if (!delaySceneMusicStart)
            {
                PlayAudioExclusive(currentSceneMusic);
            }
        }
        else if (scene.buildIndex == firstLevelIndex)
        {
            currentSceneMusic = "Level01_Music";
            if (!delaySceneMusicStart)
            {
                PlayAudioExclusive(currentSceneMusic);
            }
        }
        else if (scene.buildIndex > firstLevelIndex && scene.buildIndex < lastLevelIndex)
        {
            currentSceneMusic = "Level02_Music";
            if (!delaySceneMusicStart)
            {
                PlayAudioExclusive(currentSceneMusic);
            }
        }
        else if (scene.buildIndex == lastLevelIndex)
        {
            currentSceneMusic = "Level03_Music";
            if (!delaySceneMusicStart)
            {
                PlayAudioExclusive(currentSceneMusic);
            }
        }
    }

    private void UpdateVolumeLevels()
    {
        int count = globalSounds.Length;
        for (int i = 0; i < count; i++)
        {
            Sound s = globalSounds[i];
            s._source.volume = s._volume * lastVolumeMultiplier;

        }

        count = soundEffects.Count;
        for (int i = 0; i < count; i++)
        {
            SoundEffect s = soundEffects[i];
            s._source.volume = s._volume * lastVolumeMultiplier;

        }
    }

    private void Update()
    {
        if (volumeMultiplier != lastVolumeMultiplier)
        {
            lastVolumeMultiplier = volumeMultiplier;

            UpdateVolumeLevels();
        }
    }
}

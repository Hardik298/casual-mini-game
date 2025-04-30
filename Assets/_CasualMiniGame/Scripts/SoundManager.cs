using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Singleton class responsible for managing music and sound effects using pooling and an external config.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSourcePrefab;

    [Header("Audio Settings")]
    [Range(0, 1)] [SerializeField] private float musicVolume = 1f;
    [Range(0, 1)] [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private bool isSfxMuted = false;

    [Header("Pooling Settings")]
    [SerializeField] private int sfxPoolSize = 5;

    [Header("SFX Configuration")]
    [SerializeField] private SFXDatabase sfxDatabase;

    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

    private void Awake()
    {
        // Ensure a single persistent instance (singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            sfxDatabase.Initialize();
            InitializeSFXPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource newSFX = Instantiate(sfxSourcePrefab, transform);
            newSFX.playOnAwake = false;
            newSFX.volume = sfxVolume;
            sfxPool.Enqueue(newSFX);
        }
    }

    public void PlaySFX(SFXType type)
    {
        if (isSfxMuted) return;

        AudioClip clip = sfxDatabase.GetClip(type);
        if (clip == null) return;

        AudioSource source = GetPooledSFXSource();
        source.clip = clip;
        source.volume = sfxVolume;
        source.Play();

        StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
    }

    private AudioSource GetPooledSFXSource()
    {
        return sfxPool.Count > 0 ? sfxPool.Dequeue() : Instantiate(sfxSourcePrefab, transform);
    }

    private IEnumerator ReturnToPoolAfterPlay(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        sfxPool.Enqueue(source);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    public void SetSFXMute(bool mute) => isSfxMuted = mute;

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}
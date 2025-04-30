using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Singleton class responsible for managing music and sound effects (SFX) in the game.
/// This class utilizes pooling for efficient handling of SFX playback and external configuration for flexibility.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // AudioSource for background music.
    [SerializeField] private AudioSource sfxSourcePrefab; // Prefab used to create AudioSource objects for SFX.

    [Header("Audio Settings")]
    [Range(0, 1)] [SerializeField] private float musicVolume = 1f; // Volume level for music.
    [Range(0, 1)] [SerializeField] private float sfxVolume = 1f; // Volume level for sound effects (SFX).
    [SerializeField] private bool isSfxMuted = false; // Mute status for SFX. If true, SFX will not play.

    [Header("Pooling Settings")]
    [SerializeField] private int sfxPoolSize = 5; // Size of the SFX audio source pool for efficient reuse.

    [Header("SFX Configuration")]
    [SerializeField] private SFXDatabase sfxDatabase; // Database holding mappings between SFX types and their AudioClips.

    private Queue<AudioSource> sfxPool = new Queue<AudioSource>(); // Pool of AudioSource objects to manage SFX playback efficiently.

    private void Awake()
    {
        // Ensure a single persistent instance (singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            sfxDatabase.Initialize(); // Initialize the SFX database.
            InitializeSFXPool(); // Initialize the SFX pool for efficient sound playback.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes the SFX pool with a specified number of AudioSource objects.
    /// These objects are used to play sound effects and are pooled for reuse.
    /// </summary>
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

    /// <summary>
    /// Plays a sound effect (SFX) based on the specified type.
    /// It retrieves the corresponding AudioClip from the SFX database and plays it.
    /// </summary>
    /// <param name="type">The type of sound effect to play (e.g., card flip, match, etc.).</param>
    public void PlaySFX(SFXType type)
    {
        if (isSfxMuted) return;

        AudioClip clip = sfxDatabase.GetClip(type);
        if (clip == null) return;

        AudioSource source = GetPooledSFXSource(); // Get an available AudioSource from the pool or create a new one if necessary.
        source.clip = clip;
        source.volume = sfxVolume;
        source.Play();

        StartCoroutine(ReturnToPoolAfterPlay(source, clip.length)); // Return the AudioSource to the pool after the sound effect finishes playing.
    }

    /// <summary>
    /// Retrieves an available AudioSource from the pool.
    /// If no AudioSource is available, a new one is instantiated.
    /// </summary>
    /// <returns>An AudioSource that can be used to play an SFX.</returns>
    private AudioSource GetPooledSFXSource()
    {
        return sfxPool.Count > 0 ? sfxPool.Dequeue() : Instantiate(sfxSourcePrefab, transform);
    }

    /// <summary>
    /// Coroutine that returns an AudioSource to the pool after a sound effect has finished playing.
    /// </summary>
    /// <param name="source">The AudioSource that was used to play the SFX.</param>
    /// <param name="duration">The duration of the sound effect (in seconds).</param>
    /// <returns>An IEnumerator used to handle the return operation after the delay.</returns>
    private IEnumerator ReturnToPoolAfterPlay(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        sfxPool.Enqueue(source);
    }

    // <summary>
    /// Plays the specified background music clip.
    /// If the same clip is already playing, it does nothing.
    /// </summary>
    /// <param name="clip">The AudioClip to play as background music.</param>
    /// <param name="loop">Whether or not to loop the music. Defaults to true.</param>
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    /// <summary>
    /// Stops the currently playing background music.
    /// </summary>
    public void StopMusic() => musicSource.Stop();

    /// <summary>
    /// Mutes or unmutes the sound effects.
    /// </summary>
    public void SetSFXMute(bool mute) => isSfxMuted = mute;

    /// <summary>
    /// Sets the volume for background music.
    /// </summary>
    /// <param name="volume">The volume level to set, between 0 and 1.</param>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    /// <summary>
    /// Sets the volume for sound effects (SFX).
    /// </summary>
    /// <param name="volume">The volume level to set, between 0 and 1.</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}
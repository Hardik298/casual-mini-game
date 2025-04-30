using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores mappings between different Sound Effect (SFX) types and their associated AudioClips.
/// This allows easy retrieval of the appropriate SFX for various game events. The SFX database can be easily extended 
/// through the Unity Inspector by adding new entries to the list.
/// </summary>
[CreateAssetMenu(fileName = "SFXDatabase", menuName = "Audio/SFX Database")]
public class SFXDatabase : ScriptableObject
{
    /// <summary>
    /// Represents a single entry in the SFX database, consisting of an SFX type and the corresponding AudioClip.
    /// </summary>
    [System.Serializable]
    public class SFXEntry
    {
        public SFXType type; // This is used to identify which sound to play.
        public AudioClip clip; // The AudioClip that corresponds to the SFX type.
    }

    [SerializeField] private List<SFXEntry> sfxEntries; // A list of all available SFX entries (type + clip) to be defined in the Inspector.

    private Dictionary<SFXType, AudioClip> sfxMap; // A dictionary for fast lookup of audio clips by their SFX type.

    /// <summary>
    /// Initializes the SFX database by converting the list of SFX entries into a dictionary.
    /// This ensures fast lookup by SFX type for efficient runtime usage.
    /// Should be called when the SFXDatabase is first used.
    /// </summary>
    public void Initialize()
    {
        sfxMap = new Dictionary<SFXType, AudioClip>();
        foreach (var entry in sfxEntries)
        {
            if (!sfxMap.ContainsKey(entry.type))
            {
                sfxMap[entry.type] = entry.clip;
            }
        }
    }

    /// <summary>
    /// Retrieves the AudioClip for the specified SFX type.
    /// If the type doesn't exist in the database, it returns null.
    /// </summary>
    /// <param name="type">The type of SFX to retrieve.</param>
    /// <returns>The AudioClip corresponding to the given SFX type, or null if not found.
    public AudioClip GetClip(SFXType type)
    {
        return sfxMap != null && sfxMap.TryGetValue(type, out var clip) ? clip : null;
    }
}
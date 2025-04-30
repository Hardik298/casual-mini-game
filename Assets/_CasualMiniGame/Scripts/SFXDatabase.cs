using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Stores mappings between SFX types and their AudioClips.
/// Easy to extend via Inspector.
/// </summary>
[CreateAssetMenu(fileName = "SFXDatabase", menuName = "Audio/SFX Database")]
public class SFXDatabase : ScriptableObject
{
    [System.Serializable]
    public class SFXEntry
    {
        public SFXType type;
        public AudioClip clip;
    }

    [SerializeField] private List<SFXEntry> sfxEntries;

    private Dictionary<SFXType, AudioClip> sfxMap;

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

    public AudioClip GetClip(SFXType type)
    {
        return sfxMap != null && sfxMap.TryGetValue(type, out var clip) ? clip : null;
    }
}
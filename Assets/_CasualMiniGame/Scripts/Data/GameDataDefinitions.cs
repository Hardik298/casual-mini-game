using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Central definitions for game data models.
/// Keeps all data classes organized.
/// </summary>
public static class GameDataDefinitions
{
    // Key used to store and retrieve the selected game difficulty level from PlayerPrefs.
    public const string GAME_DIFFICULTY = "PLAYER_PREF_DIFFICULTY";

    // Key used to store and retrieve the selected game difficulty level from PlayerPrefs.
    public const string GAME_SAVE_FILE = "PLAYER_PREF_GAME_SAVE_FILE";
}

/// <summary>
/// Represents different possible layout grid types for the game board.
/// The EnumFlags attribute will allow multiple layouts to be selected.
/// </summary>
[System.Flags]
[System.Serializable]
public enum LayoutType
{
    None = 0,
    Layout2x2 = 1 << 0, // 1
    Layout2x3 = 1 << 1, // 2
    Layout2x4 = 1 << 2, // 4
    Layout4x4 = 1 << 3, // 8
    Layout5x6 = 1 << 4, // 16
}

/// <summary>
/// ScriptableObject that stores all information about a card category.
/// </summary>
[CreateAssetMenu(fileName = "CardCategoryData", menuName = "CardGame/CardCategoryData")]
public class CardCategoryData : ScriptableObject
{
    [Tooltip("Name of the category, e.g., Animals, Trees.")]
    public string CategoryName;

    [Tooltip("Sprite shown on the back of the cards.")]
    public Sprite CardBackSprite;

    [Tooltip("Sprites shown on the front of the cards.")]
    public List<Sprite> CardFrontSprites;

    [Tooltip("Allowed layouts for this category.")]
    public LayoutType AllowedLayouts;

    [Tooltip("Spacing between cards (in pixels).")]
    public Vector2 CardSpacing;

    public Sprite GetSpriteByName(string spriteName)
    {
        return CardFrontSprites.FirstOrDefault(sprite => sprite.name == spriteName);
    }
}

/// <summary>
/// Serializable data class that holds the persistent state of the game.
/// </summary>
[System.Serializable]
public class GameData
{
    public LayoutType selectedLayout;
    public string selectedCategoryName;
    public int turnCount;
    public int matchCount;
    public int totalScore;
    public int currentCombo;

    public List<string> spawnedCardIds = new List<string>();
    public List<string> waitingCardIds = new List<string>();
    public List<string> matchedCardIds = new List<string>();
}

/// <summary>
/// Defines a contract for game save and load functionality.
/// </summary>
public interface ISaveService
{
    void SaveGame(GameData data);
    GameData LoadGame();
    bool HasSavedGame();
    void DeleteSavedGame();
}

/// <summary>
/// Contains types of sfxs.
/// It's extendable by adding more sfx types here and set it in SFXDatabase.
/// </summary>
public enum SFXType
{
    CardFlip,
    Match,
    Mismatch,
    GameOver
}

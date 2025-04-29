using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central definitions for game data types like categories, layouts, etc.
/// Keeps all data classes organized.
/// </summary>
public static class GameDataDefinitions
{
    /// <summary>
    /// Represents different possible layout grid types for the game board.
    /// The EnumFlags attribute will allow multiple layouts to be selected.
    /// </summary>
    [System.Flags]
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
    }
}
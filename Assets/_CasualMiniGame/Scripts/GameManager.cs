using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the overall game flow: initialization, setup, and state transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Board Settings")]
    [Tooltip("Parent container where card instances will be placed.")]
    [SerializeField] private RectTransform boardParent;

    [Tooltip("Prefab to instantiate each card.")]
    [SerializeField] private Card cardPrefab;

    [Tooltip("Spacing between cards (in pixels).")]
    [SerializeField] private Vector2 cardSpacing = new Vector2(10f, 10f);

    private GameDataDefinitions.LayoutType selectedLayout;
    private GameDataDefinitions.CardCategoryData selectedCategory;

    private List<Card> spawnedCards = new List<Card>();

    private int rows;
    private int columns;

    private void Start()
    {
        //For Testing
        selectedLayout = GameDataDefinitions.LayoutType.Layout2x2;
        selectedCategory = CategoryManager.Instance.GetRandomCategoryForLayout(selectedLayout);

        InitializeBoard();
    }

    /// <summary>
    /// Initializes the board based on selected category and layout.
    /// </summary>
    private void InitializeBoard()
    {
        // Safety Check
        if (selectedCategory == null || cardPrefab == null || boardParent == null)
        {
            Debug.LogError("Missing references in GameManager!");
            return;
        }

        // Get layout size (rows, columns) based on layout enum
        GetLayoutSize(selectedLayout);

        int totalCardsNeeded = rows * columns;

        // Ensure even number of cards
        if (totalCardsNeeded % 2 != 0)
        {
            Debug.LogError("Total cards should be an even number for matching!");
            return;
        }

        // Total required pairs
        int pairCount = totalCardsNeeded / 2;

        // Generate card list
        List<Sprite> selectedSprites = GenerateRandomSpritePairs(pairCount);

        // Shuffle the sprite list
        Shuffle(selectedSprites);

        // Instantiate cards
        for (int i = 0; i < selectedSprites.Count; i++)
        {
            Card newCard = Instantiate(cardPrefab, boardParent);
            newCard.InitializeCard(selectedSprites[i], selectedCategory.CardBackSprite);
            spawnedCards.Add(newCard);
        }

        // Arrange cards in grid
        ArrangeCardsInGrid();
    }

    /// <summary>
    /// Determines the number of rows and columns based on the selected layout.
    /// </summary>
    private void GetLayoutSize(GameDataDefinitions.LayoutType layout)
    {
        switch (layout)
        {
            case GameDataDefinitions.LayoutType.Layout2x2:
                rows = 2;
                columns = 2;
                break;
            case GameDataDefinitions.LayoutType.Layout2x3:
                rows = 2;
                columns = 3;
                break;
            case GameDataDefinitions.LayoutType.Layout2x4:
                rows = 2;
                columns = 4;
                break;
            case GameDataDefinitions.LayoutType.Layout4x4:
                rows = 4;
                columns = 4;
                break;
            case GameDataDefinitions.LayoutType.Layout5x6:
                rows = 5;
                columns = 6;
                break;
            default:
                Debug.LogError("Unsupported layout type.");
                break;
        }
    }

    /// <summary>
    /// Generates a list containing pairs of randomly selected sprites.
    /// </summary>
    private List<Sprite> GenerateRandomSpritePairs(int pairCount)
    {
        List<Sprite> availableSprites = new List<Sprite>(selectedCategory.CardFrontSprites);
        List<Sprite> selectedPairs = new List<Sprite>();

        for (int i = 0; i < pairCount; i++)
        {
            if (availableSprites.Count == 0)
            {
                Debug.LogError("Not enough unique sprites.");
            }

            int randomIndex = Random.Range(0, availableSprites.Count);
            Sprite selectedSprite = availableSprites[randomIndex];

            selectedPairs.Add(selectedSprite); // First card
            selectedPairs.Add(selectedSprite); // Matching card

            availableSprites.RemoveAt(randomIndex);
        }

        return selectedPairs;
    }

    /// <summary>
    /// Shuffles the list randomly using Fisher-Yates algorithm.
    /// </summary>
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    /// <summary>
    /// Arranges spawned cards into a grid inside the boardParent.
    /// </summary>
    private void ArrangeCardsInGrid()
    {
        GridLayoutGroup gridLayout = boardParent.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = boardParent.gameObject.AddComponent<GridLayoutGroup>();
        }

        Vector2 parentSize = boardParent.rect.size;

        // Calculate card size based on parent size, rows, columns, and spacing
        float cardWidth = (parentSize.x - (columns - 1) * cardSpacing.x) / columns;
        float cardHeight = (parentSize.y - (rows - 1) * cardSpacing.y) / rows;

        gridLayout.cellSize = new Vector2(cardWidth, cardHeight);
        gridLayout.spacing = cardSpacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
    }
}
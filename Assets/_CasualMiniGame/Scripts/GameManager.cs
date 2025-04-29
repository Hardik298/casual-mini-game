using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Initializes and manages the game board layout and delegates card flipping to the match controller.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Board Settings")]
    [Tooltip("Parent container where card instances will be placed.")]
    [SerializeField] private RectTransform boardParent;

    [Tooltip("Prefab to instantiate each card.")]
    [SerializeField] private Card cardPrefab;

    [Tooltip("Duration in seconds to show all cards at game start before flipping them back.")]
    [SerializeField] private float previewDuration = 2f;

    [Header("Managers")]
    [SerializeField] private CardMatchController matchController;
    [SerializeField] private ScoreManager scoreManager;

    private LayoutType selectedLayout;
    private CardCategoryData selectedCategory;

    private List<Card> spawnedCards = new List<Card>();

    private int rows;
    private int columns;

    private void Start()
    {
        //For Testing
        //selectedLayout = GameDataDefinitions.LayoutType.Layout5x6;
        //selectedCategory = CategoryManager.Instance.GetRandomCategoryForLayout(selectedLayout);

        scoreManager.ResetScore();
        InitializeBoard();

        StartCoroutine(PreviewCardsThenPlay());
    }

    /// <summary>
    /// Initializes the board based on selected category and layout.
    /// </summary>
    private void InitializeBoard()
    {
        // Retrieve the saved difficulty level (1 to 5), defaulting to 1 if not set
        int difficultyLevel = PlayerPrefs.GetInt(GameDataDefinitions.GAME_DIFFICULTY, 1);

        // Map difficulty level to a single LayoutType using bit shift (1 << (difficultyLevel - 1))
        LayoutType selectedLayoutType = (LayoutType)(1 << (difficultyLevel - 1));
        Debug.Log($"Selected Layout: {selectedLayoutType}");

        // Store the layout for use in board generation
        selectedLayout = selectedLayoutType;

        // Fetch a random category that supports the selected layout
        selectedCategory = CategoryManager.Instance.GetRandomCategoryForLayout(selectedLayout);

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
            newCard.OnCardFlipped += matchController.RegisterCard;
            spawnedCards.Add(newCard);
        }

        // Arrange cards in grid
        ArrangeCardsInGrid();
    }

    /// <summary>
    /// Determines the number of rows and columns based on the selected layout.
    /// </summary>
    private void GetLayoutSize(LayoutType layout)
    {
        switch (layout)
        {
            case LayoutType.Layout2x2:
                rows = 2;
                columns = 2;
                break;
            case LayoutType.Layout2x3:
                rows = 2;
                columns = 3;
                break;
            case LayoutType.Layout2x4:
                rows = 2;
                columns = 4;
                break;
            case LayoutType.Layout4x4:
                rows = 4;
                columns = 4;
                break;
            case LayoutType.Layout5x6:
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
        float cardWidth = (parentSize.x - (columns - 1) * selectedCategory.CardSpacing.x) / columns;
        float cardHeight = (parentSize.y - (rows - 1) * selectedCategory.CardSpacing.y) / rows;

        gridLayout.cellSize = new Vector2(cardWidth, cardHeight);
        gridLayout.spacing = selectedCategory.CardSpacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
    }

    /// <summary>
    /// Coroutine that shows all card faces, then flips them back and enables interaction.
    /// </summary>
    private IEnumerator PreviewCardsThenPlay()
    {
        // Show all cards face-up for preview
        foreach (var card in spawnedCards)
        {
            card.ChangeCardFace(false);
        }

        // Wait for preview duration
        yield return new WaitForSeconds(previewDuration);

        // Flip all cards back to start the game
        foreach (var card in spawnedCards)
        {
            card.ChangeCardFace(true);
        }
    }
}
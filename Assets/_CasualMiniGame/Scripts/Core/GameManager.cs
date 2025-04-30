using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private CardMatchController cardMatchController;
    [SerializeField] private ScoreManager scoreManager;

    private LayoutType selectedLayout;
    private CardCategoryData selectedCategory;

    private List<Card> spawnedCards = new List<Card>();

    private int rows;
    private int columns;

    private bool isGameOver = false;

    // Event will be triggered when game is over
    public event System.Action OnGameOver;

    private void OnEnable()
    {
        // Subscribe events
        cardMatchController.OnCardMatched += CheckGameOverConditions; // Receiving and Handling of card match event
    }

    private void OnDisable()
    {
        // Unsubscribe events
        cardMatchController.OnCardMatched -= CheckGameOverConditions;
    }

    private void Start()
    {
        scoreManager.ResetScore(); // Resets score at start of the game

        // Checks if there is any valid save file
        bool hasSaveFile = (PlayerPrefs.GetInt(GameDataDefinitions.GAME_SAVE_FILE, 0) == 0) ? false : true;
        if (hasSaveFile)
        {
            GameProgressManager.Instance.LoadGame(); // Loads saved game
        }
        else // Starts new game
        {
            InitializeBoard();
            StartCoroutine(PreviewCardsThenPlay());
        }
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
            newCard.OnCardFlipped += cardMatchController.RegisterCard;
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

    /// <summary>
    /// Returns string list of spawnedCards list.
    /// </summary>
    public List<string> GetSpawnedCardKeys()
    {
        var list = spawnedCards.Select(card => card.GetCategorySpriteKey()).ToList();
        return list;
    }

    /// <summary>
    /// Returns CardCategoryName as string.
    /// </summary>
    public string GetCardCategoryName()
    {
        return selectedCategory.CategoryName;
    }

    /// <summary>
    /// Returns selectedLayout as assigned value of LayoutType enum.
    /// </summary>
    public LayoutType GetLayoutType()
    {
        return selectedLayout;
    }

    /// <summary>
    /// Loads the game data from save file.
    /// </summary>
    public void LoadGameData(GameData data)
    {
        // Store the layout for use in board generation
        selectedLayout = data.selectedLayout;

        // Fetch and assign selected card category scriptable object
        selectedCategory = CategoryManager.Instance.GetCategoryByName(data.selectedCategoryName);

        // Safety Check
        if (selectedCategory == null || cardPrefab == null || boardParent == null)
        {
            // TODO Restart the game from main menu with warning UI prompt as well.
            Debug.LogError("Corrupted Data!");
            return;
        }

        // Get layout size (rows, columns) based on layout enum
        GetLayoutSize(selectedLayout);

        // Spawning all the cards in game board
        foreach (var key in data.spawnedCardIds)
        {
            var parts = key.Split('_');
            if (parts.Length != 2) continue;

            string cardSpriteName = parts[1];

            Sprite cardSprite = selectedCategory.GetSpriteByName(cardSpriteName);
            if (cardSprite == null)
            {
                // TODO Restart the game from main menu with warning UI prompt as well.
                Debug.LogWarning($"Sprite '{cardSpriteName}' not found in category '{data.selectedCategoryName}'.");
                continue;
            }

            Card newCard = Instantiate(cardPrefab, boardParent);
            newCard.InitializeCard(cardSprite, selectedCategory.CardBackSprite);
            newCard.OnCardFlipped += cardMatchController.RegisterCard;
            spawnedCards.Add(newCard);

            // Loading matched cards
            if (data.matchedCardIds.Contains(newCard.GetCategorySpriteKey()))
                cardMatchController.LoadMatchedCards(newCard);

            // Loading waiting cards
            if (data.waitingCardIds.Contains(newCard.GetCategorySpriteKey()))
                cardMatchController.LoadPendingMatchEvalutionCards(newCard);
        }

        // Arrange cards in grid
        ArrangeCardsInGrid();

        // Loading score progress
        scoreManager.LoadProgress(data.turnCount, data.totalScore, data.matchCount, data.currentCombo);

        // Starts matchmaking of cards in waitingCards list
        cardMatchController.InitiateWaitingCardsMatchMaking();

        // Set interactibility to true for not opened cards
        foreach(Card card in spawnedCards)
            card.SetCardInteractability(true);
    }

    /// <summary>
    /// This will be received when user kills application.
    /// </summary>
    private void OnApplicationQuit()
    {
        if(!isGameOver)
            GameProgressManager.Instance.Save(); // saving game file
    }

    /// <summary>
    /// Checks game over conditions.
    /// </summary>
    private void CheckGameOverConditions(int matchedCardsCount)
    {
        if(spawnedCards.Count == matchedCardsCount)
        {
            isGameOver = true;
            SoundManager.Instance.PlaySFX(SFXType.GameOver);
            GameProgressManager.Instance.DeleteSave();
            OnGameOver?.Invoke();
        }
    }
}
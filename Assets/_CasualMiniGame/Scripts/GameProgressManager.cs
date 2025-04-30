using UnityEngine;

/// <summary>
/// Coordinates saving and loading game state using injected services and current gameplay managers.
/// </summary>
public class GameProgressManager : MonoBehaviour
{
    [Header("Dependencies")]
    private GameManager gameManager;
    private ScoreManager scoreManager;
    private CardMatchController matchController;

    private ISaveService saveService;

    public static GameProgressManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure a single persistent instance (singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            saveService = new FileSaveService();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Saves the current game state using the save service.
    /// </summary>
    public void Save()
    {
        gameManager = FindObjectOfType<GameManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        matchController = FindObjectOfType<CardMatchController>();

        GameData data = new GameData
        {
            selectedCategoryName = gameManager.GetCardCategoryName(),
            selectedLayout = gameManager.GetLayoutType(),
            turnCount = scoreManager.TurnCount,
            matchCount = scoreManager.MatchCount,
            totalScore = scoreManager.TotalScore,
            currentCombo = scoreManager.CurrentCombo,
            spawnedCardIds = gameManager.GetSpawnedCardKeys(),
            waitingCardIds = matchController.GetPendingMatchEvalutionCardsKeys(),
            matchedCardIds = matchController.GetMatchedCardsKeys()
        };

        saveService.SaveGame(data);
        PlayerPrefs.SetInt(GameDataDefinitions.GAME_SAVE_FILE, 1);
    }

    /// <summary>
    /// Loads a previously saved game state and applies it to managers.
    /// </summary>
    public void LoadGame()
    {
        if (!saveService.HasSavedGame()) return;

        GameData data = saveService.LoadGame();

        gameManager = FindObjectOfType<GameManager>();
        gameManager.LoadGameData(data);
    }

    /// <summary>
    /// Clears saved data (e.g., on new game).
    /// </summary>
    public void DeleteSave()
    {
        saveService.DeleteSavedGame();
        PlayerPrefs.DeleteKey(GameDataDefinitions.GAME_SAVE_FILE); 
    }
}
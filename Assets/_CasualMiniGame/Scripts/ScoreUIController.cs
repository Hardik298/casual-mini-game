using TMPro;
using UnityEngine;

/// <summary>
/// Handles real-time updates of the score, turn count and match count UI.
/// Listens to ScoreManager events and updates corresponding TextMeshProUGUI elements.
/// </summary>
public class ScoreUIController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Reference to the UI Text component displaying the total turn count.")]
    [SerializeField] private TextMeshProUGUI turnCountText;

    [Tooltip("Reference to the UI Text component displaying the player's score.")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Tooltip("Reference to the UI Text component displaying the total match count.")]
    [SerializeField] private TextMeshProUGUI matchCountText;

    [Tooltip("Reference to the ScoreManager that raises score/match events.")]
    [SerializeField] private ScoreManager scoreManager;

    /// <summary>
    /// Subscribes to ScoreManager events when this object is enabled.
    /// Ensures the UI updates when score, turns or matches change.
    /// </summary>
    private void OnEnable()
    {
        scoreManager.OnTurnCountChanged += UpdateTurnCountText;
        scoreManager.OnScoreChanged += UpdateScoreText;
        scoreManager.OnMatchCountChanged += UpdateMatchCountText;
    }

    /// <summary>
    /// Unsubscribes from ScoreManager events to prevent memory leaks or null reference errors.
    /// </summary>
    private void OnDisable()
    {
        scoreManager.OnTurnCountChanged -= UpdateTurnCountText;
        scoreManager.OnScoreChanged -= UpdateScoreText;
        scoreManager.OnMatchCountChanged -= UpdateMatchCountText;
    }

    /// <summary>
    /// Updates the turns display when CardMatchController notifies a turn change.
    /// </summary>
    /// <param name="turnCount">The updated turn count.</param>
    private void UpdateTurnCountText(int turnCount)
    {
        turnCountText.text = turnCount.ToString();
    }

    /// <summary>
    /// Updates the score display when ScoreManager notifies a score change.
    /// </summary>
    /// <param name="score">The updated score value.</param>
    private void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    /// <summary>
    /// Updates the match count display when ScoreManager notifies a match count change.
    /// </summary>
    /// <param name="matchCount">The updated match count.</param>
    private void UpdateMatchCountText(int matchCount)
    {
        matchCountText.text = $"Matches: {matchCount}";
    }
}
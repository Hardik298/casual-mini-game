using UnityEngine;
using System;

/// <summary>
/// Handles scoring logic for the card match game,
/// including combo streaks and real-time event notifications to UI listeners.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Scoring Settings")]
    [Tooltip("Points awarded for a successful match (base score).")]
    [SerializeField] private int matchScore = 1;

    [Tooltip("Additional points awarded per combo level beyond the first.")]
    [SerializeField] private int comboBonus = 1;

    /// <summary>
    /// Event triggered whenever the turn count is updated.
    /// The updated turn count is passed as an int argument.
    /// </summary>
    public event Action<int> OnTurnCountChanged;

    /// <summary>
    /// Event triggered whenever the score is updated.
    /// The updated score is passed as an int argument.
    /// </summary>
    public event Action<int> OnScoreChanged;

    /// <summary>
    /// Event triggered whenever the total match count is updated.
    /// The updated match count is passed as an int argument.
    /// </summary>
    public event Action<int> OnMatchCountChanged;

    private int _turnCount;
    /// <summary>
    /// The current turn count of the player.
    /// Triggers <see cref="OnTurnCountChanged"/> when updated.
    /// </summary>
    public int TurnCount
    {
        get => _turnCount;
        private set
        {
            _turnCount = value;
            OnTurnCountChanged?.Invoke(_turnCount);
        }
    }

    private int _totalScore;
    /// <summary>
    /// The current total score of the player.
    /// Triggers <see cref="OnScoreChanged"/> when updated.
    /// </summary>
    public int TotalScore
    {
        get => _totalScore;
        private set
        {
            _totalScore = value;
            OnScoreChanged?.Invoke(_totalScore);
        }
    }

    private int _matchCount;
    /// <summary>
    /// The total number of successful matches made.
    /// Triggers <see cref="OnMatchCountChanged"/> when updated.
    /// </summary>
    public int MatchCount
    {
        get => _matchCount;
        private set
        {
            _matchCount = value;
            OnMatchCountChanged?.Invoke(_matchCount);
        }
    }

    /// <summary>
    /// Tracks the current combo streak.
    /// Increases with each consecutive match, resets on mismatch.
    /// </summary>
    public int CurrentCombo { get; private set; }

    /// <summary>
    /// Registers a successful card match.
    /// Increases score based on base match points and combo multiplier.
    /// Increments combo and match count.
    /// </summary>
    public void RegisterMatch()
    {
        CurrentCombo++;
        int bonus = (CurrentCombo - 1) * comboBonus;
        int scoreToAdd = matchScore + bonus;

        TotalScore += scoreToAdd;
        MatchCount++;

        Debug.Log($"Match! Combo x{CurrentCombo} → +{scoreToAdd} points. Total Score: {TotalScore}");
    }

    /// <summary>
    /// Registers a mismatch.
    /// Resets the current combo counter to 0.
    /// </summary>
    public void RegisterMismatch()
    {
        if (CurrentCombo > 0)
        {
            Debug.Log($"Mismatch! Combo reset from x{CurrentCombo}");
        }

        CurrentCombo = 0;
    }

    /// <summary>
    /// Increase turn count by 1.
    /// </summary>
    public void IncreaseTurnCount()
    {
        TurnCount += 1;
    }

    /// <summary>
    /// Resets score, combo, and match count.
    /// Use this at the beginning of a game or when restarting.
    /// </summary>
    public void ResetScore()
    {
        TotalScore = 0;
        MatchCount = 0;
        CurrentCombo = 0;
    }
}
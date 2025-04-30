using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages matching logic for flipped cards. Allows multiple independent pairs to be evaluated simultaneously.
/// </summary>
public class CardMatchController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private ScoreManager scoreManager;

    [Tooltip("Delay (in seconds) before flipping back unmatched cards.")]
    [SerializeField] private float mismatchDelay = 1.0f;

    /// <summary>
    /// Cards waiting to be matched (processed in pairs).
    /// </summary>
    private List<Card> waitingCards = new();

    /// <summary>
    /// Matched cards.
    /// </summary>
    private List<Card> matchedCards = new();

    /// <summary>
    /// Cards waiting to be evaluted.
    /// This list will help while saving and loading not evaluted cards data to save file.
    /// </summary>
    private List<Card> pendingMatchEvalutionCards = new();

    /// <summary>
    /// Called when a card is flipped; enqueues for matching.
    /// </summary>
    /// <param name="card">The newly flipped card.</param>
    public void RegisterCard(Card card)
    {
        if (card == null || waitingCards.Contains(card))
            return;

        waitingCards.Add(card);
        pendingMatchEvalutionCards.Add(card);
        scoreManager.IncreaseTurnCount();

        // Process cards in pairs immediately
        if (waitingCards.Count >= 2)
        {
            Card cardA = waitingCards[0];
            Card cardB = waitingCards[1];
            waitingCards.RemoveRange(0, 2);

            StartCoroutine(EvaluateMatch(cardA, cardB));
        }
    }

    /// <summary>
    /// Compares two cards and marks them as matched or resets after delay.
    /// </summary>
    private IEnumerator EvaluateMatch(Card cardA, Card cardB)
    {
        yield return new WaitForSeconds(mismatchDelay);

        if (cardA.GetCategorySpriteKey() == cardB.GetCategorySpriteKey())
        {
            cardA.MatchCard(false);
            cardB.MatchCard(false);
            matchedCards.Add(cardA);
            matchedCards.Add(cardB);
            scoreManager.RegisterMatch();
        }
        else
        {
            cardA.ResetCard();
            cardB.ResetCard();
            scoreManager.RegisterMismatch();
        }

        pendingMatchEvalutionCards.Remove(cardA);
        pendingMatchEvalutionCards.Remove(cardB);
    }

    public List<string> GetMatchedCardsKeys()
    {
        return matchedCards.Select(card => card.GetCategorySpriteKey()).ToList();
    }

    public List<string> GetPendingMatchEvalutionCardsKeys()
    {
        return pendingMatchEvalutionCards.Select(card => card.GetCategorySpriteKey()).ToList();
    }

    public void LoadMatchedCards(Card matchedCard)
    {
        matchedCards.Add(matchedCard);
        matchedCard.MatchCard(true);
    }

    public void LoadPendingMatchEvalutionCards(Card waitingCard)
    {
        waitingCards.Add(waitingCard);
        pendingMatchEvalutionCards.Add(waitingCard);
    }

    public void InitiateWaitingCardsMatchMaking()
    {
        while (waitingCards.Count >= 2)
        {
            Card cardA = waitingCards[0];
            Card cardB = waitingCards[1];
            waitingCards.RemoveRange(0, 2);

            StartCoroutine(EvaluateMatch(cardA, cardB));
        }
    }
}
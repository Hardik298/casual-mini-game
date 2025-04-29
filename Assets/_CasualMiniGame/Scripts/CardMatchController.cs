using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages matching logic for flipped cards. Allows multiple independent pairs to be evaluated simultaneously.
/// </summary>
public class CardMatchController : MonoBehaviour
{
    [Tooltip("Delay (in seconds) before flipping back unmatched cards.")]
    [SerializeField] private float mismatchDelay = 1.0f;

    /// <summary>
    /// Cards waiting to be matched (processed in pairs).
    /// </summary>
    private List<Card> waitingCards = new();

    /// <summary>
    /// Called when a card is flipped; enqueues for matching.
    /// </summary>
    /// <param name="card">The newly flipped card.</param>
    public void RegisterCard(Card card)
    {
        if (card == null || waitingCards.Contains(card))
            return;

        waitingCards.Add(card);

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

        if (cardA.GetCardFrontSprite() == cardB.GetCardFrontSprite())
        {
            cardA.MatchCard();
            cardB.MatchCard();
        }
        else
        {
            cardA.ResetCard();
            cardB.ResetCard();
        }
    }
}
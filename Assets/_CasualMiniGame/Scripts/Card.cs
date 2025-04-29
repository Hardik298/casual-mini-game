using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages individual card behavior in the game.
/// </summary>
public class Card : MonoBehaviour
{
    [Header("Card Settings")]
    [Tooltip("The image component for front side of card.")]
    [SerializeField] private Image frontCardImage; // Image component for the card

    [Tooltip("The image component for back side of card.")]
    [SerializeField] private Image backCardImage; // Image component for the card

    private Sprite cardFrontSprite;  // The front sprite that the card will display after flipping
    private Sprite cardBackSprite;  // The back sprite that the card will display after reset

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (frontCardImage == null)
            frontCardImage = GetComponent<Image>(); // Fallback if not assigned
    }

    /// <summary>
    /// Initializes the card with a front sprite and sets the back sprite.
    /// </summary>
    /// <param name="frontSprite">The sprite to be shown on the front of the card.</param>
    /// <param name="backSprite">The sprite to be shown on the back of the card.</param>
    public void InitializeCard(Sprite frontSprite, Sprite backSprite)
    {
        cardFrontSprite = frontSprite;
        cardBackSprite = backSprite;

        frontCardImage.sprite = cardFrontSprite;
        backCardImage.sprite = cardBackSprite;

        frontCardImage.gameObject.SetActive(false);
        backCardImage.gameObject.SetActive(true); // Initially show the card back
        transform.rotation = Quaternion.identity;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles card visuals, flipping animations, and state tracking (flipped, matched).
/// </summary>
public class Card : MonoBehaviour
{
    [Header("Card Settings")]
    [Tooltip("The image component for background of front side of card.")]
    [SerializeField] private Image frontCardBackgroundImage; // Image component for the card

    [Tooltip("The image component for front side of card.")]
    [SerializeField] private Image frontCardImage; // Image component for the card

    [Tooltip("The image component for back side of card.")]
    [SerializeField] private Image backCardImage; // Image component for the card

    private Sprite cardFrontSprite;  // The front sprite that the card will display after flipping
    private Sprite cardBackSprite;  // The back sprite that the card will display after reset

    private bool isFlipped = false;
    private bool isMatched = false;
    private bool isFlipping = false;
    private bool isInteractable = false;

    public string CategoryName { get; private set; } // Assigned during board creation

    public event System.Action<Card> OnCardFlipped; // Event will be triggered when a card is flipped to front.

    private void Awake()
    {
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

    /// <summary>
    /// Changes card face from front to back and vice versa.
    /// <param name="isCardInteractable">Toggles card interactability.</param>
    /// </summary>
    public void ChangeCardFace(bool cardInteractability)
    {
        SetCardInteractability(cardInteractability);
        StartCoroutine(CardFlipRoutine());
    }

    /// <summary>
    /// Changes card face from front to back and vice versa.
    /// <param name="isCardInteractable">Toggles card interactability.</param>
    /// </summary>
    public void SetCardInteractability(bool cardInteractability)
    {
        isInteractable = cardInteractability;
    }

    /// <summary>
    /// Starts the flip animation if not already flipping or matched.
    /// </summary>
    public void FlipCard()
    {
        if (!isInteractable || isFlipping || isMatched) return;
        SoundManager.Instance.PlaySFX(SFXType.CardFlip);
        StartCoroutine(CardFlipRoutine());
    }

    /// <summary>
    /// Resets the card to face-down state.
    /// </summary>
    public void ResetCard()
    {
        if (isMatched || isFlipping || !isFlipped) return;
        StartCoroutine(CardFlipRoutine());
    }

    /// <summary>
    /// Marks the card as matched and visually dims it.
    /// </summary>
    public void MatchCard(bool isDirectSet)
    {
        isMatched = true;
        isInteractable = false;

        if (isDirectSet)
        {
            isFlipped = true;
            frontCardBackgroundImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Semi-transparent match indicator
            frontCardImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Semi-transparent match indicator
            backCardImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f); // Semi-transparent match indicator
        }
        else
        {
            StartCoroutine(FadeOutImageRoutine(frontCardBackgroundImage.GetComponent<Image>(), 1f));
            StartCoroutine(FadeOutImageRoutine(frontCardImage.GetComponent<Image>(), 1f));
            StartCoroutine(FadeOutImageRoutine(backCardImage.GetComponent<Image>(), 1f));
        }
    }

    /// <summary>
    /// Returns a unique key combining category and sprite name.
    /// </summary>
    public string GetCategorySpriteKey()
    {
        return $"{CategoryName}_{cardFrontSprite.name}";
    }

    /// <summary>
    /// Smoothly rotates the card to flip it front/back.
    /// </summary>
    private IEnumerator CardFlipRoutine()
    {
        isFlipping = true;
        float duration = 0.4f;
        float halfTime = duration / 2f;
        float elapsed = 0f;

        Quaternion startRot = transform.rotation;
        Quaternion midRot = Quaternion.Euler(0, 90, 0);
        Quaternion endRot = isFlipped ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        // First half: rotate to side
        while (elapsed < halfTime)
        {
            transform.rotation = Quaternion.Slerp(startRot, midRot, elapsed / halfTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Toggle face state at 90°
        transform.rotation = midRot;
        isFlipped = !isFlipped;
        frontCardImage.gameObject.SetActive(isFlipped);
        backCardImage.gameObject.SetActive(!isFlipped);

        if (isInteractable && isFlipped)
            OnCardFlipped?.Invoke(this);

        // Second half: rotate to full flip
        elapsed = 0f;
        while (elapsed < halfTime)
        {
            transform.rotation = Quaternion.Slerp(midRot, endRot, elapsed / halfTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
        isFlipping = false;
    }

    /// <summary>
    /// Smoothly fades out the image.
    /// </summary>
    private IEnumerator FadeOutImageRoutine(Image image, float duration)
    {
        Color originalColor = image.color;
        float startAlpha = originalColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's fully transparent at the end
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

}
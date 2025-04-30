using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

/// <summary>
/// Handles setup and functionality of a confirmation popup with customizable message and actions.
/// </summary>
public class ConsentPopUp : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Text element displaying the popup message.")]
    [SerializeField] private TMP_Text titleText;

    [Tooltip("Button to confirm the action.")]
    [SerializeField] private Button confirmButton;

    [Tooltip("Button to cancel the action.")]
    [SerializeField] private Button cancelButton;

    // Actions to execute on button click
    private Action onConfirmAction;
    private Action onCancelAction;

    /// <summary>
    /// Initializes the popup with custom message and actions for confirm and cancel buttons.
    /// </summary>
    /// <param name="message">Message to display in the popup.</param>
    /// <param name="onConfirm">Action to execute when confirm button is clicked.</param>
    /// <param name="onCancel">Action to execute when cancel button is clicked.</param>
    public void Setup(string message, Action onConfirm, Action onCancel)
    {
        titleText.text = message;
        onConfirmAction = onConfirm;
        onCancelAction = onCancel;

        // Ensure previous listeners are cleared before assigning new ones
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() => { onConfirmAction?.Invoke(); ClosePopup(); });
        cancelButton.onClick.AddListener(() => { onCancelAction?.Invoke(); ClosePopup(); });

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes the popup and destroys its GameObject.
    /// </summary>
    public void ClosePopup()
    {
        Destroy(gameObject);
    }
}
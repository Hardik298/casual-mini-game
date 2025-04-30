using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ConsentPopUp : MonoBehaviour
{
    public TMP_Text titleText;
    public Button confirmButton;
    public Button cancelButton;

    private Action onConfirmAction;
    private Action onCancelAction;

    public void Setup(string message, Action onConfirm, Action onCancel)
    {
        titleText.text = message;
        onConfirmAction = onConfirm;
        onCancelAction = onCancel;

        // Assign dynamic actions to buttons
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() => { onConfirmAction?.Invoke(); ClosePopup(); });
        cancelButton.onClick.AddListener(() => { onCancelAction?.Invoke(); ClosePopup(); });

        gameObject.SetActive(true);
    }

    public void ClosePopup()
    {
        Destroy(gameObject); // Destroy after action
    }
}
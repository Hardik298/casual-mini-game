using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the UI elements during gameplay, such as handling scene transitions and managing game over events.
/// </summary>
public class GamePlayUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager; // Reference to the GameManager
    [SerializeField] private string mainMenuSceneName; // Name of the main menu scene to load
    [SerializeField] private string gamePlaySceneName; // Name of the game play scene to load

    /// <summary>
    /// Subscribes to the game over event when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        gameManager.OnGameOver += HandleGameOverEvent; // Subscribe to the OnGameOver event.
    }

    /// <summary>
    /// Unsubscribes from the game over event when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        gameManager.OnGameOver -= HandleGameOverEvent; // Unsubscribe from the OnGameOver event to avoid memory leaks.
    }

    /// <summary>
    /// Called when the home button is clicked. It loads the main menu scene.
    /// </summary>
    public void OnHomeButtonClicked()
    {
        SceneManagerController.Instance.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Handles the game over event by instantiating and displaying a confirmation pop-up.
    /// </summary>
    private void HandleGameOverEvent()
    {
        GameObject popUp = Instantiate(Resources.Load<GameObject>("ConsentPopUp"));
        popUp.GetComponent<ConsentPopUp>().Setup("Do you want to play next level?", OnGameOverConfirmationPopUpYesBtnClicked, OnGameOverConfirmationPopUpNoBtnClicked);
    }

    void OnGameOverConfirmationPopUpYesBtnClicked()
    {
        GameProgressManager.Instance.DeleteSave();
        SceneManagerController.Instance.LoadScene(gamePlaySceneName); // Load the gameplay scene
    }

    void OnGameOverConfirmationPopUpNoBtnClicked()
    {
        GameProgressManager.Instance.DeleteSave();
        SceneManagerController.Instance.LoadScene(mainMenuSceneName); // Load the main menu scene
    }
}

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles interactions with main menu options
/// </summary>
public class MainMenuUIController : MonoBehaviour
{
    [SerializeField] private string gamePlaySceneName; // Name of the gameplay scene to load
    [SerializeField] private ToggleGroup gameDifficultyToggleGroup; // Toggle group for difficulty level selection

    /// <summary>
    /// Called when the Play button is clicked. 
    /// If there is a saved game, it prompts the user with a popup asking if they want to resume their last game or start a new one.
    /// If there is no saved game, it starts a new game directly.
    /// </summary>
    public void OnPlayButtonClicked()
    {
        bool hasSaveFile = (PlayerPrefs.GetInt(GameDataDefinitions.GAME_SAVE_FILE, 0) == 0) ? false : true;
        if (hasSaveFile)
        {
            GameObject popUp = Instantiate(Resources.Load<GameObject>("ConsentPopUp"));
            popUp.GetComponent<ConsentPopUp>().Setup("Do you want to continue your last game?", OnResumeGameConfirmationPopUpYesBtnClicked, OnResumeGameConfirmationPopUpNoBtnClicked);
        }
        else
        {
            StartNewGame();
        }
    }

    void OnResumeGameConfirmationPopUpYesBtnClicked()
    {
        SceneManagerController.Instance.LoadScene(gamePlaySceneName); // Load the gameplay scene
    }

    void OnResumeGameConfirmationPopUpNoBtnClicked()
    {
        GameProgressManager.Instance.DeleteSave();
        StartNewGame(); // Starts new game
    }

    private void StartNewGame()
    {
        // Get the currently active toggle from the toggle group
        var activeToggle = gameDifficultyToggleGroup.ActiveToggles().FirstOrDefault();

        // Parse the toggle's name into an integer difficulty level
        int difficultyLevel = int.Parse(activeToggle.name);

        // Save the difficulty level in PlayerPrefs using the defined constant key
        PlayerPrefs.SetInt(GameDataDefinitions.GAME_DIFFICULTY, difficultyLevel);

        // Load the gameplay scene
        SceneManagerController.Instance.LoadScene(gamePlaySceneName);
    }
}

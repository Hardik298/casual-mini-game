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
    /// Stores the selected difficulty level in PlayerPrefs and loads the gameplay scene.
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
        // Load the gameplay scene
        SceneManagerController.Instance.LoadScene(gamePlaySceneName);
    }

    void OnResumeGameConfirmationPopUpNoBtnClicked()
    {
        GameProgressManager.Instance.DeleteSave();
        StartNewGame();
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

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles interactions with main menu options
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gamePlaySceneName; // Name of the gameplay scene to load
    [SerializeField] private ToggleGroup gameDifficultyToggleGroup; // Toggle group for difficulty level selection

    /// <summary>
    /// Called when the Play button is clicked.
    /// Stores the selected difficulty level in PlayerPrefs and loads the gameplay scene.
    /// </summary>
    public void OnPlayButtonClicked()
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

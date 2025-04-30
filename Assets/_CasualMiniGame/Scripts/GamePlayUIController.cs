using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayUIController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private string mainMenuSceneName; // Name of the main menu scene to load
    [SerializeField] private string gamePlaySceneName; // Name of the game play scene to load

    private void OnEnable()
    {
        gameManager.OnGameOver += HandleGameOverEvent;
    }

    private void OnDisable()
    {
        gameManager.OnGameOver -= HandleGameOverEvent;
    }

    public void OnHomeButtonClicked()
    {
        SceneManagerController.Instance.LoadScene(mainMenuSceneName);
    }

    private void HandleGameOverEvent()
    {
        GameObject popUp = Instantiate(Resources.Load<GameObject>("ConsentPopUp"));
        popUp.GetComponent<ConsentPopUp>().Setup("Do you want to play next level?", OnGameOverConfirmationPopUpYesBtnClicked, OnGameOverConfirmationPopUpNoBtnClicked);
    }

    void OnGameOverConfirmationPopUpYesBtnClicked()
    {
        // Load the gameplay scene
        SceneManagerController.Instance.LoadScene(gamePlaySceneName);
    }

    void OnGameOverConfirmationPopUpNoBtnClicked()
    {
        // Load the main menu scene
        SceneManagerController.Instance.LoadScene(mainMenuSceneName);
    }
}

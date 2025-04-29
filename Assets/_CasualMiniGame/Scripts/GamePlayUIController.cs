using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayUIController : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName; // Name of the main menu scene to load

    public void OnHomeButtonClicked()
    {
        SceneManagerController.Instance.LoadScene(mainMenuSceneName);
    }
}

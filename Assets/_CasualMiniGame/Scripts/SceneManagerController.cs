using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// A singleton controller to manage scene loading operations, including synchronous and asynchronous loading.
/// </summary>
public class SceneManagerController : MonoBehaviour
{
    private static SceneManagerController _instance;

    /// <summary>
    /// Provides global access to the SceneManagerController instance.
    /// </summary>
    public static SceneManagerController Instance => _instance;

    private void Awake()
    {
        // Ensure a single persistent instance (singleton pattern)
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads a scene synchronously by its name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads a scene synchronously by its build index.
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to load.</param>
    public void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Loads a scene asynchronously by its name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    /// <summary>
    /// Coroutine for handling asynchronous scene loading.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            // You can hook into this progress for a UI loading bar, if needed
            Debug.Log($"Loading progress: {asyncLoad.progress * 100:F0}%");
            yield return null;
        }
    }
}
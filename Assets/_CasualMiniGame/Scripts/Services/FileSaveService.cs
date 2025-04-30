using System.IO;
using UnityEngine;

/// <summary>
/// Handles saving and loading game data using JSON serialization to disk.
/// </summary>
public class FileSaveService : ISaveService
{
    /// <summary>
    /// Full file path where the game data will be saved.
    /// </summary>
    private readonly string saveFilePath;

    /// <summary>
    /// Constructor initializes the file path using Unity's persistent data directory.
    /// </summary>
    public FileSaveService()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    /// <summary>
    /// Saves the game data to disk as a JSON file.
    /// </summary>
    /// <param name="data">Game data object to serialize and save.</param>
    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(saveFilePath, json);
#if UNITY_EDITOR
        Debug.Log("Game saved at: " + saveFilePath);
#endif
    }

    /// <summary>
    /// Loads game data from the JSON file on disk.
    /// </summary>
    /// <returns>Deserialized GameData object if the file exists; otherwise, null.</returns>
    public GameData LoadGame()
    {
        if (!File.Exists(saveFilePath)) return null;

        string json = File.ReadAllText(saveFilePath);
        return JsonUtility.FromJson<GameData>(json);
    }

    /// <summary>
    /// Checks if a saved game file exists.
    /// </summary>
    /// <returns>True if save file exists; otherwise, false.</returns>
    public bool HasSavedGame()
    {
        return File.Exists(saveFilePath);
    }

    /// <summary>
    /// Deletes the saved game file from disk, if it exists.
    /// </summary>
    public void DeleteSavedGame()
    {
        if (File.Exists(saveFilePath))
            File.Delete(saveFilePath);
    }
}
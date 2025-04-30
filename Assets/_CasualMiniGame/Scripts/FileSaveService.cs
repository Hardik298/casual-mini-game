using System.IO;
using UnityEngine;

/// <summary>
/// Handles saving and loading game data using JSON serialization to disk.
/// </summary>
public class FileSaveService : ISaveService
{
    private readonly string saveFilePath;

    public FileSaveService()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(saveFilePath, json);
#if UNITY_EDITOR
        Debug.Log("Game saved at: " + saveFilePath);
#endif
    }

    public GameData LoadGame()
    {
        if (!File.Exists(saveFilePath)) return null;

        string json = File.ReadAllText(saveFilePath);
        return JsonUtility.FromJson<GameData>(json);
    }

    public bool HasSavedGame()
    {
        return File.Exists(saveFilePath);
    }

    public void DeleteSavedGame()
    {
        if (File.Exists(saveFilePath))
            File.Delete(saveFilePath);
    }
}
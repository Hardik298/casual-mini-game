using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles loading and managing card categories.
/// </summary>
public class CategoryManager : MonoBehaviour
{
    public static CategoryManager Instance { get; private set; }

    private List<CardCategoryData> availableCategories;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of CategoryManager detected.");
            return;
        }
        Instance = this;
        LoadCategories();
    }

    /// <summary>
    /// Loads all CardCategoryData assets from the Resources/CardCategories folder.
    /// </summary>
    private void LoadCategories()
    {
        availableCategories = new List<CardCategoryData>(
            Resources.LoadAll<CardCategoryData>("CardCategories")
        );

        Debug.Log($"Loaded {availableCategories.Count} card categories.");
    }

    /// <summary>
    /// Finds categories that support a specific layout.
    /// </summary>
    /// <param name="layoutType">The layout type we want to find categories for.</param>
    /// <returns>A list of categories that support the given layout.</returns>
    public List<CardCategoryData> GetCategoriesForLayout(LayoutType layoutType)
    {
        List<CardCategoryData> matchingCategories = new();

        foreach (var category in availableCategories)
        {
            if ((category.AllowedLayouts & layoutType) != 0)
            {
                matchingCategories.Add(category);
            }
        }

        return matchingCategories;
    }

    /// <summary>
    /// Returns a random category that supports the selected layout.
    /// </summary>
    /// <param name="layoutType">The layout type we want to find categories for.</param>
    /// <returns>A random category that supports the given layout, or null if none found.</returns>
    public CardCategoryData GetRandomCategoryForLayout(LayoutType layoutType)
    {
        List<CardCategoryData> validCategories = GetCategoriesForLayout(layoutType);

        if (validCategories.Count == 0)
        {
            Debug.LogWarning($"No categories available for layout {layoutType}");
            return null;
        }

        return validCategories[Random.Range(0, validCategories.Count)];
    }
}
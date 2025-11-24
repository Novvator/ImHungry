using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Vector3 spawnPosition = new Vector3(4f, 2.5f, 0f);
    public Vector3 targetPosition = new Vector3(0f, 2.5f, 0f);
    public float moveDuration = 0.5f;
    public float spawnDelay = 0.15f;

    [Header("Food Names Override")]
    public string[] foodNameOverrides;

    private List<string> availableFoodNames = new List<string>();
    private int lastIndex = -1; // track last spawned food

    void Awake()
    {
        // Use overrides if provided
        if (foodNameOverrides != null && foodNameOverrides.Length > 0)
        {
            availableFoodNames.AddRange(foodNameOverrides);
        }

        // Fallback list if none provided
        if (availableFoodNames.Count == 0)
        {
            availableFoodNames.AddRange(new[] { "burg", "cola", "fries", "gyros", "nuggies" });
        }
    }

    /// <summary>
    /// Spawns the next food object at spawnPosition and moves it to targetPosition.
    /// Sequential logic: no food repeats immediately.
    /// </summary>
    public GameObject SpawnNextFood()
    {
        string foodName = GetNextFoodName();

        if (foodName == null)
        {
            Debug.LogError("FoodSpawner: No food names available to spawn.");
            return null;
        }

        Sprite[] sprites = LoadSprites(foodName);

        if (sprites.Length == 0)
        {
            Debug.LogWarning($"FoodSpawner: No sprites found for '{foodName}'. Skipping spawn.");
            return null;
        }

        // Create GameObject
        GameObject go = new GameObject(foodName);
        go.transform.SetParent(transform);
        go.transform.position = spawnPosition;
        go.transform.localScale = Vector3.one * 3.5f;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprites[0];

        go.AddComponent<BoxCollider2D>();

        FoodScript fs = go.AddComponent<FoodScript>();
        fs.Init(sprites);
        fs.MoveToTarget(targetPosition);

        StartCoroutine(MoveToPosition(go.transform, targetPosition, moveDuration));

        return go;
    }

    /// <summary>
    /// Returns the next food name sequentially, skipping last spawned.
    /// Works for any number of foods, including 2-food cases.
    /// </summary>
    private string GetNextFoodName()
    {
        int count = availableFoodNames.Count;

        if (count == 0) return null;

        // Only 1 food → always return it
        if (count == 1) return availableFoodNames[0];

        // Only 2 foods → alternate perfectly
        if (count == 2)
        {
            lastIndex = (lastIndex == 0 ? 1 : 0);
            return availableFoodNames[lastIndex];
        }

        // 3 or more foods → sequential, skip last
        int next = (lastIndex + 1) % count;

        if (next == lastIndex) // safety
            next = (next + 1) % count;

        lastIndex = next;
        return availableFoodNames[next];
    }

    /// <summary>
    /// Loads up to 3 sprites for a food from Resources/foods/<foodName>/<foodName>1,2,3
    /// </summary>
    private Sprite[] LoadSprites(string foodName)
    {
        List<Sprite> list = new List<Sprite>();

        string basePath = $"foods/{foodName}/{foodName}";

        for (int i = 1; i <= 3; i++)
        {
            Sprite s = Resources.Load<Sprite>($"{basePath}{i}");
            if (s != null)
                list.Add(s);
        }

        return list.ToArray();
    }

    /// <summary>
    /// Moves a transform smoothly to a target position over duration.
    /// </summary>
    private IEnumerator MoveToPosition(Transform t, Vector3 target, float duration)
    {
        Vector3 start = t.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            t.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        t.position = target;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// FoodSpawner: spawns foods at a fixed spawn position and moves them to a target position.
// It loads sprites from Assets/foods/{name}/{name}1.png in the Editor, or Resources/foods/{name}/{name}1 at runtime.
public class FoodSpawner : MonoBehaviour
{
    public Vector3 spawnPosition = new Vector3(4f, 2.5f, 0f);
    public Vector3 targetPosition = new Vector3(0f, 2.5f, 0f);
    public float moveDuration = 0.5f;
    public float spawnDelay = 0.15f; // delay between spawning each food

    // Optional override list of food names (if empty the spawner will auto-detect in Editor or use fallback list)
    public string[] foodNameOverrides;

    private List<string> availableFoodNames = new List<string>();
    private int nextIndex = 0;

    void Awake()
    {
        // Build the internal food name list once
        if (foodNameOverrides != null && foodNameOverrides.Length > 0)
        {
            availableFoodNames.AddRange(foodNameOverrides);
        }

#if UNITY_EDITOR
        if (availableFoodNames.Count == 0)
        {
            if (AssetDatabase.IsValidFolder("Assets/foods"))
            {
                string[] sub = AssetDatabase.GetSubFolders("Assets/foods");
                foreach (var s in sub)
                {
                    availableFoodNames.Add(Path.GetFileName(s));
                }
            }
        }
#endif

        if (availableFoodNames.Count == 0)
        {
            availableFoodNames.AddRange(new string[] { "burg", "cola", "fries", "gyros", "nuggies" });
        }
    }

    // Spawn a single next food GameObject and return it
    public GameObject SpawnNextFood()
    {
        if (availableFoodNames.Count == 0)
        {
            Debug.LogError("FoodSpawner: No food names available to spawn.");
            return null;
        }

        string foodName = availableFoodNames[nextIndex];
        nextIndex = (nextIndex + 1) % availableFoodNames.Count;

        Sprite s1 = null;
        Sprite s2 = null;
        Sprite s3 = null;

#if UNITY_EDITOR
        string p1 = $"Assets/foods/{foodName}/{foodName}1.png";
        string p2 = $"Assets/foods/{foodName}/{foodName}2.png";
        string p3 = $"Assets/foods/{foodName}/{foodName}3.png";
        s1 = AssetDatabase.LoadAssetAtPath<Sprite>(p1);
        s2 = AssetDatabase.LoadAssetAtPath<Sprite>(p2);
        s3 = AssetDatabase.LoadAssetAtPath<Sprite>(p3);
#endif

        if (s1 == null)
        {
            s1 = Resources.Load<Sprite>($"foods/{foodName}/{foodName}1");
            s2 = Resources.Load<Sprite>($"foods/{foodName}/{foodName}2");
            s3 = Resources.Load<Sprite>($"foods/{foodName}/{foodName}3");
        }

        if (s1 == null)
        {
            Debug.LogWarning($"FoodSpawner: Could not find sprites for '{foodName}'. Skipping spawn.");
            return null;
        }

        GameObject go = new GameObject(foodName);
        go.transform.SetParent(this.transform);
        go.transform.position = spawnPosition;
        go.transform.localScale = Vector3.one * 3.5f;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = s1;

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = false;

        var fs = go.AddComponent<FoodScript>();
        // Assign sprites using the public Init method
        var spriteList = new List<Sprite>();
        if (s1 != null) spriteList.Add(s1);
        if (s2 != null) spriteList.Add(s2);
        if (s3 != null) spriteList.Add(s3);
        fs.Init(spriteList.ToArray());
        Debug.Log($"FoodSpawner: Called Init for {foodName} with {spriteList.Count} sprites");

        GameObject target = GameObject.Find("FoodTarget") ?? new GameObject("FoodTarget");
        target.transform.position = targetPosition;
        fs.MoveToTarget(targetPosition);

        StartCoroutine(MoveToPosition(go.transform, targetPosition, moveDuration));

        return go;
    }

    IEnumerator MoveToPosition(Transform t, Vector3 target, float duration)
    {
        float elapsed = 0f;
        Vector3 start = t.position;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / duration);
            t.position = Vector3.Lerp(start, target, p);
            yield return null;
        }
        t.position = target;
    }
}

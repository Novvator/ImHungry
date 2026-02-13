using System.Collections.Generic;
using UnityEngine;

public class UnlockDatabase : MonoBehaviour
{
    public static UnlockDatabase Instance;

    [Header("Food Sprites")]
    [SerializeField] private Sprite ramenSprite;
    [SerializeField] private Sprite pizzaSprite;
    [SerializeField] private Sprite sushiSprite;

    [Header("Trail Sprites")]
    [SerializeField] private Sprite flameTrailSprite;
    [SerializeField] private Sprite sparkleTrailSprite;

    private Dictionary<string, List<UnlockItem>> unlocksByLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BuildDatabase()
    {
        unlocksByLevel = new Dictionary<string, List<UnlockItem>>();

        // Level 1 unlocks
        unlocksByLevel.Add("Level1", new List<UnlockItem>()
        {
            new UnlockItem { itemName = "Spicy Ramen", itemSprite = ramenSprite }
        });

        // Level 2 unlocks
        unlocksByLevel.Add("Level2", new List<UnlockItem>()
        {
            new UnlockItem { itemName = "Pizza Slice", itemSprite = pizzaSprite },
            new UnlockItem { itemName = "Flame Trail", itemSprite = flameTrailSprite }
        });

        // Level 3 unlocks
        unlocksByLevel.Add("Level3", new List<UnlockItem>()
        {
            new UnlockItem { itemName = "Sushi Roll", itemSprite = sushiSprite },
            new UnlockItem { itemName = "Sparkle Trail", itemSprite = sparkleTrailSprite }
        });
    }

    public List<UnlockItem> GetUnlocksForLevel(string levelName)
    {
        if (unlocksByLevel.ContainsKey(levelName))
            return unlocksByLevel[levelName];

        return null;
    }
}

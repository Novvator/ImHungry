using UnityEngine;

// Single place for world/stage layout and the PlayerPrefs keys used for progression.
public static class StageProgress
{
    // Number of stages in each world, in order. Add an entry here to add a world.
    public static readonly int[] StagesPerWorld = { 5, 5, 5 };

    public static int TotalStages
    {
        get
        {
            int total = 0;
            foreach (int count in StagesPerWorld) total += count;
            return total;
        }
    }

    // Returns the 1-based world number for a 1-based stage number
    public static int WorldOfStage(int stage)
    {
        int last = 0;
        for (int i = 0; i < StagesPerWorld.Length; i++)
        {
            last += StagesPerWorld[i];
            if (stage <= last) return i + 1;
        }
        return StagesPerWorld.Length;
    }

    public static int FirstStageOfWorld(int world)
    {
        int first = 1;
        for (int i = 0; i < world - 1 && i < StagesPerWorld.Length; i++) first += StagesPerWorld[i];
        return first;
    }

    public static bool IsLastStageOfWorld(int stage)
    {
        int world = WorldOfStage(stage);
        return stage == FirstStageOfWorld(world) + StagesPerWorld[world - 1] - 1;
    }

    public static string LevelMenuForWorld(int world)
    {
        return world <= 1 ? "Level Menu" : "Level Menu " + world;
    }

    public static bool IsStageCompleted(int stage)
    {
        return PlayerPrefs.HasKey("Stage " + stage + " Completed");
    }

    // Stage 1 is always open; every other stage opens when the previous one is completed
    public static bool IsStageUnlocked(int stage)
    {
        return stage <= 1 || IsStageCompleted(stage - 1);
    }

    public static bool IsWorldUnlocked(int world)
    {
        return world <= 1 || PlayerPrefs.HasKey("World " + world + " Unlocked");
    }

    public static void MarkStageCompleted(string sceneName)
    {
        PlayerPrefs.SetInt(sceneName + " Completed", 1);
        PlayerPrefs.Save();
    }

    public static void UnlockWorld(string worldName)
    {
        PlayerPrefs.SetInt(worldName + " Unlocked", 1);
        PlayerPrefs.Save();
    }

    // Extracts the stage number from a scene name like "Stage 12"; returns -1 if there is none
    public static int StageNumberFromScene(string sceneName)
    {
        var match = System.Text.RegularExpressions.Regex.Match(sceneName, @"\d+");
        return match.Success ? int.Parse(match.Value) : -1;
    }
}

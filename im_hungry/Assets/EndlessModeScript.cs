using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Endless (score attack) mode. Same single-finger tracing as PatternScript, but:
/// - every food eaten adds time to the clock (bonus shrinks as you go)
/// - eating in a row builds a combo multiplier, slipping off the path resets it
/// - patterns start rotating/flipping after a few foods
/// - when time runs out the score is saved locally and posted to the
///   Google Play Games leaderboard through LeaderboardService
/// </summary>
public class EndlessModeScript : PatternScript
{
    public const string SceneName = "Endless";
    public const string BestScoreKey = "Endless Best";

    [Header("Scoring")]
    [SerializeField] int pointsPerFood = 10;
    [SerializeField] int foodsPerMultiplierStep = 3;
    [SerializeField] int maxMultiplier = 5;

    [Header("Time bonus")]
    [SerializeField] float startTimeBonus = 3f;
    [SerializeField] float minTimeBonus = 1f;
    [SerializeField] float timeBonusDecayPerFood = 0.1f;

    [Header("Difficulty")]
    [SerializeField] int foodsBeforeRandomTransform = 5;

    Text scoreText;
    int score = 0;
    int combo = 0;
    int foodsEaten = 0;

    int Multiplier
    {
        get { return Mathf.Min(maxMultiplier, 1 + combo / Mathf.Max(1, foodsPerMultiplierStep)); }
    }

    protected override bool ShouldRandomizeTransform => foodsEaten >= foodsBeforeRandomTransform;

    protected override IEnumerator Start()
    {
        LeaderboardService.SignIn();

        // The stage HUD shows "score/goal"; this mode drives the text itself
        ScoreScript stageScore = FindObjectOfType<ScoreScript>();
        if (stageScore != null)
        {
            scoreText = stageScore.GetComponent<Text>();
            stageScore.enabled = false;
        }

        yield return base.Start();
        UpdateScoreText();
    }

    protected override void AddPatternScore()
    {
        combo++;
        foodsEaten++;
        score += pointsPerFood * Multiplier;
        AddTime(Mathf.Max(minTimeBonus, startTimeBonus - timeBonusDecayPerFood * (foodsEaten - 1)));
        UpdateScoreText();
    }

    protected override void OnAttemptFailed()
    {
        if (combo == 0) return;
        combo = 0;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText == null) return;
        scoreText.text = "Score\n" + score + (Multiplier > 1 ? "  x" + Multiplier : "");
    }

    protected override void OnTimeUp()
    {
        int best = PlayerPrefs.GetInt(BestScoreKey, 0);
        bool newBest = score > best;
        if (newBest)
        {
            best = score;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        LeaderboardService.SubmitScore(score);

        foreach (Transform child in EndCanvas.transform)
        {
            string n = child.gameObject.name;
            if (n == "YOU WIN!" || n == "YOU LOSE" || n == "nextStage")
                child.gameObject.SetActive(false);
            else if (n == "burgerdance")
                child.gameObject.SetActive(newBest);
            else if (n == "burgercry")
                child.gameObject.SetActive(!newBest);
        }

        SoundManagerScript.PlaySound(newBest ? "win" : "end");
        BuildResultsUI(newBest, best);
    }

    // Built in code so the mode needs no new prefabs; restyle here if it overlaps the art
    void BuildResultsUI(bool newBest, int best)
    {
        Font font = CountdownText.font;

        string title = newBest ? "NEW BEST!" : "TIME'S UP!";
        CreateText(EndCanvas.transform, "EndlessResults", font,
            title + "\nScore: " + score + "\nBest: " + best,
            new Vector2(0.5f, 0.78f), new Vector2(1200, 400), 90);

        if (LeaderboardService.IsAvailable)
        {
            CreateButton(EndCanvas.transform, "LeaderboardButton", font, "LEADERBOARD",
                new Vector2(0.5f, 0.42f), new Vector2(700, 150), LeaderboardService.ShowLeaderboard);
        }
    }

    static Text CreateText(Transform parent, string name, Font font, string content,
        Vector2 anchor, Vector2 size, int fontSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.sizeDelta = size;

        Text t = go.GetComponent<Text>();
        t.font = font;
        t.fontSize = fontSize;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.text = content;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(4, -4);
        return t;
    }

    static void CreateButton(Transform parent, string name, Font font, string label,
        Vector2 anchor, Vector2 size, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.sizeDelta = size;

        go.GetComponent<Image>().color = new Color(1f, 0.72f, 0f);
        go.GetComponent<Button>().onClick.AddListener(onClick);

        Text t = CreateText(go.transform, "Label", font, label, new Vector2(0.5f, 0.5f), size, 70);
        t.color = Color.black;
        Destroy(t.GetComponent<Outline>());
    }
}

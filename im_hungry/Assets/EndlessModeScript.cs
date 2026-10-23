using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Endless (score attack) mode. Same tracing rules as PatternScript, but:
/// - every food eaten adds time to the clock (bonus shrinks as you go)
/// - eating in a row builds a combo multiplier, slipping off the path resets it
/// - patterns start rotating/flipping after a few foods
/// - when time runs out the score is saved locally and posted to the
///   Google Play Games leaderboard through LeaderboardService
/// </summary>
public class EndlessModeScript : MonoBehaviour
{
    public const string SceneName = "Endless";
    public const string BestScoreKey = "Endless Best";

    private GameObject chosenpat;
    private GameObject chosenfood;
    private FoodScript chosenfoodscript;

    float currentTime = 0f;
    bool startTimer = false;
    bool isPaused = false;

    [SerializeField] GameObject Trail;
    [SerializeField] float startingTime = 30f;
    [SerializeField] Text countdownText;
    [SerializeField] Text scoreText;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject endCanvasUI;
    [SerializeField] private GameObject[] tubes;

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

    private FoodSpawner spawner;

    public bool hitted = false;
    bool trg1 = false;
    bool trg2 = false;
    private Queue<int> patternQueue;
    private int lastPattern = -1;
    bool gameOver = false;
    private bool isInitialized = false;

    int score = 0;
    int combo = 0;
    int foodsEaten = 0;

    private bool[] countdownSoundPlayed = new bool[3];

    [SerializeField] private Image redFlashImage;
    public float redFlashDuration = 0.45f;
    private Color originalColor;
    private bool redFlashIsOnCooldown = false;
    [SerializeField] private float redFlashCooldown = 0.5f;

    int Multiplier
    {
        get { return Mathf.Min(maxMultiplier, 1 + combo / Mathf.Max(1, foodsPerMultiplierStep)); }
    }

    IEnumerator Start()
    {
        LeaderboardService.SignIn();

        spawner = FindObjectOfType<FoodSpawner>();
        if (spawner == null)
        {
            GameObject spgo = new GameObject("FoodSpawner");
            spawner = spgo.AddComponent<FoodSpawner>();
        }

        // The stage HUD shows "score/goal"; this mode drives the text itself
        if (scoreText == null)
        {
            ScoreScript stageScore = FindObjectOfType<ScoreScript>();
            if (stageScore != null) scoreText = stageScore.GetComponent<Text>();
        }
        foreach (ScoreScript s in FindObjectsOfType<ScoreScript>())
        {
            s.enabled = false;
        }

        if (tubes == null || tubes.Length == 0)
        {
            Debug.LogError("EndlessModeScript: No pattern tubes assigned.");
            yield break;
        }

        chosenfood = spawner.SpawnNextFood();
        if (chosenfood == null)
        {
            Debug.LogError("EndlessModeScript: Failed to spawn initial food.");
            yield break;
        }

        originalColor = redFlashImage.color;
        patternQueue = InitializeQueue(tubes.Length);
        InitializePatternsAndFood();
        currentTime = startingTime;
        countdownText.text = currentTime.ToString("N1", CultureInfo.InvariantCulture);
        pauseMenuUI.SetActive(false);
        endCanvasUI.SetActive(false);
        UpdateScoreText();
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized || isPaused) return;

        HandleInput();
        UpdateTimer();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void InitializePatternsAndFood()
    {
        chosenpat = tubes[GetNextPattern()];
        ApplyDifficulty(chosenpat);

        foreach (GameObject tube in tubes)
        {
            tube.SetActive(tube == chosenpat);
        }

        chosenfoodscript = chosenfood.GetComponent<FoodScript>();
        chosenfood.transform.localScale = Vector3.one * 3.5f;
        chosenfood.SetActive(true);
        chosenfoodscript.OnStage1();
    }

    void HandleInput()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId != 0)
            {
                continue;
            }

            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.zero);
            if (touch.phase == TouchPhase.Began)
            {
                Instantiate(Trail, pos, Quaternion.identity);
            }

            if (gameOver) continue;

            if (hit.collider != null && touch.phase != TouchPhase.Ended)
            {
                HandlePatternInteraction(hit);
            }
            else
            {
                ResetOnTouchRelease();
            }
        }
    }

    void HandlePatternInteraction(RaycastHit2D hit)
    {
        if (hit.collider.name == "fdcc")
        {
            hitted = true;
            startTimer = true;
            chosenpat.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }

        if (hit.collider.name == chosenpat.transform.GetChild(1).name && hitted)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.GetChild(1).gameObject.SetActive(false);
            chosenfoodscript.OnStage2();
            trg1 = true;
        }

        if (hit.collider.name == chosenpat.transform.GetChild(2).name && hitted && trg1)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.GetChild(2).gameObject.SetActive(false);
            chosenfoodscript.OnStage3();
            trg2 = true;
        }

        if (hit.collider.name == chosenpat.transform.GetChild(3).name && hitted && trg1 && trg2)
        {
            OnFoodEaten();
        }
    }

    void OnFoodEaten()
    {
        SoundManagerScript.PlaySound("okpattern");

        combo++;
        foodsEaten++;
        score += pointsPerFood * Multiplier;
        currentTime += Mathf.Max(minTimeBonus, startTimeBonus - timeBonusDecayPerFood * (foodsEaten - 1));
        // Allow the 3-2-1 warning to play again if the bonus pulled us back above it
        for (int i = 0; i < countdownSoundPlayed.Length; i++)
        {
            if (currentTime > 3 - i) countdownSoundPlayed[i] = false;
        }
        UpdateScoreText();

        // reset current pattern so it is clean when it comes round again
        chosenpat.transform.GetChild(1).gameObject.SetActive(true);
        chosenpat.transform.GetChild(2).gameObject.SetActive(true);
        chosenpat.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        chosenpat.SetActive(false);

        chosenpat = tubes[GetNextPattern()];
        ApplyDifficulty(chosenpat);
        chosenpat.SetActive(true);

        if (chosenfood != null)
        {
            Destroy(chosenfood);
        }
        chosenfood = spawner.SpawnNextFood();
        if (chosenfood != null)
        {
            chosenfoodscript = chosenfood.GetComponent<FoodScript>();
            chosenfood.transform.localScale = Vector3.one * 3.5f;
            chosenfood.SetActive(true);
        }

        trg1 = false;
        trg2 = false;
        hitted = false;

        StartCoroutine(StartRedFlashCooldown());
    }

    void ApplyDifficulty(GameObject pattern)
    {
        Vector3 scale = pattern.transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        scale.y = Mathf.Abs(scale.y);

        if (foodsEaten < foodsBeforeRandomTransform)
        {
            pattern.transform.rotation = Quaternion.identity;
            pattern.transform.localScale = scale;
            return;
        }

        int[] angles = { 0, 90, 180, 270 };
        pattern.transform.rotation = Quaternion.Euler(0, 0, angles[Random.Range(0, angles.Length)]);

        int flip = Random.Range(0, 3);
        if (flip == 1) scale.x = -scale.x;
        else if (flip == 2) scale.y = -scale.y;
        pattern.transform.localScale = scale;
    }

    void ResetOnTouchRelease()
    {
        if (hitted)
        {
            if (!redFlashIsOnCooldown)
            {
                SoundManagerScript.PlaySound("fail");
                StartCoroutine(PlayRedFlash());
                StartCoroutine(StartRedFlashCooldown());
            }
            hitted = false;
            trg1 = false;
            trg2 = false;

            if (combo > 0)
            {
                combo = 0;
                UpdateScoreText();
            }

            chosenpat.transform.GetChild(1).gameObject.SetActive(true);
            chosenpat.transform.GetChild(2).gameObject.SetActive(true);
            chosenpat.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

            chosenfoodscript.OnStage1();
        }
    }

    void UpdateScoreText()
    {
        if (scoreText == null) return;
        scoreText.text = "Score\n" + score + (Multiplier > 1 ? "  x" + Multiplier : "");
    }

    void UpdateTimer()
    {
        if (!startTimer || gameOver) return;

        currentTime -= Time.deltaTime;
        if (currentTime < 0) currentTime = 0;
        countdownText.text = currentTime.ToString("N1", CultureInfo.InvariantCulture);

        int roundedTime = Mathf.CeilToInt(currentTime);
        for (int i = 0; i < countdownSoundPlayed.Length; i++)
        {
            if (roundedTime == 3 - i && !countdownSoundPlayed[i])
            {
                SoundManagerScript.PlaySound("ending");
                countdownSoundPlayed[i] = true;
            }
        }

        if (currentTime == 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        gameOver = true;
        pauseButton.SetActive(false);
        chosenpat.SetActive(false);
        if (chosenfood != null) chosenfood.SetActive(false);

        int best = PlayerPrefs.GetInt(BestScoreKey, 0);
        bool newBest = score > best;
        if (newBest)
        {
            best = score;
            PlayerPrefs.SetInt(BestScoreKey, best);
            PlayerPrefs.Save();
        }

        LeaderboardService.SubmitScore(score);

        endCanvasUI.SetActive(true);
        foreach (Transform child in endCanvasUI.transform)
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

    // Built in code so the mode needs no new prefabs; restyle in the editor if you like
    void BuildResultsUI(bool newBest, int best)
    {
        Font font = countdownText.font;

        string title = newBest ? "NEW BEST!" : "TIME'S UP!";
        CreateText(endCanvasUI.transform, "EndlessResults", font,
            title + "\nScore: " + score + "\nBest: " + best,
            new Vector2(0.5f, 0.78f), new Vector2(1200, 400), 90);

        if (LeaderboardService.IsAvailable)
        {
            CreateButton(endCanvasUI.transform, "LeaderboardButton", font, "LEADERBOARD",
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

    // shuffling queue (same approach as PatternScript)
    private Queue<int> InitializeQueue(int count)
    {
        List<int> items = new List<int>();
        for (int i = 0; i < count; i++)
        {
            items.Add(i);
        }
        ShuffleList(items);
        return new Queue<int>(items);
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private int GetNextPattern()
    {
        int current = patternQueue.Dequeue();
        if (current == lastPattern && patternQueue.Count > 0)
        {
            patternQueue.Enqueue(current);
            current = patternQueue.Dequeue();
        }
        lastPattern = current;
        patternQueue.Enqueue(current);

        List<int> list = new List<int>(patternQueue);
        ShuffleList(list);
        patternQueue = new Queue<int>(list);
        return current;
    }

    private IEnumerator StartRedFlashCooldown()
    {
        redFlashIsOnCooldown = true;
        yield return new WaitForSeconds(redFlashCooldown);
        redFlashIsOnCooldown = false;
    }

    IEnumerator PlayRedFlash()
    {
        redFlashImage.gameObject.SetActive(true);

        float startTime = Time.time;
        while (Time.time - startTime < redFlashDuration)
        {
            float normalizedTime = (Time.time - startTime) / redFlashDuration;
            redFlashImage.color = Color.Lerp(originalColor, Color.clear, normalizedTime);
            yield return null;
        }

        redFlashImage.color = originalColor;
        redFlashImage.gameObject.SetActive(false);
    }
}

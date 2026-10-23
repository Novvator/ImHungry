using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Shared game loop for the pattern stages: food spawning, pattern rotation, timer,
// pause, red flash, scoring and win/lose. Subclasses only handle touch input and
// how a pattern's checkpoints are tracked (one finger or two).
// Serialized field names must stay as they are: the stage scenes store values under these names.
public abstract class PatternGameBase : MonoBehaviour
{
    protected GameObject chosenpat;
    protected GameObject chosenfood;
    protected FoodScript chosenfoodscript;

    float currentTime = 0f;
    protected bool startTimer = false;
    bool isPaused = false;

    [SerializeField] float startingTime = 30f;
    [SerializeField] int scoreGoal;
    [SerializeField] string unlockWorld;
    [SerializeField] Text countdownText;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject endCanvasUI;
    [SerializeField] private bool randomizeTransform = false;

    List<GameObject> winComponents = new List<GameObject>();
    List<GameObject> loseComponents = new List<GameObject>();

    // Foods are created at runtime by FoodSpawner; do not assign in the inspector
    private FoodSpawner spawner;
    [SerializeField] private GameObject[] tubes;

    private Queue<int> patternQueue;
    private int lastPattern = -1;
    protected bool completedlvl = false;
    // Set to true once Start() has finished initialization (spawner spawned foods and arrays populated)
    private bool isInitialized = false;

    private bool[] countdownSoundPlayed = new bool[3]; // To track sounds for 3, 2, and 1 seconds
    bool endhasPlayed = false;

    [SerializeField] private Image redFlashImage; // Assign the UI Image in the Inspector
    public float redFlashDuration = 0.45f; // Duration of the red flash effect
    private Color originalColor;
    private bool redFlashIsOnCooldown = false; // Red Flash Cooldown flag
    [SerializeField] private float redFlashCooldown = 0.5f; // Cooldown duration in seconds

    [SerializeField] private GameObject UnlockMessagePopup;
    private MessagePopupScript PopupScript;

    // Reads touches and calls HandlePatternInteraction / FailAttempt
    protected abstract void HandleInput();

    // True while the player is partway through the current pattern
    protected abstract bool IsTracing { get; }

    // Clears checkpoint flags and re-enables the current pattern's checkpoints and start circles
    protected abstract void ResetPatternState();

    // Reacts to the player touching one of the pattern's colliders
    protected abstract void HandlePatternInteraction(RaycastHit2D hit);

    private void ApplyRandomFlip(GameObject pattern)
    {
        int choice = Random.Range(0, 3);
        // 0 = no flip
        // 1 = horizontal flip
        // 2 = vertical flip

        Vector3 scale = pattern.transform.localScale;

        switch (choice)
        {
            case 1: // horizontal flip
                scale.x = -Mathf.Abs(scale.x);
                break;

            case 2: // vertical flip
                scale.y = -Mathf.Abs(scale.y);
                break;

            default: // no flip
                scale.x = Mathf.Abs(scale.x);
                scale.y = Mathf.Abs(scale.y);
                break;
        }

        pattern.transform.localScale = scale;
    }

    private void ApplyRandomTransform(GameObject pattern)
    {
        if (!randomizeTransform) return; // If checkbox is OFF, keep default transform

        int[] angles = { 0, 90, 180, 270 };
        int angle = angles[Random.Range(0, angles.Length)];
        pattern.transform.rotation = Quaternion.Euler(0, 0, angle);
        ApplyRandomFlip(pattern);
    }

    // Start is called before the first frame update
    protected virtual IEnumerator Start()
    {
        // Ensure a FoodSpawner exists and keep a reference to it
        spawner = FindObjectOfType<FoodSpawner>();
        if (spawner == null)
        {
            GameObject spgo = new GameObject("FoodSpawner");
            spawner = spgo.AddComponent<FoodSpawner>();
        }

        // Spawn the first food immediately
        if (!SpawnNextFood())
        {
            Debug.LogError(GetType().Name + ": Failed to spawn initial food.");
            isInitialized = true;
            yield break;
        }

        // Validate we have tubes before proceeding
        if (tubes == null || tubes.Length == 0)
        {
            Debug.LogError(GetType().Name + ": No pattern tubes found. Assign tubes in the inspector or ensure tubes exist in the scene.");
            isInitialized = true;
            yield break;
        }

        originalColor = redFlashImage.color;
        patternQueue = InitializeQueue(tubes.Length);
        InitializePatterns();
        GetEndCanvasChildren();
        currentTime = startingTime;
        pauseMenuUI.SetActive(false);
        endCanvasUI.SetActive(false);
        ScoreScript.scoreGoal = scoreGoal;
        if (UnlockMessagePopup != null) { PopupScript = UnlockMessagePopup.GetComponent<MessagePopupScript>(); }
        // Mark initialization complete so Update() can run safely
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return; // Wait until Start() finished setup

        if (!isPaused)
        {
            HandleInput();
            UpdateTimer();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume normal time scale
        isPaused = false;
    }

    void InitializePatterns()
    {
        chosenpat = tubes[GetNextPattern()];
        ApplyRandomTransform(chosenpat);

        foreach (GameObject tube in tubes)
        {
            tube.SetActive(tube == chosenpat);
        }

        // Ensure it starts at stage 1
        chosenfoodscript.OnStage1();
    }

    // Spawns the next food from the spawner; returns false if nothing could be spawned
    bool SpawnNextFood()
    {
        chosenfood = spawner != null ? spawner.SpawnNextFood() : null;
        if (chosenfood == null)
        {
            chosenfoodscript = null;
            return false;
        }

        chosenfoodscript = chosenfood.GetComponent<FoodScript>();
        chosenfood.transform.localScale = Vector3.one * 3.5f;
        chosenfood.SetActive(true);
        return true;
    }

    // Moves the food to the given eating stage (2 or 3), never backwards
    protected void AdvanceFood(int stage)
    {
        if (chosenfoodscript == null || chosenfoodscript.currentStage >= stage) return;

        if (stage == 2) chosenfoodscript.OnStage2();
        else if (stage == 3) chosenfoodscript.OnStage3();
    }

    // Called when the whole pattern has been traced
    protected void CompletePattern()
    {
        //play sound
        SoundManagerScript.PlaySound("okpattern");

        //reset checkpoints of the finished pattern so it is ready next time it comes up
        ResetPatternState();

        //update score
        ScoreScript.scoreValue += 10;

        //disable previous pattern
        chosenpat.SetActive(false);

        // choose next pattern
        chosenpat = tubes[GetNextPattern()];
        ApplyRandomTransform(chosenpat);
        chosenpat.SetActive(true);

        // destroy previous food and spawn the next one
        if (chosenfood != null)
        {
            Destroy(chosenfood);
        }
        SpawnNextFood();

        //cooldown on red flash to avoid flashing on start of next pattern
        StartCoroutine(StartRedFlashCooldown());
    }

    // Called when a finger leaves the pattern or lifts off
    protected void FailAttempt()
    {
        if (!IsTracing) return;

        Debug.Log("Lost target");
        if (!redFlashIsOnCooldown)
        {
            SoundManagerScript.PlaySound("fail");
            StartCoroutine(PlayRedFlash());
            StartCoroutine(StartRedFlashCooldown());
        }

        ResetPatternState();

        //when leaving tube reset the food to its first sprite
        if (chosenfoodscript != null) chosenfoodscript.OnStage1();
    }

    // shuffling queue
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

    private void ShuffleQueue(Queue<int> queue)
    {
        List<int> list = new List<int>(queue);
        ShuffleList(list);
        queue.Clear();
        foreach (int item in list)
        {
            queue.Enqueue(item);
        }
    }

    private int GetNextItem(Queue<int> queue, ref int lastItem)
    {
        int currentItem = queue.Dequeue();
        if (currentItem == lastItem && queue.Count > 0)
        {
            queue.Enqueue(currentItem);
            currentItem = queue.Dequeue();
        }
        lastItem = currentItem;
        queue.Enqueue(currentItem);
        ShuffleQueue(queue);
        return currentItem;
    }

    public int GetNextPattern()
    {
        return GetNextItem(patternQueue, ref lastPattern);
    }

    private IEnumerator StartRedFlashCooldown()
    {
        redFlashIsOnCooldown = true; // Set cooldown flag
        yield return new WaitForSeconds(redFlashCooldown); // Wait for the cooldown duration
        redFlashIsOnCooldown = false; // Reset cooldown flag
    }

    IEnumerator PlayRedFlash()
    {
        redFlashImage.gameObject.SetActive(true);

        // Smoothly fade in the red flash
        float startTime = Time.time;
        while (Time.time - startTime < redFlashDuration)
        {
            float normalizedTime = (Time.time - startTime) / redFlashDuration;
            redFlashImage.color = Color.Lerp(originalColor, Color.clear, normalizedTime);
            yield return null;
        }

        // Restore the original color
        redFlashImage.color = originalColor;
        redFlashImage.gameObject.SetActive(false);
    }

    void UpdateTimer()
    {
        if (startTimer == true)
        {
            currentTime -= 1 * Time.deltaTime;
            countdownText.text = currentTime.ToString("N1", CultureInfo.InvariantCulture);

            int roundedTime = Mathf.CeilToInt(currentTime);
            if (roundedTime == 3 && !countdownSoundPlayed[0])
            {
                SoundManagerScript.PlaySound("ending");
                countdownSoundPlayed[0] = true;
            }
            else if (roundedTime == 2 && !countdownSoundPlayed[1])
            {
                SoundManagerScript.PlaySound("ending");
                countdownSoundPlayed[1] = true;
            }
            else if (roundedTime == 1 && !countdownSoundPlayed[2])
            {
                SoundManagerScript.PlaySound("ending");
                countdownSoundPlayed[2] = true;
            }

            if (currentTime <= 0)
            {
                currentTime = 0;
            }

            if (currentTime == 0)
            {
                //play end sound
                endCanvasUI.SetActive(true);
                if (!endhasPlayed) { checkIfWin(); }
                endhasPlayed = true;

                completedlvl = true;
                pauseButton.SetActive(false);
                chosenpat.SetActive(false);
                if (chosenfood != null) chosenfood.SetActive(false);
            }
        }
    }

    void GetEndCanvasChildren()
    {
        string[] winlist = { "burgerdance", "YOU WIN!", "nextStage" };
        string[] loselist = { "burgercry", "YOU LOSE" };

        foreach (Transform child in endCanvasUI.transform)
        {
            if (System.Array.Exists(winlist, element => element == child.gameObject.name))
            {
                winComponents.Add(child.gameObject);
            }
            else if (System.Array.Exists(loselist, element => element == child.gameObject.name))
            {
                loseComponents.Add(child.gameObject);
            }
        }
    }

    void checkIfWin()
    {
        if (ScoreScript.scoreValue >= scoreGoal)
        {
            SetComponentsActive(winComponents, true);
            SoundManagerScript.PlaySound("win");
            //Set stage completed
            StageProgress.MarkStageCompleted(SceneManager.GetActiveScene().name);

            if (!string.IsNullOrWhiteSpace(unlockWorld))
            {
                StageProgress.UnlockWorld(unlockWorld);
                if (PopupScript != null)
                {
                    PopupScript.ShowUnlockMessage("YOU HAVE UNLOCKED " + unlockWorld + "!");
                }
                else
                {
                    Debug.LogWarning(GetType().Name + ": UnlockMessagePopup is not assigned, skipping unlock message.");
                }
            }
        }
        else
        {
            SetComponentsActive(loseComponents, true);
            SoundManagerScript.PlaySound("end");
        }
    }

    // Helper method to set active state of components
    void SetComponentsActive(List<GameObject> components, bool isActive)
    {
        foreach (GameObject component in components)
        {
            component.SetActive(isActive);
        }
    }
}

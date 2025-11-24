//using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PatternScript : MonoBehaviour
{
    private GameObject chosenpat;
    private GameObject chosenfood;
    private FoodScript chosenfoodscript;
    private GameObject activetrail_1;
    private GameObject activetrail_2;

    private TrailScript activetrailscript_1;
    private TrailScript activetrailscript_2;

    float currentTime = 0f;
    bool startTimer = false;
    bool isPaused = false;

    [SerializeField] GameObject Trail;
    [SerializeField] float startingTime = 30f;
    [SerializeField] int scoreGoal;
    [SerializeField] string unlockWorld;
    [SerializeField] Text countdownText;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject endCanvasUI;

    List<GameObject> winComponents = new List<GameObject>();
    List<GameObject> loseComponents = new List<GameObject>();

    // Foods are created at runtime by FoodSpawner; do not assign in the inspector
    private GameObject[] food;
    private FoodSpawner spawner;
    [SerializeField] private GameObject[] tubes;

    public bool hitted_1 = false;
    public bool hitted_2 = false;
    bool trg1_1 = false;
    bool trg1_2 = false;
    bool trg2_1 = false;
    bool trg2_2 = false;
    int num;
    int num2;
    int lastnum;
    int lastnum2;
    private Queue<int> patternQueue;
    private int lastPattern;
    bool completedlvl = false;
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

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // Ensure a FoodSpawner exists and keep a reference to it
        spawner = FindObjectOfType<FoodSpawner>();
        if (spawner == null)
        {
            GameObject spgo = new GameObject("FoodSpawner");
            spawner = spgo.AddComponent<FoodSpawner>();
        }

        // Spawn the first food immediately
        chosenfood = spawner.SpawnNextFood();
        if (chosenfood == null)
        {
            Debug.LogError("PatternScript: Failed to spawn initial food.");
            isInitialized = true;
            yield break;
        }
        chosenfoodscript = chosenfood.GetComponent<FoodScript>();
        chosenfood.transform.localScale = Vector3.one * 3.5f;

        // Validate we have tubes before proceeding
        if (tubes == null || tubes.Length == 0)
        {
            Debug.LogError("PatternScript: No pattern tubes found. Assign tubes in the inspector or ensure tubes exist in the scene.");
            isInitialized = true;
            yield break;
        }

        originalColor = redFlashImage.color;
        patternQueue = InitializeQueue(tubes.Length);
        InitializePatternsAndFood();
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

    void InitializePatternsAndFood()
    {
        chosenpat = tubes[GetNextPattern()];

        foreach (GameObject tube in tubes)
        {
            tube.SetActive(tube == chosenpat);
        }

        // Ensure we have a chosenfood (spawned in Start). If not, spawn one now.
        if (chosenfood == null && spawner != null)
        {
            chosenfood = spawner.SpawnNextFood();
        }
        if (chosenfood != null)
        {
            chosenfoodscript = chosenfood.GetComponent<FoodScript>();
            chosenfood.transform.localScale = Vector3.one * 3.5f;
            chosenfood.SetActive(true);
            // Ensure it starts at stage 1
            chosenfoodscript.OnStage1();
        }
    }

    void HandleInput()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId > 1)
            {
                continue; // Ignore touches with different IDs
            }

            if (touch.fingerId == 0)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit_1 = Physics2D.Raycast(pos, Vector3.zero);
                if (touch.phase == TouchPhase.Began)
                {
                    
                    
                    activetrail_1 = Instantiate(Trail, pos, Quaternion.identity);
                    activetrailscript_1 = activetrail_1.GetComponent<TrailScript>();
                    //activetrailscript.InitTouch(touch);
                
                }
                if (hit_1 != null && hit_1.collider != null && touch.phase != TouchPhase.Ended)
                    {
                        if (!completedlvl) HandlePatternInteraction(hit_1);
                    }
                else
                    {   
                        if (!completedlvl) ResetOnTouchRelease();
                    }
            }
            if (touch.fingerId == 1)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit_2 = Physics2D.Raycast(pos, Vector3.zero);
                if (touch.phase == TouchPhase.Began)
                {
                    
                    
                    activetrail_2 = Instantiate(Trail, pos, Quaternion.identity);
                    activetrailscript_2 = activetrail_2.GetComponent<TrailScript>();
                    //activetrailscript.InitTouch(touch);
                
                }
                if (hit_2 != null && hit_2.collider != null && touch.phase != TouchPhase.Ended)
                    {
                        if (!completedlvl) HandlePatternInteraction(hit_2);

                    }
                else
                    {   
                        if (!completedlvl) ResetOnTouchRelease();
                    }
            }
        }            
            


            

    }

    void HandlePatternInteraction(RaycastHit2D hit)
    {
        string name = hit.collider.name;

        // ------------------------------
        // PATTERN SET 1  ( _1 )
        // ------------------------------
        if (name == "fdcc_1")
        {
            hitted_1 = true;
            startTimer = true;
            chosenpat.transform.Find("fdcc_1/start").gameObject.SetActive(false);
        }

        if (name == "2_1" && hitted_1)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.Find("2_1").gameObject.SetActive(false);
            if (chosenfoodscript.currentStage < 2) chosenfoodscript.OnStage2();
            trg1_1 = true;
        }

        if (name == "3_1" && hitted_1 && trg1_1)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.Find("3_1").gameObject.SetActive(false);
            if (chosenfoodscript.currentStage < 3) chosenfoodscript.OnStage3();
            trg2_1 = true;
        }

        if (name == "finish_1" && hitted_1 && trg1_1 && trg2_1 && hitted_2 && trg1_2 && trg2_2)
        {
            CompletePatternSet();
        }

        // ------------------------------
        // PATTERN SET 2  ( _2 )
        // ------------------------------
        if (name == "fdcc_2")
        {
            hitted_2 = true;
            startTimer = true;
            chosenpat.transform.Find("fdcc_2/start").gameObject.SetActive(false);
        }

        if (name == "2_2" && hitted_2)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.Find("2_2").gameObject.SetActive(false);
            if (chosenfoodscript.currentStage < 2) chosenfoodscript.OnStage2();
            trg1_2 = true;
        }

        if (name == "3_2" && hitted_2 && trg1_2)
        {
            SoundManagerScript.PlaySound("eating1");
            chosenpat.transform.Find("3_2").gameObject.SetActive(false);
            if (chosenfoodscript.currentStage < 3) chosenfoodscript.OnStage3();
            trg2_2 = true;
        }

        if (name == "finish_2" && hitted_2 && trg1_2 && trg2_2 && hitted_1 && trg1_1 && trg2_1)
        {
            CompletePatternSet();
        }
    }
    

    void CompletePatternSet()
{
    // Play success
    SoundManagerScript.PlaySound("okpattern");

    // Reset visuals for all pattern sets
    chosenpat.transform.Find("fdcc_1/start").gameObject.SetActive(true);
    chosenpat.transform.Find("2_1").gameObject.SetActive(true);
    chosenpat.transform.Find("3_1").gameObject.SetActive(true);

    chosenpat.transform.Find("fdcc_2/start").gameObject.SetActive(true);
    chosenpat.transform.Find("2_2").gameObject.SetActive(true);
    chosenpat.transform.Find("3_2").gameObject.SetActive(true);

    // Reset food
    chosenfoodscript.OnStage1();
    chosenfood.transform.position = new Vector3(4f, 2.5f, 0f);

    // Score
    ScoreScript.scoreValue += 10;

    // Disable previous pattern & food
    chosenpat.SetActive(false);
    chosenfood.SetActive(false);

    // Spawn next pattern
    int next = GetNextPattern();
    chosenpat = tubes[next];
    chosenpat.SetActive(true);

    // Spawn next food
    if (chosenfood != null) Destroy(chosenfood);
    chosenfood = spawner.SpawnNextFood();
    chosenfoodscript = chosenfood.GetComponent<FoodScript>();
    chosenfood.transform.localScale = Vector3.one * 3.5f;


    hitted_1 = trg1_1 = trg2_1 = false;
    hitted_2 = trg1_2 = trg2_2 = false;


    // Flash cooldown to avoid immediate fail flash
    StartCoroutine(StartRedFlashCooldown());
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
    

    void ResetOnTouchRelease()
    {
        if (hitted_1 == true || hitted_2 == true)
        {
            Debug.Log("Lost target");
            if (!redFlashIsOnCooldown)
            {
                SoundManagerScript.PlaySound("fail");
                StartCoroutine(PlayRedFlash());
                StartCoroutine(StartRedFlashCooldown());
            }
            hitted_1 = false;
            hitted_2 = false;
            trg1_1 = false;
            trg2_1 = false;
            trg1_2 = false;
            trg2_2 = false;

            //reset stage triggers and green circle
            chosenpat.transform.Find("fdcc_1/start").gameObject.SetActive(true);
            chosenpat.transform.Find("2_1").gameObject.SetActive(true);
            chosenpat.transform.Find("3_1").gameObject.SetActive(true);

            chosenpat.transform.Find("fdcc_2/start").gameObject.SetActive(true);
            chosenpat.transform.Find("2_2").gameObject.SetActive(true);
            chosenpat.transform.Find("3_2").gameObject.SetActive(true);



            //when leaving tube to reset first burger
            chosenfoodscript.OnStage1();

        }
        // Reset logic when touch is released
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
                chosenfood.SetActive(false);
            }
        }
    }

    void GetEndCanvasChildren()
    {
        string[] winlist = { "burgerdance", "YOU WIN!", "nextStage"};
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
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + " Completed", 1);
            
            if (!string.IsNullOrWhiteSpace(unlockWorld))
            {
                PopupScript.ShowUnlockMessage("YOU HAVE UNLOCKED " + unlockWorld + "!");
                PlayerPrefs.SetInt(unlockWorld + " Unlocked", 1);
                
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

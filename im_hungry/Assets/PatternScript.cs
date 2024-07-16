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
    private GameObject activetrail;
    private TrailScript activetrailscript;

    float currentTime = 0f;
    float startingTime = 30f;
    bool startTimer = false;
    bool isPaused = false;

    [SerializeField] GameObject Trail;
    [SerializeField] int scoreGoal;
    [SerializeField] string finishedWorld = null;
    [SerializeField] Text countdownText;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject endCanvasUI;

    List<GameObject> winComponents = new List<GameObject>();
    List<GameObject> loseComponents = new List<GameObject>();

    [SerializeField] private GameObject[] food;
    [SerializeField] private GameObject[] tubes;

    public bool hitted = false;
    bool trg1 = false;
    bool trg2 = false;
    int num;
    int num2;
    int lastnum;
    int lastnum2;
    private Queue<int> patternQueue;
    private Queue<int> foodQueue;
    private int lastPattern;
    private int lastFood;
    bool completedlvl = false;

    private bool[] countdownSoundPlayed = new bool[3]; // To track sounds for 3, 2, and 1 seconds
    bool endhasPlayed = false;

    [SerializeField] private Image redFlashImage; // Assign the UI Image in the Inspector
    public float redFlashDuration = 0.45f; // Duration of the red flash effect
    private Color originalColor;
    private bool redFlashIsOnCooldown = false; // Red Flash Cooldown flag
    [SerializeField] private float redFlashCooldown = 0.5f; // Cooldown duration in seconds

    // Start is called before the first frame update
    void Start()
    {
        originalColor = redFlashImage.color;
        patternQueue = InitializeQueue(tubes.Length);
        foodQueue = InitializeQueue(food.Length);
        InitializePatternsAndFood();
        GetEndCanvasChildren();
        currentTime = startingTime;
        pauseMenuUI.SetActive(false);
        endCanvasUI.SetActive(false);
        ScoreScript.scoreGoal = scoreGoal;
    }

    void Update()
    {
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

        chosenfood = food[GetNextFood()];
        chosenfoodscript = chosenfood.GetComponent<FoodScript>();

        foreach (GameObject foodObj in food)
        {
            foodObj.SetActive(foodObj == chosenfood);
        }
    }

    void HandleInput()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId != 0)
            {
                continue; // Ignore touches with different IDs
            }

            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.zero);
            if (touch.phase == TouchPhase.Began)
            {
                
                
                activetrail = Instantiate(Trail, pos, Quaternion.identity);
                activetrailscript = activetrail.GetComponent<TrailScript>();
                //activetrailscript.InitTouch(touch);
                
            }
            
            

        if (hit != null && hit.collider != null && touch.phase != TouchPhase.Ended)
            {
                if (!completedlvl) HandlePatternInteraction(hit);

            }
        else
            {   
                if (!completedlvl) ResetOnTouchRelease();
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
            //prevent replay of sound
            chosenpat.transform.GetChild(1).gameObject.SetActive(false);
            chosenfoodscript.OnStage2();
            trg1 = true;
        }

        if (hit.collider.name == chosenpat.transform.GetChild(2).name && hitted && trg1)
        {
            SoundManagerScript.PlaySound("eating1");
            //prevent replay of sound
            chosenpat.transform.GetChild(2).gameObject.SetActive(false);
            chosenfoodscript.OnStage3();
            trg2 = true;
        }

        if (hit.collider.name == chosenpat.transform.GetChild(3).name && hitted && trg1 & trg2)
        {
            // Handle pattern interaction logic
            {
                //play sound
                SoundManagerScript.PlaySound("okpattern");

                //reset stage triggers
                chosenpat.transform.GetChild(1).gameObject.SetActive(true);
                chosenpat.transform.GetChild(2).gameObject.SetActive(true);
                //reset start circle
                chosenpat.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

                //reset food sprite
                chosenfoodscript.OnStage1();

                //reset position of food
                chosenfood.transform.position = new Vector3(4f, 2.5f, 0f);

                //update score
                ScoreScript.scoreValue += 10;

                //disable previous food/pattern

                chosenpat.SetActive(false);
                chosenfood.SetActive(false);

                //choose next food/pattern

                num = GetNextPattern();
                num2 = GetNextFood();

                chosenpat = tubes[num];
                chosenfood = food[num2];
                chosenfoodscript = chosenfood.GetComponent<FoodScript>();

                //set next chosen food/pattern
                chosenpat.SetActive(true);
                chosenfood.SetActive(true);

                //reset triggers
                trg1 = false;
                trg2 = false;
                hitted = false;
                //activeTouchId = -1;

                //cooldown on red flash to avoid flashing on start of next pattern
                StartCoroutine(StartRedFlashCooldown());
            }
        }
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

    public int GetNextFood()
    {
        return GetNextItem(foodQueue, ref lastFood);
    }

    void ResetOnTouchRelease()
    {
        if (hitted == true)
        {
            Debug.Log("Lost target");
            if (!redFlashIsOnCooldown)
            {
                SoundManagerScript.PlaySound("fail");
                StartCoroutine(PlayRedFlash());
                StartCoroutine(StartRedFlashCooldown());
            }
            hitted = false;
            trg1 = false;
            trg2 = false;

            //reset stage triggers and green circle
            chosenpat.transform.GetChild(1).gameObject.SetActive(true);
            chosenpat.transform.GetChild(2).gameObject.SetActive(true);
            chosenpat.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);



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
                
                if (!endhasPlayed) { checkIfWin(); }
                endhasPlayed = true;

                completedlvl = true;
                endCanvasUI.SetActive(true);
                
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
            
            if (finishedWorld != null)
            {
                PlayerPrefs.SetInt(finishedWorld + " Completed", 1);
                
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

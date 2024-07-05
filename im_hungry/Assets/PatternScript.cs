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
    [SerializeField] Text countdownText;
    [SerializeField] GameObject pauseMenuUI;

    [SerializeField] private GameObject resetButton;
    [SerializeField] private GameObject backButton;

    [SerializeField] private GameObject[] food;
    [SerializeField] private GameObject[] tubes;

    public bool hitted = false;
    bool trg1 = false;
    bool trg2 = false;
    int num;
    int num2;
    int lastnum;
    int lastnum2;
    bool completedlvl = false;

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
        InitializePatternsAndFood();
        currentTime = startingTime;
        resetButton.SetActive(false);
        backButton.SetActive(false);
        pauseMenuUI.SetActive(false);
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
        int patternIndex = UnityEngine.Random.Range(0, tubes.Length);
        chosenpat = tubes[patternIndex];

        foreach (GameObject tube in tubes)
        {
            tube.SetActive(tube == chosenpat);
        }

        int foodIndex = UnityEngine.Random.Range(0, food.Length);
        chosenfood = food[foodIndex];
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


                lastnum = num;
                lastnum2 = num2;

                num = Random.Range(0, tubes.Length);
                num2 = Random.Range(0, food.Length);

                //dont choose last pattern

                if (num == lastnum)
                {
                    num = (num + 2) % tubes.Length;
                }

                if (num2 == lastnum2)
                {
                    num2 = (num2 + 2) % food.Length;
                }

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
            }
        }
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

            //reset stage triggers
            chosenpat.transform.GetChild(1).gameObject.SetActive(true);
            chosenpat.transform.GetChild(2).gameObject.SetActive(true);



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
                chosenpat.SetActive(false);
                resetButton.SetActive(true);
                backButton.SetActive(true);
            }
        }
    }
    void checkIfWin()
    {
        if (ScoreScript.scoreValue >= scoreGoal)
        {
            SoundManagerScript.PlaySound("win");
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + " Completed", 1);
        }
        else
        {
            SoundManagerScript.PlaySound("end");
        }
    }
}

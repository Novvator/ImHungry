using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Patter : MonoBehaviour
{
    private GameObject chosenPattern;
    private GameObject chosenFood;

    float currentTime = 0f;
    float startingTime = 30f;
    bool startTimer = false;
    [SerializeField] Text countdownText;

    private bool hitted;
    private GameObject resetButton;

    private GameObject[] food;
    private GameObject[] tubes;
    private GameObject[] stage;

    bool completedLevel = false;
    bool endSoundPlayed = false;

    void Start()
    {
        currentTime = startingTime;
        resetButton.SetActive(false);

        InitializePatternsAndFood();
    }

    void Update()
    {
        HandleInput();
        UpdateTimer();
    }

    void InitializePatternsAndFood()
    {
        int patternIndex = UnityEngine.Random.Range(0, tubes.Length);
        chosenPattern = tubes[patternIndex];

        foreach (GameObject tube in tubes)
        {
            tube.SetActive(tube == chosenPattern);
        }

        int foodIndex = UnityEngine.Random.Range(0, food.Length);
        chosenFood = food[foodIndex];

        foreach (GameObject foodObj in food)
        {
            foodObj.SetActive(foodObj == chosenFood);
        }
    }

    void HandleInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.zero);

            if (hit != null && hit.collider != null && touch.phase != TouchPhase.Ended)
            {
                HandleResetButton(hit);
                HandlePatternInteraction(hit);
            }
            else
            {
                ResetOnTouchRelease();
            }
        }
    }

    void HandleResetButton(RaycastHit2D hit)
    {
        if (hit.collider.name == "resetButton")
        {
            completedLevel = false;
            resetButton.SetActive(false);
            currentTime = startingTime;
            ScoreScript.scoreValue = 0;
            endSoundPlayed = false;
        }
    }

    void HandlePatternInteraction(RaycastHit2D hit)
    {
        if (completedLevel || chosenPattern == null || chosenFood == null)
            return;

        if (hit.collider.name == "fdcc")
        {
            // Handle stage completion logic
        }

        if (hit.collider.name == chosenPattern.transform.GetChild(0).name)
        {
            // Handle pattern interaction logic
        }

        if (hit.collider.name == chosenPattern.transform.GetChild(1).name)
        {
            // Handle pattern interaction logic
        }

        if (hit.collider.name == chosenPattern.transform.GetChild(2).name)
        {
            // Handle pattern interaction logic
        }
    }

    void ResetOnTouchRelease()
    {
        if (completedLevel || !hitted)
            return;

        // Reset logic when touch is released
    }

    void UpdateTimer()
    {
        if (hitted)
        {
            // Update timer and countdown text
        }
    }
}

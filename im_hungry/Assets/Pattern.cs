//using System;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class Patter : MonoBehaviour
//{
//    private GameObject chosenPattern;
//    private GameObject chosenFood;

//    float currentTime = 0f;
//    float startingTime = 30f;
//    bool startTimer = false;
//    [SerializeField] Text countdownText;

//    private bool hitted;
//    private GameObject resetButton;

//    private GameObject[] food;
//    private GameObject[] tubes;
//    private GameObject[] stage;

//    bool completedLevel = false;
//    bool endSoundPlayed = false;

//    void Start()
//    {
//        currentTime = startingTime;
//        resetButton.SetActive(false);

//        InitializePatternsAndFood();
//    }

//    void Update()
//    {
//        HandleInput();
//        UpdateTimer();
//    }

//    void InitializePatternsAndFood()
//    {
//        int patternIndex = UnityEngine.Random.Range(0, tubes.Length);
//        chosenPattern = tubes[patternIndex];

//        foreach (GameObject tube in tubes)
//        {
//            tube.SetActive(tube == chosenPattern);
//        }

//        int foodIndex = UnityEngine.Random.Range(0, food.Length);
//        chosenFood = food[foodIndex];

//        foreach (GameObject foodObj in food)
//        {
//            foodObj.SetActive(foodObj == chosenFood);
//        }
//    }

//    void HandleInput()
//    {
//        if (Input.touchCount == 1)
//        {
//            Touch touch = Input.GetTouch(0);
//            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
//            RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.zero);

//            if (hit != null && hit.collider != null && touch.phase != TouchPhase.Ended)
//            {
//                HandleResetButton(hit);
//                HandlePatternInteraction(hit);
//            }
//            else
//            {
//                ResetOnTouchRelease();
//            }
//        }
//    }

//    void HandleResetButton(RaycastHit2D hit)
//    {
//        if (hit.collider.name == "resetButton")
//        {
//            completedLevel = false;
//            resetButton.SetActive(false);
//            currentTime = startingTime;
//            ScoreScript.scoreValue = 0;
//            endSoundPlayed = false;
//        }
//    }

//    void HandlePatternInteraction(RaycastHit2D hit)
//    {
//        if (completedLevel || chosenPattern == null || chosenFood == null)
//            return;

//        if (hit.collider.name == "fdcc")
//        {
//            hitted = true;
//            startTimer = true;
//        }

//        if (hit.collider.name == chosenPattern.transform.GetChild(0).name)
//        {
//            SoundManagerScript.PlaySound("eating1");
//            //prevent replay of sound
//            chosenpat.transform.GetChild(0).gameObject.SetActive(false);
//            chosenfood.GetComponent<Animator>().SetInteger("eat", 2);
//            trg1 = true;
//        }

//        if (hit.collider.name == chosenPattern.transform.GetChild(1).name && trg1)
//        {
//            SoundManagerScript.PlaySound("eating1");
//            //prevent replay of sound
//            chosenpat.transform.GetChild(1).gameObject.SetActive(false);
//            chosenfood.GetComponent<Animator>().SetInteger("eat", 3);
//            trg2 = true;
//        }

//        if (hit.collider.name == chosenPattern.transform.GetChild(2).name && trg1 && trg2)
//        {
//            // Handle pattern interaction logic
//            {
//                //play sound
//                SoundManagerScript.PlaySound("okpattern");

//                //reset stage triggers
//                chosenpat.transform.GetChild(0).gameObject.SetActive(true);
//                chosenpat.transform.GetChild(1).gameObject.SetActive(true);

//                //reset food sprite
//                chosenfood.GetComponent<Animator>().SetInteger("eat", 1);

//                //reset position of food
//                chosenfood.transform.position = new Vector3(4f, 2.5f, 0f);

//                //update score
//                ScoreScript.scoreValue += 10;

//                //disable previous food/pattern

//                chosenpat.SetActive(false);
//                chosenfood.SetActive(false);

//                //choose next food/pattern


//                lastnum = num;
//                lastnum2 = num2;

//                num = Random.Range(0, tubes.Length);
//                num2 = Random.Range(0, food.Length);

//                //dont choose last pattern

//                if (num == lastnum)
//                {
//                    num = (num + 1) % tubes.Length;
//                }

//                if (num2 == lastnum2)
//                {
//                    num2 = (num2 + 1) % food.Length;
//                }

//                chosenpat = tubes[num];
//                chosenfood = food[num2];

//                //set next chosen food/pattern
//                chosenpat.SetActive(true);
//                chosenfood.SetActive(true);

//                //reset triggers
//                trg1 = false;
//                trg2 = false;
//                hitted = false;
//            }
//        }
//    }

//    void ResetOnTouchRelease()
//    {
//        if (hitted == true)
//        {
//            Debug.Log("Lost target");
//            hitted = false;
//            trg1 = false;
//            trg2 = false;

//            //reset stage triggers
//            chosenpat.transform.GetChild(0).gameObject.SetActive(true);
//            chosenpat.transform.GetChild(1).gameObject.SetActive(true);



//            //when leaving tube to reset first burger
//            chosenfood.GetComponent<Animator>().SetInteger("eat", 1);
//        }
//        // Reset logic when touch is released
//    }

//    void UpdateTimer()
//    {
//        if (hitted)
//        {
//            if(startTimer == true)
//            {
//                currentTime -= 1 * Time.deltaTime;
//                countdownText.text = currentTime.ToString("N1", CultureInfo.InvariantCulture);
        
//                if(currentTime <= 0)
//                {
//                    currentTime = 0;
//                }
        
//                if (currentTime == 0)
//                {
//                    //play end sound
//                    if(!endhasPlayed) { SoundManagerScript.PlaySound("end"); }
//                    endhasPlayed = true;
        
//                    completedlvl = true;
//                    resetButton.SetActive(true);
//                }
//            }
//        }
//    }
//}

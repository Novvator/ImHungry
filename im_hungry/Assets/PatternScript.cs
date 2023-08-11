//using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
//using System.Diagnostics;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PatternScript : MonoBehaviour
{
    private GameObject chosenpat;
    private GameObject chosenfood;

    float currentTime = 0f;
    float startingTime = 30f;
    bool startTimer = false;
    [SerializeField] Text countdownText;

    public GameObject resetButton;

    public GameObject[] food;
    public GameObject[] tubes;
    public GameObject[] stage;

    public bool hitted = false;
    bool trg1 = false;
    bool trg2 = false;
    int num;
    int num2;
    int lastnum;
    int lastnum2;
    bool completedlvl = false;

    bool endhasPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startingTime;
        resetButton.SetActive(false);

        //choose first pattern/food
        num = Random.Range(0, tubes.Length);
        chosenpat = tubes[num];
        foreach (GameObject go in tubes)
        {
            if (go != chosenpat)
            {
                go.SetActive(false);
            }
        }

        num2 = Random.Range(0, food.Length);
        chosenfood = food[num2];
        foreach (GameObject gb in food)
        {
            if (gb != chosenfood)
            {
                gb.SetActive(false);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {

        
        if (Input.touchCount == 1)
        {

            Touch touch = Input.GetTouch(0);
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.zero);

            //pressing reset button
            if (hit !=  null && hit.collider != null && touch.phase != TouchPhase.Ended)
            {
                if (hit.collider.name == "resetButton")
                {
                    completedlvl = false;
                    resetButton.SetActive(false);
                    currentTime = 30f;
                    ScoreScript.scoreValue = 0;
                    endhasPlayed = false;

                }
            }

            if (hit != null && hit.collider != null && touch.phase != TouchPhase.Ended && completedlvl == false)
            {

                //Debug.Log("I'm hitting " + hit.collider.name
                
                //stages to complete pattern
                if (hit.collider.name == "fdcc")
                {
                    hitted = true;
                    startTimer = true;

                }

                if(hit.collider.name == chosenpat.transform.GetChild(0).name && hitted)
                {
                    SoundManagerScript.PlaySound("eating1");
                    //prevent replay of sound
                    chosenpat.transform.GetChild(0).gameObject.SetActive(false);
                    chosenfood.GetComponent<Animator>().SetInteger("eat", 2);
                    trg1 = true;
                }
                
                if(hit.collider.name == chosenpat.transform.GetChild(1).name && hitted && trg1)
                {
                    SoundManagerScript.PlaySound("eating1");
                    //prevent replay of sound
                    chosenpat.transform.GetChild(1).gameObject.SetActive(false);
                    chosenfood.GetComponent<Animator>().SetInteger("eat", 3);
                    trg2 = true;

                }
                if (hit.collider.name == chosenpat.transform.GetChild(2).name && hitted && trg1 & trg2)
                {
                    //play sound
                    SoundManagerScript.PlaySound("okpattern");

                    //reset stage triggers
                    chosenpat.transform.GetChild(0).gameObject.SetActive(true);
                    chosenpat.transform.GetChild(1).gameObject.SetActive(true);

                    //reset food sprite
                    chosenfood.GetComponent<Animator>().SetInteger("eat", 1);

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
                        num = (num + 1) % tubes.Length;
                    }

                    if (num2 == lastnum2)
                    {
                        num2 = (num2 + 1) % food.Length;
                    }

                    chosenpat = tubes[num];
                    chosenfood = food[num2];

                    //set next chosen food/pattern
                    chosenpat.SetActive(true);
                    chosenfood.SetActive(true);

                    //move chosen food to position


                    // chosenpat.SetActive(true);
                    // chosenfood.SetActive(true);

                    //reset triggers
                    trg1 = false;
                    trg2 = false;
                    hitted = false;
                }

            }
            else
            {
                //reset when removing touch

                if (hitted == true)
                {

                    Debug.Log("Lost target");
                    hitted = false;
                    trg1 = false;
                    trg2 = false;

                    //reset stage triggers
                    chosenpat.transform.GetChild(0).gameObject.SetActive(true);
                    chosenpat.transform.GetChild(1).gameObject.SetActive(true);

                    //when leaving tube to reset first burger
                    chosenfood.GetComponent<Animator>().SetInteger("eat", 1);

                }
            }



        }

        if(startTimer == true)
        {
            currentTime -= 1 * Time.deltaTime;
            countdownText.text = currentTime.ToString("N1", CultureInfo.InvariantCulture);

            if(currentTime <= 0)
            {
                currentTime = 0;
            }

            if (currentTime == 0)
            {
                //play end sound
                if(!endhasPlayed) { SoundManagerScript.PlaySound("end"); }
                endhasPlayed = true;

                completedlvl = true;
                resetButton.SetActive(true);
            }
        }
    }
}

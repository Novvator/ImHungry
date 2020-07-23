//using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
//using System.Diagnostics;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PatternScript : MonoBehaviour
{
    private GameObject chosenpat;
    private GameObject chosenfood;

    public GameObject[] food;
    public GameObject[] tubes;
    public GameObject[] stage;
    public GameObject target;

    public bool hitted = false;
    bool trg1 = false;
    bool trg2 = false;
    int num;
    int num2;

    // Start is called before the first frame update
    void Start()
    {
        num = Random.Range(0, 2);
        chosenpat = tubes[num];
        foreach (GameObject go in tubes)
        {
            if (go != chosenpat)
            {
                go.SetActive(false);
            }
        }

        num2 = Random.Range(0, 2);
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
            if (hit != null && hit.collider != null && touch.phase != TouchPhase.Ended)
            {

                //Debug.Log("I'm hitting " + hit.collider.name
                
                //stages to complete pattern
                if (hit.collider.name == "fdcc")
                {
                    hitted = true;

                }

                if(hit.collider.name == chosenpat.transform.GetChild(0).name && hitted)
                {
                    chosenfood.GetComponent<Animator>().SetInteger("eat", 2);
                    trg1 = true;
                }
                
                if(hit.collider.name == chosenpat.transform.GetChild(1).name && hitted && trg1)
                {

                    chosenfood.GetComponent<Animator>().SetInteger("eat", 3);
                    trg2 = true;

                }
                if (hit.collider.name == chosenpat.transform.GetChild(2).name && hitted && trg1 & trg2)
                {


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
                    num = Random.Range(0, 2);
                    num2 = Random.Range(0, 2);

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



                    //when leaving tube to reset first burger
                    chosenfood.GetComponent<Animator>().SetInteger("eat", 1);

                }
            }



        }

        if( hitted == true)
        {
            chosenpat.transform.GetChild(3).transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            chosenpat.transform.GetChild(3).transform.GetChild(0).GetComponent<Animator>().enabled = false;
        }
        else
        {
            chosenpat.transform.GetChild(3).transform.GetChild(0).GetComponent<Renderer>().enabled = true;
            chosenpat.transform.GetChild(3).transform.GetChild(0).GetComponent<Animator>().enabled = true;
        }
    }
}

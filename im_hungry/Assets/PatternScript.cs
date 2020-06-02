//using System;
using System.Collections;
using System.Collections.Generic;
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
    public GameObject burger;
    public GameObject[] stage;
    bool hitted = false;
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
            Vector2 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit != null && hit.collider != null && touch.phase != TouchPhase.Ended)
            {
                
                //Debug.Log("I'm hitting " + hit.collider.name);

                

                if (hit.collider.name == chosenpat.name)
                {
                    hitted = true;
                }

                if(hit.collider.name == "2"/*chosenpat.transform.GetChild(0).name*/ && hitted)
                {
                    chosenfood.GetComponent<Animator>().SetInteger("eat", 2);
                    //burger.gameObject.GetComponent<SpriteRenderer>().sprite = bsprite[1];
                    trg1 = true;
                }
                
                if(hit.collider.name == chosenpat.transform.GetChild(1).name && hitted && trg1)
                {

                    chosenfood.GetComponent<Animator>().SetInteger("eat", 3);
                    trg2 = true;

                }
                if (hit.collider.name == chosenpat.transform.GetChild(2).name && hitted && trg1 & trg2)
                {

                    //burger.SetActive(false);

                    //go to next pattern/food
                    chosenfood.GetComponent<Animator>().SetInteger("eat", 1);

                    num = Random.Range(0, 2);
                    num2 = Random.Range(0, 2);

                    chosenpat = tubes[num];
                    chosenfood = food[num2];

                    chosenpat.SetActive(true);
                    foreach (GameObject go in tubes)
                    {
                        if (go != chosenpat)
                        {
                            go.SetActive(false);
                        }
                    }

                    chosenfood.SetActive(true);
                    foreach (GameObject gb in food)
                    {
                        if (gb != chosenfood)
                        {
                            gb.SetActive(false);
                        }
                    }

                    //reset triggers
                    trg1 = false;
                    trg2 = false;
                    hitted = false;
                }

            }
            else
            {
                if (hitted == true)
                {
                    Debug.Log("Lost target");
                    hitted = false;

                    //when leaving tube to reset first burger
                    chosenfood.GetComponent<Animator>().SetInteger("eat", 1);

                }
            }

            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PatternScript : MonoBehaviour
{
    public GameObject[] burgers;
    public GameObject[] stage;
    bool hitted = false;
    bool trg1 = false;
    bool trg2 = false;

    // Start is called before the first frame update
    void Start()
    {
        burgers[1].SetActive(false);
        burgers[2].SetActive(false);
        
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
                
                Debug.Log("I'm hitting " + hit.collider.name);

                

                if (hit.collider.name == "tubee")
                {
                    hitted = true;
                }

                if(hit.collider.name == "2" && hitted)
                {
                    burgers[0].SetActive(false);
                    burgers[1].SetActive(true);
                    burgers[2].SetActive(false);
                    trg1 = true;
                }
                
                if(hit.collider.name == "3" && hitted && trg1)
                {
                    burgers[0].SetActive(false);
                    burgers[1].SetActive(false);
                    burgers[2].SetActive(true);
                    trg2 = true;

                }
                if (hit.collider.name == "finish" && hitted && trg1 & trg2)
                {
                    burgers[0].SetActive(false);
                    burgers[1].SetActive(false);
                    burgers[2].SetActive(false);
                    SceneManager.LoadScene("scene2");
                }

            }
            else
            {
                if (hitted == true)
                {
                    Debug.Log("Lost target");
                    hitted = false;

                    //when leaving tube to reset first burger
                    if(burgers[0].activeSelf == false)
                    {
                        burgers[0].SetActive(true);
                    }

                    if (burgers[1].activeSelf == true)
                    {
                        burgers[1].SetActive(false);
                    }

                    if (burgers[2].activeSelf == true)
                    {
                        burgers[2].SetActive(false);
                    }
                    
                  
                }
            }

            
        }
    }
}

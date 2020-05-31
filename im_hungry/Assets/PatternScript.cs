using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PatternScript : MonoBehaviour
{

    public Sprite[] bsprite;
    public GameObject burger;
    public GameObject[] stage;
    bool hitted = false;
    bool trg1 = false;
    bool trg2 = false;

    // Start is called before the first frame update
    void Start()
    {


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

                    burger.gameObject.GetComponent<SpriteRenderer>().sprite = bsprite[1];
                    trg1 = true;
                }
                
                if(hit.collider.name == "3" && hitted && trg1)
                {

                    burger.gameObject.GetComponent<SpriteRenderer>().sprite = bsprite[2];
                    trg2 = true;

                }
                if (hit.collider.name == "finish" && hitted && trg1 & trg2)
                {

                    burger.SetActive(false);
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
                    burger.gameObject.GetComponent<SpriteRenderer>().sprite = bsprite[0];

                }
            }

            
        }
    }
}

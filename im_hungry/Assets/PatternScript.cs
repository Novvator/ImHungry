using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PatternScript : MonoBehaviour
{
    public GameObject[] burgers;
    public GameObject[] stage;
    int hitted = 0;

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
            if (hit != null && hit.collider != null)
            {
                
                Debug.Log("I'm hitting " + hit.collider.name);

                if (hit.collider.name == "tubee")
                {
                    hitted = 1;
                }

                if(hit.collider.name == "2" && hitted == 1)
                {
                    burgers[0].SetActive(false);
                    burgers[1].SetActive(true);

                }
                
                if(hit.collider.name == "3" && hitted == 1)
                {
                    burgers[1].SetActive(false);
                    burgers[2].SetActive(true);

                }
                if (hit.collider.name == "finish" && hitted == 1)
                {
                    burgers[2].SetActive(false);
                }

            }
            else
            {
                if (hitted == 1)
                {
                    Debug.Log("Lost target");
                    hitted = 0;

                    //when leaving tube to reset first burger
                    //burgers[0].SetActive(true);
                    //burgers[1].SetActive(false);
                    //burgers[2].SetActive(false);
                }
            }

            
        }
    }
}

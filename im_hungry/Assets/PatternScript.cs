using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternScript : MonoBehaviour
{
    public GameObject[] poz;
    int endd = 0;
    // Start is called before the first frame update
    void Start()
    {
        poz[1].SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit != null && hit.collider != null)
            {
                Debug.Log("I'm hitting " + hit.collider.name);
                
                    while (touch.phase == TouchPhase.Moved)
                    {
                        while (hit.collider.name == "tubee")
                        {
                            if (hit.collider.name == "finish")
                            {
                                endd = 1;
                            }
                            if (endd == 1) { break; }

                        }
                        if (endd == 1) { break; }
                    }
                    if (endd == 1) { poz[1].SetActive(true); }
               


            }
        }
    }
}

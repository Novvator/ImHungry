using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        while (Input.touchCount > i)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
               // Debug.Log("touch began");
                //Destroy(gameObject);
                
            }
            ++i;

        }
    }
 }

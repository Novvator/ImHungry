using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetector : MonoBehaviour
{
    public GameObject[] objects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit != null && hit.collider != null)
            {
                Debug.Log("I'm hitting " + hit.collider.name);
                if(hit.collider.name == "reload")
                {
                    objects[0].SetActive(true);
                    objects[1].SetActive(true);
                    objects[2].SetActive(true);
                }

                else
                {

                    hit.collider.gameObject.SetActive(false);

                }
                
            
            }
        }
    }
}

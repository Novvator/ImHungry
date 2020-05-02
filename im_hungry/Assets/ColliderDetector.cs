using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetector : MonoBehaviour
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
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit != null && hit.collider != null)
            {
                Debug.Log("I'm hitting " + hit.collider.name);
                Destroy(hit.collider.gameObject);
            
            }

            ++i;
        }
    }
}

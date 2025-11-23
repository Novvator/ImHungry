using System.Collections;
using UnityEngine;

public class TrailCreator : MonoBehaviour
{
    [SerializeField] private GameObject Trail; // Assign your Trail prefab in the Inspector

    private GameObject activetrail;
    private TrailScript activetrailscript;

    void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        foreach (Touch touch in Input.touches)
        {
            // Only track the first finger
            if (touch.fingerId != 0) continue;

            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
            pos.z = 0f; // Ensure z=0 for 2D

            if (touch.phase == TouchPhase.Began)
            {
                // Instantiate the trail at touch position
                activetrail = Instantiate(Trail, pos, Quaternion.identity);
                activetrailscript = activetrail.GetComponent<TrailScript>();
                
                // Optionally initialize the TrailScript if needed
                // activetrailscript.InitTouch(touch);
            }

            // You can also update trail position while moving
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (activetrail != null)
                {
                    activetrail.transform.position = pos;
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                activetrail = null; // Stop tracking trail
            }
        }
    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField] GameObject trailPrefab;
    [SerializeField] float trailLifetime = 2.0f;

    private GameObject currentTrail;
    private TrailRenderer currentTrailRenderer;

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                CreateTrail(touchPosition);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                UpdateTrailPosition(touchPosition);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                DestroyTrail();
            }
        }
    }

    private void CreateTrail(Vector3 initialPosition)
    {
        currentTrail = Instantiate(trailPrefab, initialPosition, Quaternion.identity);
        currentTrailRenderer = currentTrail.GetComponent<TrailRenderer>();
        Destroy(currentTrail, trailLifetime);
    }

    private void UpdateTrailPosition(Vector3 newPosition)
    {
        if (currentTrailRenderer != null)
        {
            currentTrail.transform.position = newPosition;
        }
    }

    private void DestroyTrail()
    {
        Destroy(currentTrail);
        currentTrailRenderer = null;
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailScript : MonoBehaviour
{
    [SerializeField] GameObject trailPrefab;
    private TrailRenderer trailRenderer;
    private SpriteRenderer spriteTrailRenderer;
    private int activeTouchId = -1;
    private GameObject currenttrail;

    private void Start()
    {

        trailRenderer = GetComponent<TrailRenderer>();
        spriteTrailRenderer = GetComponent<SpriteRenderer>();
        spriteTrailRenderer.enabled = false;
    }

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                if (activeTouchId == -1)
                {
                    activeTouchId = touch.fingerId;
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    touchPosition.z = -1; // Ensure the trail is at the same depth as your game objects
                    transform.position = touchPosition;
                    EnableTrail(true);
                }
                
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (touch.fingerId == activeTouchId)
                {
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    touchPosition.z = -1; // Ensure the trail is at the same depth as your game objects
                    transform.position = touchPosition;
                }
            }
            
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (touch.fingerId == activeTouchId)
                {
                    // EnableTrail(false);
                    Destroy(gameObject);
                }
                activeTouchId = -1;
            }
        }
    }

    private void EnableTrail(bool state)
    {
        trailRenderer.enabled = state;
        spriteTrailRenderer.enabled = state;
    }

    public void InstantiateTrail(Touch touch)
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        touchPosition.z = -1;
        currenttrail = Instantiate(gameObject, touchPosition, Quaternion.identity);
    }
}


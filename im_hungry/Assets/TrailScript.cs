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
    private GameObject trail;
    private TrailRenderer trailRenderer;
    private SpriteRenderer spriteTrailRenderer;
    private Touch touch;

    private void Start()
    {

        trailRenderer = this.GetComponent<TrailRenderer>();
        spriteTrailRenderer = this.GetComponent<SpriteRenderer>();
        spriteTrailRenderer.enabled = false;
    }

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                touchPosition.z = -1; // Ensure the trail is at the same depth as your game objects
                Instantiate(trailPrefab, touchPosition, Quaternion.identity);
                this.transform.position = touchPosition;
                EnableTrail(true);
                
            }
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                touchPosition.z = -1; // Ensure the trail is at the same depth as your game objects
                this.transform.position = touchPosition;
            }
            
            else if (touch.phase == TouchPhase.Ended)
            {
                // EnableTrail(false);
                Destroy(this.gameObject);
            }
        }
    }

    private void EnableTrail(bool state)
    {
        trailRenderer.enabled = state;
        spriteTrailRenderer.enabled = state;
    }

    public void InitTouch(Touch touch)
    {
        this.touch = touch;
    }
    
    public GameObject InstantiateTrail(Touch touch)
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        touchPosition.z = -1;
        trail = Instantiate(this.gameObject, touchPosition, Quaternion.identity);
        return trail;
    }
}


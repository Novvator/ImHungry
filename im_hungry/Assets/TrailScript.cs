using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailScript : MonoBehaviour
{
    [SerializeField] GameObject trailPrefab;
    private GameObject trail;
    private TrailRenderer trailRenderer;
    private SpriteRenderer spriteTrailRenderer;
    //private Touch touch;

    private void Start()
    {
        trailRenderer = this.GetComponent<TrailRenderer>();
        spriteTrailRenderer = this.GetComponent<SpriteRenderer>();
        spriteTrailRenderer.enabled = false;
    }

    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId != 0)
            {
                continue; // Ignore touches with different IDs
            }
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
                Destroy(this.gameObject);
            }
        }
    }

    private void EnableTrail(bool state)
    {
        trailRenderer.enabled = state;
        spriteTrailRenderer.enabled = state;
    }

    //public void InitTouch(Touch touch)
    //{
     //   this.touch = touch;
    //}
    
    public GameObject InstantiateTrail(Touch touch)
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        touchPosition.z = -1;
        trail = Instantiate(this.gameObject, touchPosition, Quaternion.identity);
        return trail;
    }
}


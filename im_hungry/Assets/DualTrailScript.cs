using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualTrailScript : MonoBehaviour
{
    [SerializeField] private GameObject trailPrefab;

    private int fingerId = -1;      // << NEW
    private TrailRenderer trailRenderer;
    private SpriteRenderer spriteTrailRenderer;

    public void SetFingerId(int id)   // << NEW
    {
        fingerId = id;
    }

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        spriteTrailRenderer = GetComponent<SpriteRenderer>();
        spriteTrailRenderer.enabled = false;

        UpdateTrailMaterial();
    }

    private void Update()
    {
        // If this instance was not assigned a finger, do nothing
        if (fingerId == -1) return;

        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId != fingerId)
                continue; // Only follow OUR finger

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = -1;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    EnableTrail(true);
                    transform.position = touchPosition;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    transform.position = touchPosition;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    Destroy(gameObject); // Destroy THIS trail only
                    break;
            }
        }
    }

    private void EnableTrail(bool state)
    {
        trailRenderer.enabled = state;
        spriteTrailRenderer.enabled = state;
    }

    private void UpdateTrailMaterial()
    {
        string currentTrail = PlayerPrefs.GetString("Current Trail", "Default-Line");
        Material m = Resources.Load<Material>(currentTrail);
        if (m != null)
            trailRenderer.material = m;
    }
}

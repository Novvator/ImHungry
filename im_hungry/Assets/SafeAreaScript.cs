using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform safeAreaTransform;
    Rect lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        safeAreaTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {

    }

    void ApplySafeArea()
    {
        lastSafeArea = Screen.safeArea;

        // Convert safe area rectangle from absolute pixels to normalized anchor coordinates
        Vector2 anchorMin = lastSafeArea.position;
        Vector2 anchorMax = lastSafeArea.position + lastSafeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        safeAreaTransform.anchorMin = anchorMin;
        safeAreaTransform.anchorMax = anchorMax;

        Debug.Log("Safe Area Applied: " + lastSafeArea);
    }
}

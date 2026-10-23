using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessagePopupScript : MonoBehaviour
{
    [SerializeField] GameObject popupPrefab;
    private byte alphaValue = 220;

    // Simple text popup (e.g. "YOU HAVE UNLOCKED World 2!") shown on the end screen
    public void ShowUnlockMessage(string message)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("MessagePopupScript: Popup Prefab is not assigned!");
            return;
        }

        GameObject canvas = GameObject.Find("CanvasEnd");
        if (canvas == null)
        {
            Debug.LogError("MessagePopupScript: CanvasEnd not found in scene.");
            return;
        }

        GameObject popup = Instantiate(popupPrefab);
        popup.transform.SetParent(canvas.transform, false);

        Image image = popup.GetComponent<Image>();
        if (image != null)
        {
            Color currentColor = image.color;
            currentColor.a = alphaValue / 255f;
            image.color = currentColor;
        }

        RectTransform rectTransform = popup.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -450);
        }

        Text text = popup.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = message;
        }
    }

    // Gallery-style popup listing the items unlocked by a level (work in progress, needs UnlockDatabase in the scene)
    public void ShowUnlockPopup(string levelName)
    {
        if (UnlockDatabase.Instance == null)
        {
            Debug.LogWarning("MessagePopupScript: No UnlockDatabase in scene.");
            return;
        }

        List<UnlockItem> unlocks = UnlockDatabase.Instance.GetUnlocksForLevel(levelName);

        if (unlocks == null || unlocks.Count == 0)
        {
            Debug.Log("No unlocks for this level.");
            return;
        }

        GameObject popup = Instantiate(popupPrefab);
        popup.transform.SetParent(GameObject.Find("CanvasEnd").transform, false);

        UnlockPopupController controller = popup.GetComponent<UnlockPopupController>();
        controller.Initialize(unlocks);
    }
}

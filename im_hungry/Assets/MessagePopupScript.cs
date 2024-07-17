using UnityEngine;
using UnityEngine.UI;

public class MessagePopupScript : MonoBehaviour
{
    [SerializeField] GameObject popupPrefab; // Assign this in the inspector
    private byte alphaValue = 220;

    public void ShowUnlockMessage(string message)
    {
        if (popupPrefab != null)
        {
            GameObject popup = Instantiate(popupPrefab);
            popup.transform.SetParent(GameObject.Find("CanvasEnd").transform, false); // Ensure it's part of the UI
            RectTransform rectTransform = popup.GetComponent<RectTransform>();
            Image image = popup.GetComponent<Image>();
            if (image != null)
            {
                Color currentColor = image.color;
                currentColor.a = alphaValue / 255f; // Convert 220 to a value between 0 and 1
                image.color = currentColor;
            }
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.y, -450);
            }
            popup.GetComponentInChildren<UnityEngine.UI.Text>().text = message; // Assuming there's a Text component in children
        }
        else
        {
            Debug.LogError("Popup Prefab is not assigned!");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ChangeTrailManager : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] GameObject popupPanel; // Assign the Panel in the inspector
    [SerializeField] Text messageText; // Assign the text component in the inspector

    private void Start()
    {
        // Find all Button components in the scene
        // buttons = FindObjectsOfType<Button>();

        // Loop through each button and add the listener
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => SetTrail(button.name));
        }
        popupPanel.SetActive(false);
    }

    private void SetTrail(string trailName)
    {
        PlayerPrefs.SetString("Current Trail", trailName);
        PlayerPrefs.Save(); // Ensure the changes are saved
        Debug.Log("Current Trail set to " + trailName);
        ShowPopup(trailName + "\nSELECTED!");
    }

    public void ShowPopup(string message)
    {
        messageText.text = message;
        popupPanel.SetActive(true); // Show the popup
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false); // Hide the popup
    }
}

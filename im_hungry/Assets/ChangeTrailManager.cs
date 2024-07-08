using UnityEngine;
using UnityEngine.UI;

public class ChangeTrailManager : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    private void Start()
    {
        // Find all Button components in the scene
        // buttons = FindObjectsOfType<Button>();

        // Loop through each button and add the listener
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => SetTrail(button.name));
        }
    }

    private void SetTrail(string trailName)
    {
        PlayerPrefs.SetString("Current Trail", trailName);
        PlayerPrefs.Save(); // Ensure the changes are saved
        Debug.Log("Current Trail set to " + trailName);
    }
}

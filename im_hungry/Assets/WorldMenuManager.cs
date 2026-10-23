using UnityEngine;
using UnityEngine.UI;

public class WorldMenuManager : MonoBehaviour
{
    public GameObject[] worldButtons;

    private void Start()
    {

        foreach (GameObject button in worldButtons)
        {
            // Buttons are named after their world number ("2", "3", ...)
            if (!int.TryParse(button.name, out int world))
            {
                Debug.LogWarning("WorldMenuManager: button name is not a world number: " + button.name);
                continue;
            }

            if (!StageProgress.IsWorldUnlocked(world))
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
    }
}

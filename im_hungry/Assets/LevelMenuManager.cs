using UnityEngine;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
    public GameObject[] levelButtons;

    private void Start()
    {

        foreach (GameObject button in levelButtons)
        {
            // Buttons are named after their stage number ("1", "2", ...)
            if (!int.TryParse(button.name, out int stage))
            {
                Debug.LogWarning("LevelMenuManager: button name is not a stage number: " + button.name);
                continue;
            }

            if (!StageProgress.IsStageUnlocked(stage))
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
    }
}

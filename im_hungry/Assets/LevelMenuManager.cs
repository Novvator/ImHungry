using UnityEngine;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
    public GameObject[] levelButtons;

    private void Start()
    {

        foreach (GameObject button in levelButtons)
        {
            if (!PlayerPrefs.HasKey("Stage " + (int.Parse(button.name) - 1) + " Completed"))
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
    }
}
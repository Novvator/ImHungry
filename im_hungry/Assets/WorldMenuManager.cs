using UnityEngine;
using UnityEngine.UI;

public class WorldMenuManager : MonoBehaviour
{
    public GameObject[] worldButtons;

    private void Start()
    {

        foreach (GameObject button in worldButtons)
        {
            if (!PlayerPrefs.HasKey("World " + (int.Parse(button.name) - 1) + " Completed"))
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
    }
}
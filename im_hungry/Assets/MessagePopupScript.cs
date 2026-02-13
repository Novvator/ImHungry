using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MessagePopupScript : MonoBehaviour
{
    [SerializeField] GameObject popupPrefab;

    public void ShowUnlockPopup(string levelName)
    {
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

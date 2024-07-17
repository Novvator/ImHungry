using UnityEngine;
using UnityEngine.EventSystems;

public class PopupSelfDestroyScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManagerScript.PlaySound("cloud");
        Destroy(gameObject);
    }
}

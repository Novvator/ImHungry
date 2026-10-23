using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpSelfDestroy : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManagerScript.PlaySound("cloud");
        Destroy(gameObject);
    }
}

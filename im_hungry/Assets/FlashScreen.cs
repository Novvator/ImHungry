using UnityEngine;

public class FlashScreen : MonoBehaviour
{
    [SerializeField] private SpriteRenderer flashRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.2f;

    private void Start()
    {
        flashRenderer.color = Color.clear;
    }

    public void Flash()
    {
        StartCoroutine(FlashCoroutine());
    }

    private System.Collections.IEnumerator FlashCoroutine()
    {
        flashRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        flashRenderer.color = Color.clear;
    }
}
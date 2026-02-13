using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPopupController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text titleText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Text itemNameText;

    [Header("Dots")]
    [SerializeField] private Transform dotsContainer;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Color activeDotColor = Color.white;
    [SerializeField] private Color inactiveDotColor = Color.gray;

    private List<UnlockItem> unlockItems;
    private int currentIndex = 0;

    public void Initialize(List<UnlockItem> items)
    {
        unlockItems = items;
        currentIndex = 0;

        titleText.text = "NEW!";
        CreateDots();
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextItem();
        }
    }

    private void NextItem()
    {
        currentIndex++;

        if (currentIndex >= unlockItems.Count)
        {
            ClosePopup();
            return;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        UnlockItem item = unlockItems[currentIndex];

        itemImage.sprite = item.itemSprite;
        itemNameText.text = item.itemName;

        UpdateDots();
    }

    private void CreateDots()
    {
        foreach (Transform child in dotsContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < unlockItems.Count; i++)
        {
            Instantiate(dotPrefab, dotsContainer);
        }
    }

    private void UpdateDots()
    {
        for (int i = 0; i < dotsContainer.childCount; i++)
        {
            Image dot = dotsContainer.GetChild(i).GetComponent<Image>();
            dot.color = (i == currentIndex) ? activeDotColor : inactiveDotColor;
        }
    }

    private void ClosePopup()
    {
        Destroy(gameObject);
    }
}

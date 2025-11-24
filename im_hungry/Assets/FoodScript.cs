using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    public float speed = 12f;

    private Sprite[] stageSprites = new Sprite[0];
    private SpriteRenderer spriteRenderer;

    private Vector3 targetPosition;
    private bool isMoving = false;

    // NEW: Track which stage (1,2,3)
    public int currentStage { get; private set; } = 1;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        }
    }

    public void Init(Sprite[] sprites)
    {
        if (sprites != null && sprites.Length > 0)
        {
            stageSprites = sprites;
            spriteRenderer.sprite = stageSprites[0];
            currentStage = 1;
        }
    }

    public void MoveToTarget(Vector3 target)
    {
        targetPosition = target;
        isMoving = true;
    }

    public void OnStage1()
    {
        if (stageSprites.Length > 0) spriteRenderer.sprite = stageSprites[0];
        currentStage = 1;
    }

    public void OnStage2()
    {
        if (stageSprites.Length > 1) spriteRenderer.sprite = stageSprites[1];
        currentStage = 2;
    }

    public void OnStage3()
    {
        if (stageSprites.Length > 2) spriteRenderer.sprite = stageSprites[2];
        currentStage = 3;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    // Movement speed towards the target position
    public float speed = 12f;

    // Sprites for the different stages (set at runtime by the spawner)
    private Sprite[] stageSprites = new Sprite[0];

    private SpriteRenderer spriteRenderer;

    // The world position this food will move towards
    private Vector3 targetPosition;

    // Whether this food is currently moving to the target
    private bool isMoving = false;

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
        // Only move if explicitly told to move
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        }
    }

    // Initialize the food at runtime with an array of sprites
    public void Init(Sprite[] sprites)
    {
        if (sprites != null && sprites.Length > 0)
        {
            stageSprites = sprites;
            spriteRenderer.sprite = stageSprites[0];
        }
    }

    // Start moving towards the target position (called by pattern logic)
    public void MoveToTarget(Vector3 target)
    {
        targetPosition = target;
        isMoving = true;
    }

    public void OnStage1()
    {
        if (stageSprites.Length > 0) spriteRenderer.sprite = stageSprites[0];
    }

    public void OnStage2()
    {
        if (stageSprites.Length > 1) spriteRenderer.sprite = stageSprites[1];
    }

    public void OnStage3()
    {
        if (stageSprites.Length > 2) spriteRenderer.sprite = stageSprites[2];
    }
}

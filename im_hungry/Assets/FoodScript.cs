using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    float speed = 12f;
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;
    [SerializeField] Sprite sprite3;
    private SpriteRenderer spriteRenderer;
    [SerializeField] GameObject target;
    //float Wradius = 1;
    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
    }


    public void OnStage1()
    {
        spriteRenderer.sprite = sprite1;
    }

    public void OnStage2()
    {
        spriteRenderer.sprite = sprite2;
    }

    public void OnStage3()
    {
        spriteRenderer.sprite = sprite3;
    }
}

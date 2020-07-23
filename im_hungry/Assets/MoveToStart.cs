using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToStart : MonoBehaviour
{
    public float speed = 1f;
    public GameObject target;
    float Wradius = 1;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
    }
}

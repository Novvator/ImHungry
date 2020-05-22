using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public GameObject[] objectss;
    // Start is called before the first frame update
    void Start()
    {
        objectss[4].SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(objectss[0].activeSelf == false && objectss[1].activeSelf == false 
            && objectss[2].activeSelf == false)
        {
            objectss[3].SetActive(false);
            objectss[4].SetActive(true);
        }
        
    }
 }


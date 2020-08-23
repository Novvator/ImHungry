using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public static AudioClip eatSound;
    public static AudioClip completeSound;
    public static AudioClip endSound;

    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        eatSound = Resources.Load<AudioClip>("eating1");
        completeSound = Resources.Load<AudioClip>("okpattern");
        endSound = Resources.Load<AudioClip>("end");
        audioSrc = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {


        
    }

    public static void PlaySound (string clip)
    {
        switch(clip)
        {
            case "eating1":
                audioSrc.PlayOneShot(eatSound);
                break;
            case "okpattern":
                audioSrc.PlayOneShot(completeSound);
                break;
            case "end":
                audioSrc.PlayOneShot(endSound);
                break;
        }
    }
}

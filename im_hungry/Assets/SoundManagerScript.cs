using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{

    public static AudioClip eatSound;
    public static AudioClip completeSound;
    public static AudioClip endSound;
    public static AudioClip winSound;
    public static AudioClip failSound;
    public static AudioClip endingSound;
    public static AudioClip endingSound2;
    public static AudioClip cloudSound;

    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        eatSound = Resources.Load<AudioClip>("eating1");
        completeSound = Resources.Load<AudioClip>("okpattern");
        endSound = Resources.Load<AudioClip>("end");
        failSound = Resources.Load<AudioClip>("fail3");
        winSound = Resources.Load<AudioClip>("win");
        endingSound = Resources.Load<AudioClip>("ending");
        endingSound2 = Resources.Load<AudioClip>("ending2");
        cloudSound = Resources.Load<AudioClip>("cloud");
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
            case "fail":
                audioSrc.PlayOneShot(failSound);
                break;
            case "win":
                audioSrc.PlayOneShot(winSound);
                break;
            case "ending":
                audioSrc.PlayOneShot(endingSound);
                break;   
            case "ending2":
                audioSrc.PlayOneShot(endingSound2);
                break;
            case "cloud":
                audioSrc.PlayOneShot(cloudSound);
                break;
        }
    }
}

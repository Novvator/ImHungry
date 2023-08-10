using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeToLevel ()
    {
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeOutComplete()
    {
        SceneManager.LoadScene(1);
        animator.SetTrigger("FadeIn");

    }
}
